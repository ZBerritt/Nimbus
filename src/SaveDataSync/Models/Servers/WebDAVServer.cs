using Dropbox.Api;
using Newtonsoft.Json.Linq;
using SaveDataSync.Models.Servers;
using SaveDataSync.Utils;
using System;
using System.Buffers.Text;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SaveDataSync.Servers
{
    internal class WebDAVServer : Server
    {
        private static HttpClient client;
        private Uri Uri { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }

        public override string Type => "WebDAV";

        public override string Host => Uri.Host;

        public async void Build(string uri, string username, string password)
        {
            Uri = new Uri(uri); // TODO: Auto format URL
            Username = username;
            Password = password;
            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(username, password),
            };
            client = new HttpClient(handler);
            await Setup();
        }

        public override Task DeserializeData(JObject data)
        {
            var username = data.GetValue("username").ToObject<string>();
            var password = data.GetValue("password").ToObject<string>();
            var uri = data.GetValue("expires").ToObject<string>();
            Build(uri, username, password);
            return Task.CompletedTask;
        }

        public override async Task<string> GetLocalSaveHash(string archiveLocation)
        {
            using var stream = File.OpenRead(archiveLocation);
            using var sha256 = SHA256.Create();
            var hashBytes = await sha256.ComputeHashAsync(stream);
            return Encoding.UTF8.GetString(hashBytes);
        }

        public async override Task<bool> GetOnlineStatus()
        {
            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Head,
                RequestUri = Uri
            };
            var res = await client.SendAsync(req);
            return res.StatusCode == HttpStatusCode.OK;
        }

        public override async Task<string> GetRemoteSaveHash(string name)
        {
            var savePath = Path.Combine(Uri.ToString(), "/SaveDataSync/Saves", name);
            using var sha256 = SHA256.Create();
            try
            {
                using var stream = await client.GetStreamAsync(savePath);
                var hashBytes = await sha256.ComputeHashAsync(stream);
                return Encoding.UTF8.GetString(hashBytes);

            } catch (HttpRequestException)
            {
                return "";
            }
        }

        public override async Task GetSaveData(string name, string destination)
        {
            if (!File.Exists(destination)) 
                throw new Exception("Destination file does not exist. Cannot retrieve data."); // should be abstracted
            var remotePath = $"/SaveDataSync/Saves/{name}";
            var response = await client.GetAsync(remotePath);
            response.EnsureSuccessStatusCode();
            using var remoteStream = await response.Content.ReadAsStreamAsync();
            using var destinationStream = File.OpenWrite(destination);
            await remoteStream.CopyToAsync(destinationStream);
        }

        public async override Task<string[]> SaveNames()
        {
            var remotePath = "/SaveDataSync/Saves/";
            var response = await client.GetAsync(remotePath);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var names = ParseNamesFromListing(content);
            return names;
        }

        public override Task<JObject> SerializeData()
        {
            return Task.FromResult(new JObject
            {
                { "username", Username },
                { "password", Password },
                { "uri", Uri },
            });
        }

        public override async Task UploadSaveData(string name, string source)
        {
            if (!File.Exists(source)) throw new Exception("Source file does not exist. Cannot upload.");
            var savePath = Path.Combine("/SaveDataSync/Saves", name);
            using var sourceStream = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.Read);
            await client.PutFile(savePath, sourceStream);
        }

        private async Task Setup()
        {
            await client.Mkcol("/SaveDataSync/");
            await client.Mkcol("/SaveDataSync/Saves");
        }

        private static string[] ParseNamesFromListing(string listing)
        {
            var names = listing.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < names.Length; i++)
            {
                // remove leading and trailing whitespace
                names[i] = names[i].Trim();
                // remove the .zip extension
                if (names[i].EndsWith(".zip"))
                {
                    names[i] = names[i].Substring(0, names[i].Length - 4);
                }
            }
            return names;
        }
    }
}
