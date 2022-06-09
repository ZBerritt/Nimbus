using Dropbox.Api;
using Dropbox.Api.Files;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace SaveDataSync.Servers
{
    public class DropboxServer : IServer
    {
        private static string APP_ID = "i136jjbqxg4aaci";
        private static string LoopbackHost = "http://127.0.0.1:12356/";
        private static readonly Uri RedirectUri = new(LoopbackHost + "authorize");
        private static readonly Uri JSRedirectUri = new(LoopbackHost + "token");

        private string AccessToken { get; set; }
        private string RefreshToken { get; }
        private DateTime Expires { get; set; }
        public string Uid { get; }

        private DropboxClient DropboxClient { get; }

        public string Name => "Dropbox";

        public string Host => "dropbox.com";

        public static async Task<DropboxServer> Build()
        {
            var http = new HttpListener();
            try
            {
                var state = Guid.NewGuid().ToString("N"); // Used for verification
                var OAuthFlow = new PKCEOAuthFlow();
                var authorizeUri = OAuthFlow.GetAuthorizeUri(OAuthResponseType.Code, APP_ID,
                    RedirectUri.ToString(), state: state, tokenAccessType: TokenAccessType.Offline,
                    scopeList: null, includeGrantedScopes: IncludeGrantedScopes.None);

                http.Prefixes.Add(LoopbackHost);

                http.Start();

                Utils.OpenUrl(authorizeUri.ToString());

                await Utils.HandleOAuth2Redirect(http, RedirectUri);

                var redirectUri = await Utils.HandleJSCodeRequest(http, JSRedirectUri);
                var tokenResult = await OAuthFlow.ProcessCodeFlowAsync(redirectUri, APP_ID, RedirectUri.ToString(), state);
                var accessToken = tokenResult.AccessToken;
                var refreshToken = tokenResult.RefreshToken;
                var expires = tokenResult.ExpiresAt;
                if (expires is null) throw new Exception("No expire time found. This is probably my fault :(");
                var uid = tokenResult.Uid;
                http.Stop();

                return new DropboxServer(accessToken, refreshToken, (DateTime)expires, uid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                http.Stop();
                return null;
            }
        }

        public static DropboxServer Build(JObject json)
        {
            var accessToken = json.GetValue("accessToken").ToObject<string>();
            var refreshToken = json.GetValue("refreshToken").ToObject<string>();
            var expires = json.GetValue("expires").ToObject<DateTime>();
            var uid = json.GetValue("uid").ToObject<string>();
            return new DropboxServer(accessToken, refreshToken, expires, uid);
        }

        public string[] SaveNames()
        {
            try
            {
                var result = DropboxClient.Files.ListFolderAsync(new ListFolderArg("",
                    recursive: false, includeMediaInfo: false, includeDeleted: false, includeMountedFolders: false)).Result;

                var entries = result.Entries;
                var nameList = new List<string>();
                return entries.Where(c => c.Name.EndsWith(".zip")).Select(s => s.Name[..^4]).ToArray();
            }
            catch (Exception)
            {
                return Array.Empty<string>();
            }
        }

        public void GetSaveData(string name, string destination)
        {
            if (!File.Exists(destination)) throw new Exception("Destination file does not exist. Cannot retrieve data.");
            using var destinationStream = File.OpenWrite(destination);
            var fileName = $"/{name}.zip";
            var response = DropboxClient.Files.DownloadAsync(new DownloadArg(fileName)).Result; // This will actually download the file
            response.GetContentAsStreamAsync().Result.CopyTo(destinationStream);
        }

        public void UploadSaveData(string name, string source)
        {
            if (!File.Exists(source)) throw new Exception("Source file does not exist. Cannot upload.");
            var fileName = $"/{name}.zip";
            using var sourceStream = File.OpenRead(source);
            DropboxClient.Files.UploadAsync(new UploadArg(fileName,
                mode: WriteMode.Overwrite.Instance, autorename: false, mute: false, strictConflict: false), sourceStream).Wait();
        }

        public bool ServerOnline()
        {
            try
            {
                var account = DropboxClient.Check.UserAsync().Result;
                return account is not null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetRemoteSaveHash(string name)
        {
            try
            {
                var fileName = $"/{name}.zip";
                var metadata = DropboxClient.Files.GetMetadataAsync(new GetMetadataArg(fileName,
                    includeMediaInfo: false, includeDeleted: false, includeHasExplicitSharedMembers: false)).Result;
                return metadata.AsFile.ContentHash;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public string GetLocalSaveHash(string archiveLocation)
        {
            if (!File.Exists(archiveLocation)) return string.Empty;
            using var fileStream = File.Open(archiveLocation, FileMode.Open);

            var buffer = new byte[4096];
            var hasher = new DropboxContentHasher();

            var n = fileStream.Read(buffer, 0, buffer.Length);
            while (n > 0)
            {
                hasher.TransformBlock(buffer, 0, n, buffer, 0);
                n = fileStream.Read(buffer, 0, buffer.Length);
            }

            hasher.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            var hex = DropboxContentHasher.ToHex(hasher.Hash);
            return hex;
        }

        public JObject Serialize()
        {
            return new JObject
            {
                { "accessToken", AccessToken },
                { "refreshToken", RefreshToken },
                { "expires", Expires },
                { "uid", Uid }
            };
        }

        public DropboxServer(string accessToken, string refreshToken, DateTime expires, string uid)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            Expires = expires;
            Uid = uid;
            DropboxClient = new DropboxClient(accessToken, refreshToken, expires, APP_ID); // Weird behavior, doesn't reload when expired. Will be testing later.
        }
    }
}