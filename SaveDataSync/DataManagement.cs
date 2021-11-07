﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaveDataSync.Servers;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SaveDataSync
{
    internal class DataManagement
    {
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

            using (FileStream localSaveStream = File.Open(Path.Combine(location, "local_saves.json"), FileMode.OpenOrCreate))
            {
                using (StreamWriter localSaveStreamWriter = new StreamWriter(localSaveStream))
                {
                    localSaveStreamWriter.Write(localSaveList.ToJson());
                }
            }
        }

        public static void SaveServerData(Server server)
        {
            SaveServerData(Locations.DataDirectory(), server);

        }
        public static void SaveServerData(string location, Server server)
        {
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
                        case "dropbox":
                            return DropboxServer.BuildFromJson(serverData);
                        default:
                            return null;
                    }
                }
            }
        }
    }
}
