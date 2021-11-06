using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SaveDataSync.Servers
{
    internal class DropboxServer : Server
    {
        public static string APP_ID = "i136jjbqxg4aaci";
        private static readonly HttpClient client = new HttpClient();


        private string bearerKey; // The acual key used to make requests
        private string refreshKey; // A key used to refresh the bearer key when expires
        private DateTime expires; // When the bearer key expires

        public DropboxServer(string bearerKey, string refreshKey, DateTime expires)
        {
            this.bearerKey = bearerKey;
            this.refreshKey = refreshKey;
            this.expires = expires;
        }

        public async static Task<DropboxServer> Build(string apiKey, string verifier)
        {
            var values = new Dictionary<string, string>
            {
                { "client_id", APP_ID },
                { "redirect_uri", "http://localhost:1235/callback" },
                { "grant_type", "authorization_code"},
                { "code", apiKey },
                { "code_verifier", verifier },
            };
            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("https://api.dropboxapi.com/oauth2/token", content);
            var responseString = await response.Content.ReadAsStringAsync();
            JObject responseObject = JObject.Parse(responseString);
            long expiresIn = long.Parse(responseObject.GetValue("expires_in").ToString());
            string refresh = responseObject.GetValue("refresh_token").ToString();
            string access = responseObject.GetValue("access_token").ToString();
            Console.WriteLine(responseObject.GetValue("expires_in"));
            Console.WriteLine(responseObject.GetValue("refresh_token"));
            Console.WriteLine(responseObject.GetValue("access_token"));
            return new DropboxServer(access, refresh, DateTime.Now.Add(TimeSpan.FromSeconds(expiresIn)));
        }
        public override byte[] GetSaveData(string name)
        {
            string urlPath = "/" + name + ".zip"; // Stored on dropbox under this name
            throw new NotImplementedException();

        }

        public override string[] SaveNames()
        {
            throw new NotImplementedException();
        }

        public override bool ServerOnline()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/check/user");
            request.Headers.Add("Authorization", "Bearer " + GetBearerKey());
            request.Content = new StringContent("{\"query\": \"verify\"}", Encoding.UTF8, "application/json");
            var response = client.SendAsync(request).Result;
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(System.Net.HttpStatusCode.OK);
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public override void UploadSaveData(string name, byte[] data)
        {
            throw new NotImplementedException();
        }

        private string GetBearerKey()
        {
            if (expires.CompareTo(DateTime.Now) > 0) return bearerKey;

            // Key expired, generate a new one
            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/oauth2/token");
            request.Headers.Add("Accept", "application/json");

            // Build url encoded data
            StringBuilder postDataBuilder = new StringBuilder();
            postDataBuilder.Append("client_id=").Append(APP_ID);
            postDataBuilder.Append("&grant_type=refresh_token");
            postDataBuilder.Append("&refresh_token=").Append(refreshKey);
            request.Content = new StringContent(postDataBuilder.ToString(),
                    Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = client.SendAsync(request).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(response);
            JObject responseObject = JObject.Parse(responseString);
            Console.WriteLine(responseObject.GetValue("expires_in"));
            Console.WriteLine(responseObject.GetValue("access_token"));
            return "";
        }

        public static string GenerateVerifier()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
            var random = new Random();
            var verifier = new char[64];
            for (int i = 0; i < verifier.Length; i++)
            {
                verifier[i] = chars[random.Next(chars.Length)];
            }

            return new string(verifier);
        }

        public static string GenerateCodeChallenge(string verifier)
        {
            var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(verifier));
            var b64Hash = Convert.ToBase64String(hash);
            var code = Regex.Replace(b64Hash, "\\+", "-");
            code = Regex.Replace(code, "\\/", "_");
            code = Regex.Replace(code, "=+$", "");
            return code;
        }
    }
}
