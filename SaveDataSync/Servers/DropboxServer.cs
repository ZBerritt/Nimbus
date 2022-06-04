using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SaveDataSync.Servers
{
    public class DropboxServer : IServer
    {
        public static string APP_ID = "i136jjbqxg4aaci";
        private static readonly HttpClient client = new();

        public string BearerKey { get; private set; } // The acual key used to make requests
        public string RefreshKey { get; private set; } // A key used to refresh the bearer key when expires
        public DateTime Expires { get; private set; } // When the bearer key expires
        public string ApiKey { get; private set; } // Does nothing honestly

        public string Name { get; } = "Dropbox";
        public string Host { get; } = "dropbox.com";

        public DropboxServer(string bearerKey, string refreshKey, DateTime expires, string apiKey)
        {
            BearerKey = bearerKey;
            RefreshKey = refreshKey;
            Expires = expires;
            ApiKey = apiKey;
        }

        public static DropboxServer Build(string apiKey, string verifier)
        {
            var values = new Dictionary<string, string>
            {
                { "client_id", APP_ID },
                { "redirect_uri", "http://localhost:1235" },
                { "grant_type", "authorization_code"},
                { "code", apiKey },
                { "code_verifier", verifier },
            };
            var content = new FormUrlEncodedContent(values);

            var response = client.PostAsync("https://api.dropboxapi.com/oauth2/token", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            var responseObject = JObject.Parse(responseString);
            try
            {
                long expiresIn = long.Parse(responseObject.GetValue("expires_in").ToString());
                string refresh = responseObject.GetValue("refresh_token").ToString();
                string access = responseObject.GetValue("access_token").ToString();
                return new DropboxServer(access, refresh, DateTime.Now.Add(TimeSpan.FromSeconds(expiresIn)), apiKey);
            }
            catch (Exception)
            {
                return null; // TODO: Refactor the error handling
            }
        }

        internal static string GetApiKey()
        {
            string key = null;
            int port = 1235;
            var listener = new TcpListener(IPAddress.Parse("127.0.0.01"), port);
            listener.Start();

            using var client = listener.AcceptTcpClient();
            using var stream = client.GetStream();
            using var writer = new StreamWriter(stream, Encoding.ASCII) { AutoFlush = true };
            using var reader = new StreamReader(stream, Encoding.ASCII);
            try
            {
                string inputLine = reader.ReadLine(); // First line contains the request
                if (inputLine.Contains("code="))
                {
                    string substring = inputLine.Substring(inputLine.IndexOf("code=") + "code=".Length);
                    key = substring.Substring(0, substring.IndexOf(" "));
                    var response = Encoding.ASCII.GetBytes("Key obtained! You may now close this tab!");
                    client.Client.Send(response);
                }
            }
            catch (Exception)
            {
                var response = Encoding.ASCII.GetBytes("Some error has occured, please try again...");
                return null;
            }

            return key;
        }

        public static DropboxServer BuildFromJson(JObject json)
        {
            var bearer = json.GetValue("bearerKey").ToObject<string>();
            var refresh = json.GetValue("refreshKey").ToObject<string>();
            var expires = json.GetValue("expires").ToObject<DateTime>();
            var apiKey = json.GetValue("apiKey").ToObject<string>();
            return new DropboxServer(bearer, refresh, expires, apiKey);
        }

        public byte[] GetSaveData(string name)
        {
            string urlPath = $"/{name}.zip"; // ALWAYS on dropbox under this name
            try
            {
                var reqBody = new JObject
                {
                    { "path", urlPath }
                };
                var jsonData = JsonConvert.SerializeObject(reqBody);

                var request = new HttpRequestMessage(HttpMethod.Post, "https://content.dropboxapi.com/2/files/download");
                request.Headers.Add("Authorization", "Bearer " + GetBearerKey());
                request.Headers.Add("Dropbox-API-Arg", jsonData);

                var response = client.SendAsync(request).Result;
                if (response.StatusCode == HttpStatusCode.Conflict) return Array.Empty<byte>(); // Return nothing if the server errors
                var content = response.Content.ReadAsByteArrayAsync().Result;
                return content;
            }
            catch (Exception)
            {
                return Array.Empty<byte>();
            }
        }

        public string[] SaveNames()
        {
            var reqBody = new JObject
            {
                { "path", "" },
                { "include_has_explicit_shared_members", false },
                { "recursive", false },
                { "include_deleted", false },
                { "include_non_downloadable_files", false }
            };
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/files/list_folder");
            request.Headers.Add("Authorization", "Bearer " + GetBearerKey());
            request.Content = new StringContent(reqBody.ToString());
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = client.SendAsync(request).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            var responseObject = JObject.Parse(result);
            var entries = responseObject.GetValue("entries").ToObject<JArray>();
            var names = new List<string>();
            foreach (JObject entry in entries)
            {
                var name = entry.GetValue("name").ToObject<string>();
                names.Add(name[..^4]);
            }

            return names.ToArray();
        }

        public bool ServerOnline()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/check/user");
                request.Headers.Add("Authorization", "Bearer " + GetBearerKey());
                request.Content = new StringContent("{\"query\": \"verify\"}", Encoding.UTF8, "application/json");
                var response = client.SendAsync(request).Result;
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void UploadSaveData(string name, byte[] data)
        {
            string urlPath = $"/{name}.zip"; // ALWAYS on dropbox under this name
            var reqBody = new JObject
            {
                { "path", urlPath },
                { "mode", "overwrite" },
                { "autorename", false },
                { "mute", false },
                { "strict_conflict", false }
            };
            var jsonData = JsonConvert.SerializeObject(reqBody);

            var request = new HttpRequestMessage(HttpMethod.Post, "https://content.dropboxapi.com/2/files/upload");
            request.Headers.Add("Authorization", "Bearer " + GetBearerKey());
            request.Headers.Add("Dropbox-API-Arg", jsonData);
            request.Content = new ByteArrayContent(data);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            client.SendAsync(request);
        }

        private string GetBearerKey()
        {
            if (Expires.CompareTo(DateTime.Now) > 0) return BearerKey;

            // Key expired, generate a new one
            var values = new Dictionary<string, string>
            {
                { "client_id", APP_ID },
                { "grant_type", "refresh_token"},
                { "refresh_token", RefreshKey }
            };
            var content = new FormUrlEncodedContent(values);
            var response = client.PostAsync("https://api.dropboxapi.com/oauth2/token", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            JObject responseObject = JObject.Parse(responseString);
            long expiresIn = long.Parse(responseObject.GetValue("expires_in").ToString());
            string access = responseObject.GetValue("access_token").ToString();
            RenewBearerKey(access, expiresIn);
            return BearerKey;
        }

        private void RenewBearerKey(string key, long expiresIn)
        {
            BearerKey = key;
            Expires = DateTime.Now.Add(TimeSpan.FromSeconds(expiresIn));
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

        public JObject ToJson()
        {
            return new JObject
            {
                { "bearerKey", BearerKey },
                { "refreshKey", RefreshKey },
                { "expires", Expires },
                { "apiKey", ApiKey }
            };
        }

        public string GetRemoteSaveHash(string name)
        {
            string urlPath = $"/{name}.zip"; // ALWAYS on dropbox under this name
            var reqBody = new JObject
            {
                { "path", urlPath },
                {"include_media_info", false },
                { "include_deleted", false },
                { "include_has_explicit_shared_members", false },
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.dropboxapi.com/2/files/get_metadata");
            request.Headers.Add("Authorization", "Bearer " + GetBearerKey());
            request.Content = new StringContent(reqBody.ToString());
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var response = client.SendAsync(request).Result;
            var jsonResponse = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            string hash = jsonResponse.GetValue("content_hash").ToObject<string>();
            return hash;
        }

        public string GetLocalSaveHash(byte[] data)
        {
            int BLOCK_SIZE = 1024 * 1024 * 4; // 4 MB Blocks
            var concatHashes = new List<byte>(); // SHA-256 hashes for each block
            var sha256 = SHA256.Create();

            int i = 0;
            while (i < data.Length)
            {
                if (i + BLOCK_SIZE > data.Length - 1) // If the file doens't have another 4 MB left...
                {
                    var smallSegment = new byte[data.Length - i]; // Create smaller segment to the end
                    Array.Copy(data, i, smallSegment, 0, data.Length - i);
                    var lastHash = sha256.ComputeHash(smallSegment);
                    concatHashes.AddRange(lastHash);
                    break;
                }
                var segment = new byte[BLOCK_SIZE];
                Array.Copy(data, i, segment, 0, BLOCK_SIZE); // 4 MB segment of array
                var hash = sha256.ComputeHash(segment);
                concatHashes.AddRange(hash);

                i += BLOCK_SIZE;
            }

            byte[] bytes = concatHashes.ToArray();
            var concatHash = sha256.ComputeHash(bytes);
            var hex = BitConverter.ToString(concatHash, 0, bytes.Length).Replace("-", "").ToLower();

            return hex;
        }
    }
}