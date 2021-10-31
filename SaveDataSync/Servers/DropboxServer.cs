using System;
using System.Net.Http;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.IO;

namespace SaveDataSync.Servers
{
    internal class DropboxServer : Server
    {
        public static string APP_ID = "i136jjbqxg4aaci";


        private string bearerKey; // The acual key used to make requests
        private string refreshKey; // A key used to refresh the bearer key when expires
        private DateTime expires; // When the bearer key expires

        public DropboxServer(string apiKey, string verifier)
        {
            HttpClient client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/oauth2/token");
            request.Headers.Add("Accept", "application/json");

            // Build url encoded data
            StringBuilder postDataBuilder = new StringBuilder();
            postDataBuilder.Append("client_id=").Append(APP_ID);
            postDataBuilder.Append("&redirect_uri=http://localhost:1235");
            postDataBuilder.Append("&grant_type=authorization_code");
            postDataBuilder.Append("&code=").Append(apiKey);
            postDataBuilder.Append("&code_verifier=").Append(verifier);
            request.Content = new StringContent(postDataBuilder.ToString(),
                    Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = client.SendAsync(request).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine(response);
            JObject responseObject = JObject.Parse(responseString);
            Console.WriteLine(responseObject.GetValue("expires_in"));
            Console.WriteLine(responseObject.GetValue("refresh_token"));
            Console.WriteLine(responseObject.GetValue("access_token"));
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
            try
            {
                HttpClient client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/check/user");
                request.Headers.Add("Authorization", "Bearer " + GetBearerKey());
                request.Headers.Add("Accept", "application/json");
                request.Content = new StringContent("{\"query\": \"verify\"}", Encoding.UTF8, "application/json");
                client.SendAsync(request).ContinueWith(res =>
                {
                    return res.Result.StatusCode == System.Net.HttpStatusCode.OK;
                });
            }
            catch (HttpRequestException e)
            {
                Console.Write(e.Message);
                return false;
            }

            return false;
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
            var verifier = new char[128];
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
