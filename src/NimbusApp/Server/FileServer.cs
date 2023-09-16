using NimbusApp.Utils;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace NimbusApp.Server
{
    public class FileServer : ServerBase
    {
        public string Location { get; set; }

        public override string Type => "File System";

        public override string Host => "localhost";

        public override async Task<string> GetLocalSaveHash(string archiveLocation)
        {
            Setup();
            using var stream = File.OpenRead(archiveLocation);
            using var sha256 = SHA256.Create();
            var hashBytes = await sha256.ComputeHashAsync(stream);
            return OtherUtils.BytesToHex(hashBytes);
        }

        public override Task<bool> GetOnlineStatus()
        {
            Setup();
            return Task.FromResult(Directory.Exists(Location));
        }

        public override async Task<string> GetRemoteSaveHash(string name)
        {
            Setup();
            try
            {
                var path = GetSavePath(name);
                using var stream = File.OpenRead(path);
                using var sha256 = SHA256.Create();
                var hashBytes = await sha256.ComputeHashAsync(stream);
                return OtherUtils.BytesToHex(hashBytes);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public override async Task GetSaveData(string name, string destination)
        {
            Setup();
            try
            {
                var path = GetSavePath(name);
                if (!File.Exists(destination)) throw new Exception("Destination file does not exist. Cannot retrieve data.");
                using var remoteStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var destinationStream = File.OpenWrite(destination);
                await remoteStream.CopyToAsync(destinationStream);
            }
            catch (FileNotFoundException)
            { }
        }

        public override Task<string[]> SaveNames()
        {
            Setup();
            return Task.FromResult(Directory
                .EnumerateFiles(Location, "*", SearchOption.TopDirectoryOnly)
                .Where(s => s.EndsWith(".zip"))
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray());
        }

        public override async Task UploadSaveData(string name, string source)
        {
            Setup();
            if (!File.Exists(source)) throw new Exception("Source file does not exist. Cannot upload.");
            var path = GetSavePath(name);
            using var sourceStream = File.Open(source, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var destinationStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            await sourceStream.CopyToAsync(destinationStream);
        }

        protected override Task Build(params string[] args)
        {
            Location = args[0];
            Setup();
            return Task.CompletedTask;
        }

        private string GetSavePath(string name)
        {
            return Path.Combine(Location, $"{name}.zip");
        }

        private void Setup()
        {
            try
            {
                if (!Directory.Exists(Location))
                {
                    Directory.CreateDirectory(Location);
                }
            }
            catch (Exception) { }
        }
    }
}
