using Dropbox.Api;
using Dropbox.Api.Users;
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
using System.Xml.Linq;

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

        public override async Task Build(params string[] args)
        {
            // TODO: Possible validate args in case something horrible goes wrong
            Uri = new Uri(args[0]); // TODO: Auto format URL
            Username = args[1];
            Password = args[2];
            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(Username, Password),
            };
            client = new HttpClient(handler);
            await Setup();
        }

        public override async Task DeserializeData(JObject data)
        {
            var username = data.GetValue("username").ToObject<string>();
            var password = data.GetValue("password").ToObject<string>();
            var uri = data.GetValue("uri").ToObject<string>();
            await Build(new string[]{ uri, username, password });
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
            var remotePath = GetSavePath(name);
            using var sha256 = SHA256.Create();
            var response = await client.GetAsync(remotePath);
            response.EnsureSuccessStatusCode();
            using var stream = await response.Content.ReadAsStreamAsync();
            var hashBytes = await sha256.ComputeHashAsync(stream);
            return Encoding.UTF8.GetString(hashBytes);
        }

        public override async Task GetSaveData(string name, string destination)
        {
            if (!File.Exists(destination)) 
                throw new Exception("Destination file does not exist. Cannot retrieve data."); // should be abstracted
            var remotePath = GetSavePath(name);
            var response = await client.GetAsync(remotePath);
            response.EnsureSuccessStatusCode();
            using var remoteStream = await response.Content.ReadAsStreamAsync();
            using var destinationStream = File.OpenWrite(destination);
            await remoteStream.CopyToAsync(destinationStream);
        }

        public async override Task<string[]> SaveNames()
        {
            var remotePath = GetSavePath();
            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = remotePath
            };
            var response = await client.SendAsync(req);
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
            var savePath = GetSavePath(name);
            using var sourceStream = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.Read);
            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Put,
                RequestUri = savePath,
                Content = new StreamContent(sourceStream)
            };
            req.Content.Headers.ContentLength = sourceStream.Length;
            var res = await client.SendAsync(req);
            
        }

        private async Task Setup()
        {
            var savePath = GetSavePath();
            var headRequest = new HttpRequestMessage(HttpMethod.Head, savePath);
            var headResponse = await client.SendAsync(headRequest);
            bool exists = headResponse.IsSuccessStatusCode;

            if (!exists)
            {
                var mkcolRequest = new HttpRequestMessage(new HttpMethod("MKCOL"), savePath);
                await client.SendAsync(mkcolRequest); // May need handling
            }
        }

        private Uri GetSavePath()
        {
            return new Uri($"{Uri}/SaveDataSync/Saves/");
        }

        private Uri GetSavePath(string name)
        {
            return new Uri($"{Uri}/SaveDataSync/Saves/{name}");
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
                    names[i] = names[i][..^4];
                }
            }
            return names;
        }
    }
}
