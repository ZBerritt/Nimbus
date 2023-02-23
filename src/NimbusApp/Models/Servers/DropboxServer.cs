using Dropbox.Api;
using Dropbox.Api.Check;
using Dropbox.Api.Files;
using NimbusApp.Utils;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NimbusApp.Models.Servers
{
    public class DropboxServer : Server
    {
        // Constants
        private static readonly string APP_ID = "i136jjbqxg4aaci";
        private static readonly string LoopbackHost = "http://127.0.0.1:12356/";
        private static readonly Uri RedirectUri = new(LoopbackHost + "authorize");
        private static readonly Uri JSRedirectUri = new(LoopbackHost + "token");

        // Properties
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expires { get; set; }
        public string Uid { get; set; }

        // Utilities
        private DropboxClient DropboxClient => new DropboxClient(AccessToken, RefreshToken, Expires, APP_ID);


        public override string Type => "Dropbox";

        public override string Host => "dropbox.com";

        protected override async Task Build(params string[] _)
        {
            using var http = new HttpListener();
            try
            {
                var state = Guid.NewGuid().ToString("N"); // Used for verification
                var OAuthFlow = new PKCEOAuthFlow();
                var authorizeUri = OAuthFlow.GetAuthorizeUri(OAuthResponseType.Code, APP_ID,
                    RedirectUri.ToString(), state: state, tokenAccessType: TokenAccessType.Offline,
                    scopeList: null, includeGrantedScopes: IncludeGrantedScopes.None);

                http.Prefixes.Add(LoopbackHost);

                http.Start();

                OtherUtils.OpenUrl(authorizeUri.ToString());

                await DropboxUtils.HandleOAuth2Redirect(http, RedirectUri);

                var redirectUri = await DropboxUtils.HandleJSCodeRequest(http, JSRedirectUri);
                var tokenResult = await OAuthFlow.ProcessCodeFlowAsync(redirectUri, APP_ID, RedirectUri.ToString(), state);
                var accessToken = tokenResult.AccessToken;
                var refreshToken = tokenResult.RefreshToken;
                var expires = (DateTime)tokenResult.ExpiresAt;
                var uid = tokenResult.Uid;

                AccessToken = accessToken;
                RefreshToken = refreshToken;
                Expires = expires;
                Uid = uid;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public override async Task<string[]> SaveNames()
        {
            try
            {
                var result = await DropboxClient.Files.ListFolderAsync(new ListFolderArg("",
                    recursive: false, includeMediaInfo: false, includeDeleted: false, includeMountedFolders: false));

                var entries = result.Entries;
                return entries.Where(c => c.Name.EndsWith(".zip")).Select(s => s.Name[..^4]).ToArray();
            }
            catch (Exception)
            {
                return Array.Empty<string>();
            }
        }

        public override async Task GetSaveData(string name, string destination)
        {
            if (!File.Exists(destination)) throw new Exception("Destination file does not exist. Cannot retrieve data.");
            using var destinationStream = File.OpenWrite(destination);
            var fileName = $"/{name}.zip";
            var response = await DropboxClient.Files.DownloadAsync(new DownloadArg(fileName)); // This will actually download the file
            var responseStream = await response.GetContentAsStreamAsync();
            await responseStream.CopyToAsync(destinationStream);
        }

        public override async Task UploadSaveData(string name, string source)
        {
            if (!File.Exists(source)) throw new Exception("Source file does not exist. Cannot upload.");
            var fileName = $"/{name}.zip";
            using var sourceStream = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.Read);
            await DropboxClient.Files.UploadAsync(new UploadArg(fileName,
                mode: WriteMode.Overwrite.Instance, autorename: false, mute: false, strictConflict: false), sourceStream);
        }

        public override async Task<bool> GetOnlineStatus()
        {
            try
            {
                EchoResult account = await DropboxClient.Check.UserAsync(); // This REALLY likes to hang...
                return account is not null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<string> GetRemoteSaveHash(string name)
        {
            try
            {
                var fileName = $"/{name}.zip";
                var metadata = await DropboxClient.Files.GetMetadataAsync(new GetMetadataArg(fileName,
                    includeMediaInfo: false, includeDeleted: false, includeHasExplicitSharedMembers: false));
                return metadata.AsFile.ContentHash;
            }
            catch (Exception)
            {
                return string.Empty; // TODO: Fix error handling when save hash cannot be found
            }
        }

        public override async Task<string> GetLocalSaveHash(string archiveLocation)
        {
            if (!File.Exists(archiveLocation)) return string.Empty;
            using var fileStream = File.Open(archiveLocation, FileMode.Open, FileAccess.Read, FileShare.Read);

            var buffer = new byte[4096];
            var hasher = new DropboxContentHasher();

            var n = await fileStream.ReadAsync(buffer);
            while (n > 0)
            {
                hasher.TransformBlock(buffer, 0, n, buffer, 0);
                n = await fileStream.ReadAsync(buffer);
            }

            hasher.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            var hex = DropboxContentHasher.ToHex(hasher.Hash);
            return hex;
        }
    }
}