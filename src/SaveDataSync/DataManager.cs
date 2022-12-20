using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaveDataSync.Utils;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SaveDataSync
{
    public class DataManager
    {
        public string Location { get; private set; } = Locations.DataDirectory;

        private string LocalSavesFile
        {
            get => Path.Combine(Location, "local_saves.json");
        }

        private string ServerFile
        {
            get => Path.Combine(Location, "server_data.dat");
        }

        private string SettingsFile
        {
            get => Path.Combine(Location, "settings.json");
        }

        public DataManager()
        {
            if (!Directory.Exists(Location))
                Directory.CreateDirectory(Location);
        }

        public DataManager(string location)
        {
            Location = FileUtils.Normalize(location);
            if (!Directory.Exists(location))
                Directory.CreateDirectory(location);
        }

        public async Task SaveAll(LocalSaves localSaves, Server server, Settings settings)
        {
            await SaveLocalSaves(localSaves);
            await SaveServerData(server);
            await SaveSettings(settings);
        }

        /* Local Saves */

        public LocalSaves GetLocalSaves()
        {
            if (!File.Exists(LocalSavesFile))
                return new LocalSaves();
            using var localSaveStream = File.Open(LocalSavesFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sr = new StreamReader(localSaveStream);
            return LocalSaves.Deserialize(sr.ReadToEnd());
        }

        public async Task SaveLocalSaves(LocalSaves localSaves)
        {
            await File.WriteAllTextAsync(LocalSavesFile, localSaves.Serialize());
        }

        /* Server */
        [SupportedOSPlatform("windows")]
        public Server GetServerData()
        {
            if (!File.Exists(ServerFile))
                return null;
            using var localSaveStream = File.Open(ServerFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var ms = new MemoryStream();
            localSaveStream.CopyTo(ms);
            var encData = ms.ToArray();
            var rawBytes = ProtectedData.Unprotect(encData, null, DataProtectionScope.CurrentUser);
            var rawText = Encoding.UTF8.GetString(rawBytes);

            JObject deserializedJson = JsonConvert.DeserializeObject<JObject>(rawText);
            string serverName = (string)deserializedJson.GetValue("name");
            JObject serverData = (JObject)deserializedJson.GetValue("data");

            var server = Server.GetServerFromType(serverName);
            server.Deserialize(serverData);
            return server;
        }

        [SupportedOSPlatform("windows")]
        public async Task SaveServerData(Server server)
        {
            if (server is null) return;
            var json = new JObject();
            var serverDataJson = server.Serialize();
            var serverName = server.Name;
            json.Add("name", serverName);
            json.Add("data", await serverDataJson);
            var result = json.ToString();

            // Protect sensitive data from other users
            var encData = ProtectedData.Protect(Encoding.UTF8.GetBytes(result), null, DataProtectionScope.CurrentUser);
            await File.WriteAllBytesAsync(ServerFile, encData);
        }

        /* Settings */

        public Settings GetSettings()
        {
            if (!File.Exists(SettingsFile))
                return new Settings();

            using var localSettingsStream = File.Open(SettingsFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var sr = new StreamReader(localSettingsStream);
            var content = sr.ReadToEnd();
            JObject settingsData = JObject.Parse(content);
            var settings = new Settings(settingsData);
            return settings;
        }

        public async Task SaveSettings(Settings settings)
        {
            var json = settings.Serialize();
            await File.WriteAllTextAsync(SettingsFile, json.ToString());
        }
    }
}