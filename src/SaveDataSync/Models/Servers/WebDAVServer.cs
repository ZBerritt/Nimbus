using Newtonsoft.Json.Linq;
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
using WebDav;

namespace SaveDataSync.Servers
{
    internal class WebDAVServer : Server
    {
        private static readonly HttpClient client = new();
        private Uri Uri { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }

        private AuthenticationHeaderValue Credentials => new("Basic",
            Convert.ToBase64String(
                Encoding.UTF8.GetBytes($"{Username}:{Password}")));

        public override string Type => "WebDAV";

        public override string Host => Uri.Host;

        public async void Build(string uri, string username, string password)
        {
            Uri = new Uri(OtherUtils.ParseUrlInText(uri));
            Username = username;
            Password = password;
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
            req.Headers.Authorization = Credentials;
            var res = await client.SendAsync(req);
            return res.StatusCode == HttpStatusCode.OK;
        }

        public override async Task<string> GetRemoteSaveHash(string name)
        {
            var savePath = Path.Combine(Uri.ToString(), "/SaveDataSync/Saves", name);
            using var stream = await client.GetRawFile(savePath);
            if (stream.StatusCode > 300) return "";
            using var sha256 = SHA256.Create();
            var hashBytes = await sha256.ComputeHashAsync(stream.Stream);
            return Encoding.UTF8.GetString(hashBytes);
        }

        public override async Task GetSaveData(string name, string destination)
        {
            var req = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = SaveUri(name)
            };
            req.Headers.Authorization = Credentials;
            req.Headers.Accept = "";
            var savePath = Path.Combine("/SaveDataSync/Saves", name);
            if (!File.Exists(destination)) throw new Exception("Destination file does not exist. Cannot retrieve data."); // should be abstracted
            using var destinationStream = File.OpenWrite(destination);
            using var stream = await client.GetRawFile(savePath);
            if (stream.StatusCode > 300) return;
            await stream.Stream.CopyToAsync(destinationStream);
        }

        public async override Task<string[]> SaveNames()
        {
            var res = await client.Propfind("/SavesDataSync/Saves/");
            return res.Resources
                .Where(r => r.DisplayName.EndsWith(".zip"))
                .Select(r => r.DisplayName)
                .ToArray();
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

        private Uri SaveUri(string name) => new(Path.Combine(Uri.ToString(), "/SaveDataSync/Saves", $"{name}.zip"));
    }
}
