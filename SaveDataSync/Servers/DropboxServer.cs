using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;

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

        public static DropboxServer Build(string apiKey, string verifier)
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

            var response = client.PostAsync("https://api.dropboxapi.com/oauth2/token", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            JObject responseObject = JObject.Parse(responseString);
            long expiresIn = long.Parse(responseObject.GetValue("expires_in").ToString());
            string refresh = responseObject.GetValue("refresh_token").ToString();
            string access = responseObject.GetValue("access_token").ToString();
            return new DropboxServer(access, refresh, DateTime.Now.Add(TimeSpan.FromSeconds(expiresIn)));
        }

        public static DropboxServer BuildFromJson(JObject json)
        {
            string bearer = (string)json.GetValue("bearerKey");
            string refresh = (string)json.GetValue("refreshKey");
            DateTime expires = (DateTime)json.GetValue("expires");
            return new DropboxServer(bearer, refresh, expires);

        }
        public override string Name()
        {
            return "Dropbox";
        }

        public override string Host()
        {
            return "dropbox.com";
        }
        public override byte[] GetSaveData(string name)
        {
            string urlPath = "/" + name + ".zip"; // Stored on dropbox under this name
            try
            {
                JObject reqBody = new JObject();
                reqBody.Add("path", urlPath);
                var request = new HttpRequestMessage(HttpMethod.Post, "https://content.dropboxapi.com/2/files/download");
                request.Headers.Add("Authorization", "Bearer " + GetBearerKey());
                request.Headers.Add("Dropbox-API-Arg", reqBody.ToString());


                var response = client.SendAsync(request).Result;
                var content = response.Content.ReadAsByteArrayAsync().Result;
                return content;
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }

        }

        public override string[] SaveNames()
        {
            JObject reqBody = new JObject();
            reqBody.Add("path", "");
            reqBody.Add("include_non_downloadable_files", false);
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/files/list_folder");
            request.Headers.Add("Authorization", "Bearer " + GetBearerKey());
            request.Content = new StringContent(reqBody.ToString());
            
            var response = client.SendAsync(request).Result;
            JObject responseObject = new JObject(response.Content.ToString());
            JArray entries = (JArray)responseObject.GetValue("entries");
            List<string> names = new List<string>();
            foreach (JObject entry in entries)
            {
                var name = (string)entry.GetValue("name");
                names.Add(name);
            }

            return names.ToArray();
        }

        public override bool ServerOnline()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/check/user");
            request.Headers.Add("Authorization", "Bearer " + GetBearerKey());
            request.Content = new StringContent("{\"query\": \"verify\"}", Encoding.UTF8, "application/json");
            var response = client.SendAsync(request).Result;
            return response.StatusCode == System.Net.HttpStatusCode.OK;
        }

        public override void UploadSaveData(string name, byte[] data)
        {
            string urlPath = "/" + name + ".zip"; // Stored on dropbox under this name
            JObject reqBody = new JObject();
            reqBody.Add("path", urlPath);
            reqBody.Add("mode", "overwrite");
            reqBody.Add("autorename", false);
            reqBody.Add("mute", false);
            reqBody.Add("strict_conflict", false);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://content.dropboxapi.com/2/files/upload");
            request.Headers.Add("Authorization", "Bearer " + GetBearerKey());
            request.Headers.Add("Dropbox-API-Arg", reqBody.ToString());
            request.Content = new ByteArrayContent(data);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            client.SendAsync(request);
        }

        private string GetBearerKey()
        {
            if (expires.CompareTo(DateTime.Now) > 0) return bearerKey;

            // Key expired, generate a new one
            var values = new Dictionary<string, string>
            {
                { "client_id", APP_ID },
                { "grant_type", "refresh_token"},
                { "refresh_token", refreshKey }
            };
            var content = new FormUrlEncodedContent(values);
            var response = client.PostAsync("https://api.dropboxapi.com/oauth2/token", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            JObject responseObject = JObject.Parse(responseString);
            long expiresIn = long.Parse(responseObject.GetValue("expires_in").ToString());
            string access = responseObject.GetValue("access_token").ToString();
            RenewBearerKey(access, expiresIn);
            return bearerKey;
        }

        private void RenewBearerKey(string key, long expiresIn)
        {
            bearerKey = key;
            expires = DateTime.Now.Add(TimeSpan.FromSeconds(expiresIn));
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

        public override JObject ToJson()
        {
            JObject json = new JObject();
            json.Add("bearerKey", bearerKey);
            json.Add("refreshKey", refreshKey);
            json.Add("expires", expires);
            return json;
        }
    }
}
