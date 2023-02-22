using NimbusApp.Models.Servers;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NimbusApp.Servers
{
    internal class WebDAVServer : Server
    {
        

        // Properties
        public Uri Uri { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }

        // Utilities
        private HttpClientHandler Handler => new()
        {
            Credentials = new NetworkCredential(Username, Password),
        };
        private HttpClient Client => new(Handler);


        public override string Type => "WebDAV";

        public override string Host => Uri.Host;

        protected override async Task Build(params string[] args)
        {
            // TODO: Possible validate args in case something horrible goes wrong
            Uri = new Uri(args[0]); // TODO: Auto format URL
            Username = args[1];
            Password = args[2];
            await Setup();
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
                RequestUri = GetSavePath()
            };
            var res = await Client.SendAsync(req);
            return res.IsSuccessStatusCode;
        }

        public override async Task<string> GetRemoteSaveHash(string name)
        {
            var remotePath = GetSavePath(name);
            using var sha256 = SHA256.Create();
            try
            {
                var response = await Client.GetAsync(remotePath);
                response.EnsureSuccessStatusCode();
                using var stream = await response.Content.ReadAsStreamAsync();
                var hashBytes = await sha256.ComputeHashAsync(stream);
                return Encoding.UTF8.GetString(hashBytes);
            }
            catch (HttpRequestException)
            {
                return ""; // May need to be null
            }
        }

        public override async Task GetSaveData(string name, string destination)
        {
            if (!File.Exists(destination))
                throw new Exception("Destination file does not exist. Cannot retrieve data."); // should be abstracted
            var remotePath = GetSavePath(name);
            try
            {
                var response = await Client.GetAsync(remotePath);
                response.EnsureSuccessStatusCode();
                using var remoteStream = await response.Content.ReadAsStreamAsync();
                using var destinationStream = File.OpenWrite(destination);
                await remoteStream.CopyToAsync(destinationStream);
            }
            catch (HttpRequestException)
            {
                throw; // May need better error handling
            }

        }

        public async override Task<string[]> SaveNames()
        {
            try
            {
                var remotePath = GetSavePath();
                var req = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = remotePath
                };
                var response = await Client.SendAsync(req);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var names = ParseNamesFromListing(content);
                return names;
            }
            catch (HttpRequestException)
            {
                return Array.Empty<string>();
            }
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
            var res = await Client.SendAsync(req);
            if (!res.IsSuccessStatusCode)
            {
                throw new Exception("Failed to upload save data to the remote sever");
            }

        }

        private async Task Setup()
        {
            var savePath = GetSavePath();
            var headRequest = new HttpRequestMessage(HttpMethod.Head, savePath);
            var headResponse = await Client.SendAsync(headRequest);

            if (!headResponse.IsSuccessStatusCode)
            {
                var mkcolRequest = new HttpRequestMessage(new HttpMethod("MKCOL"), savePath);
                await Client.SendAsync(mkcolRequest); // TODO: May need handling
            }
        }

        private Uri GetSavePath()
        {
            return new Uri($"{Uri}/NimbusApp/Saves/");
        }

        private Uri GetSavePath(string name)
        {
            return new Uri($"{Uri}/NimbusApp/Saves/{name}");
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
