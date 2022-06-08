using Dropbox.Api;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
        }

        public byte[] GetSaveData(string name)
        {
            throw new NotImplementedException();
        }

        public void UploadSaveData(string name, byte[] data)
        {
            throw new NotImplementedException();
        }

        public bool ServerOnline()
        {
            try
            {
                var account = DropboxClient.Users.GetCurrentAccountAsync().Result;
                return account is not null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetRemoteSaveHash(string name)
        {
            throw new NotImplementedException();
        }

        public string GetLocalSaveHash(string archiveLocation)
        {
            throw new NotImplementedException();
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
            DropboxClient = new DropboxClient(accessToken, refreshToken, expires, APP_ID);
        }
    }
}