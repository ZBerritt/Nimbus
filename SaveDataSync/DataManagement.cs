using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaveDataSync.Servers;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SaveDataSync
{
    public class DataManagement
    {
        /* Server Management */

        public static LocalSaveList GetLocalSaveList()
        {
            return GetLocalSaveList(Locations.DataDirectory());
        }

        public static LocalSaveList GetLocalSaveList(string location)
        {
            if (!Directory.Exists(location) || !File.Exists(Path.Combine(location, "local_saves.json")))
                return new LocalSaveList();
            using (FileStream localSaveStream = File.Open(Path.Combine(location, "local_saves.json"), FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(localSaveStream))
                {
                    LocalSaveList localSaveList = LocalSaveList.FromJson(sr.ReadToEnd());
                    return localSaveList;
                }
            }
        }

        public static void SaveLocalSaveList(LocalSaveList localSaveList)
        {
            SaveLocalSaveList(Locations.DataDirectory(), localSaveList);
        }

        public static void SaveLocalSaveList(string location, LocalSaveList localSaveList)
        {
            // Check to see if the location exists, otherwise create it
            if (!Directory.Exists(location)) Directory.CreateDirectory(location);

            File.WriteAllText(Path.Combine(location, "local_saves.json"), localSaveList.ToJson());
        }

        public static void SaveServerData(Server server)
        {
            SaveServerData(Locations.DataDirectory(), server);
        }

        public static void SaveServerData(string location, Server server)
        {
            if (server == null) return;
            var json = new JObject();
            var serverDataJson = server.ToJson();
            var serverName = server.Name();
            json.Add("name", serverName);
            json.Add("data", serverDataJson);
            var result = json.ToString();

            // Protect the sensitive data
            var encData = ProtectedData.Protect(Encoding.UTF8.GetBytes(result), null, DataProtectionScope.CurrentUser);

            // Store the sensitive data
            using (FileStream localSaveStream = File.Open(Path.Combine(location, "server_data.dat"), FileMode.OpenOrCreate))
            {
                localSaveStream.Write(encData, 0, encData.Length);
            }
        }

        public static Server GetServerData()
        {
            return GetServerData(Locations.DataDirectory());
        }

        public static Server GetServerData(string location)
        {
            if (!Directory.Exists(location) || !File.Exists(Path.Combine(location, "server_data.dat")))
                return null;
            using (FileStream localSaveStream = File.Open(Path.Combine(location, "server_data.dat"), FileMode.Open))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    localSaveStream.CopyTo(ms);
                    var encData = ms.ToArray();
                    var rawBytes = ProtectedData.Unprotect(encData, null, DataProtectionScope.CurrentUser);
                    var rawText = Encoding.UTF8.GetString(rawBytes);

                    JObject deserializedJson = JsonConvert.DeserializeObject<JObject>(rawText);
                    string serverName = (string)deserializedJson.GetValue("name");
                    JObject serverData = (JObject)deserializedJson.GetValue("data");

                    // The only hardcoded instance where abstract server data cannot work. Methods to be implemented manually
                    switch (serverName)
                    {
                        case "Dropbox":
                            return DropboxServer.BuildFromJson(serverData);

                        default:
                            return null;
                    }
                }
            }
        }

        /* Settings Management */

        public static void SaveSettings(Settings settings)
        {
            SaveSettings(Locations.DataDirectory(), settings);
        }

        public static void SaveSettings(string location, Settings settings)
        {
            var json = settings.ToJSON();
            File.WriteAllText(Path.Combine(location, "settings.json"), json.ToString());
        }

        public static Settings GetSettings()
        {
            return GetSettings(Locations.DataDirectory());
        }

        public static Settings GetSettings(string location)
        {
            if (!Directory.Exists(location) || !File.Exists(Path.Combine(location, "settings.json")))
            {
                var settings = new Settings();
                SaveSettings(location, settings);
                return settings;
            }

            using (FileStream localSettingsStream = File.Open(Path.Combine(location, "settings.json"), FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(localSettingsStream))
                {
                    var content = sr.ReadToEnd();
                    JObject settingsData = JObject.Parse(content);
                    var settings = new Settings(settingsData);
                    return settings;
                }
            }
        }
    }
}