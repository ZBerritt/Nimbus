using Newtonsoft.Json.Linq;
using SaveDataSync.UI;
using SaveDataSync.Utils;
using System;
using System.IO;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SaveDataSync
{
    /// <summary>
    /// Singleton class for connecting UI to app logic
    /// </summary>
    [SupportedOSPlatform("windows7.0")]
    public class SaveDataSyncEngine
    {
        public static SaveDataSyncEngine Instance { get; private set; }

        public string DataFile { get; init; }

        private LocalSaveList _localSaveList;
        private Server _server;
        private Settings _settings;

        // TODO: This stuff doesn't use await. No clue how to fix...
        public LocalSaveList LocalSaveList
        {
            get => _localSaveList;
        }

        public Server Server
        {
            get => _server;
        }

        public Settings Settings
        {
            get => _settings;
        }

        // Asynchronous setters
        public async Task SetLocalSaveList(LocalSaveList saveList)
        {
            _localSaveList = saveList;
            await Save();
        }

        public async Task SetServer(Server server)
        {
            _server = server;
            await Save();
        }

        public async Task SetSettings(Settings settings)
        {
            _settings = settings;
            await Save();
        }

        private SaveDataSyncEngine(string dataFile) {
            DataFile = dataFile;
        }

        /// <summary>
        /// Starts the instance of the engine using the default data directory
        /// </summary>
        /// <returns>The single instance of SaveDataSyncEngine</returns>
        public async static Task<SaveDataSyncEngine> Start()
        {
            if (Instance != null) return Instance;
            return await Start(DefaultLocations.DataFile);
        }

        /// <summary>
        /// Starts the instance of the engine
        /// </summary>
        /// <param name="dataLocation">The directory to store all app data</param>
        /// <returns>The single instance of SaveDataSyncEngine</returns>
        public async static Task<SaveDataSyncEngine> Start(string dataFile)
        {
            if (Instance != null) return Instance;
            Instance = new SaveDataSyncEngine(dataFile);
            await Instance.Load();

            return Instance;
        }

        /// <summary>
        /// Gets the save manager used to manage save game data
        /// </summary>
        /// <returns>The instance save manager</returns>
        public SaveManager GetSaveManager()
        {
            return new SaveManager(_server, _localSaveList);
        }

        /// <summary>
        /// Adds a save to the save list and saves the new data
        /// </summary>
        /// <param name="name">The name of the game save</param>
        /// <param name="location">The file/folder location</param>
        /// <returns>Task representing asynchronous operation</returns>
        public async Task AddSave(string name, string location)
        {
            GetSaveManager().AddSave(name, location);
            await Save();
        }

        public async Task<string[]> ExportSaves(string[] saves, ProgressBarControl progress)
        {
            return await GetSaveManager().ExportSaves(saves, progress);
        }

        public async Task<string[]> ImportSaves(string[] saves, ProgressBarControl progress)
        {
            return await GetSaveManager().ImportSaves(saves, progress);
        }

        /// <summary>
        /// Gets the local save hash for comparing files
        /// </summary>
        /// <param name="save">The name of the save game</param>
        /// <returns>An asynchronous task resulting in the local save hash</returns>
        public async Task<string> GetLocalHash(string save)
        {
            using var tmpFile = new FileUtils.TemporaryFile();
            await LocalSaveList.ArchiveSaveData(save, tmpFile.FilePath);
            var saveHash = Server.GetLocalSaveHash(tmpFile.FilePath);
            return await saveHash;
        }

        public async Task<string> GetRemoteHash(string save)
        {
            return await Server.GetRemoteSaveHash(save);
        }

        /// <summary>
        /// Saves settings, local saves, and server data to storage
        /// </summary>
        /// <returns>Task representing asynchronous operation</returns>
        public async Task Save()
        {
            CreateDataFile();

            // Serialize the data as JSON
            var json = new JObject();
            if (_localSaveList != null)
            {
                json.Add("save_list", _localSaveList.Serialize());
            }
            if (_settings != null)
            {
                json.Add("settings", _settings.Serialize());
            }
            if (_server != null)
            {
                json.Add("server", await _server.Serialize());
            }

            // Encrypt and write data 
            var jsonAsBytes = Encoding.ASCII.GetBytes(json.ToString());
            using var fsStream = File.Open(DataFile, FileMode.OpenOrCreate);
            var encData = ProtectedData.Protect(jsonAsBytes, null, DataProtectionScope.CurrentUser);
            await fsStream.WriteAsync(encData);
        }

        /// <summary>
        /// Loads settings, local saves, and server data to engine
        /// </summary>
        /// <returns>Task representing asynchronous operation</returns>
        public async Task Load()
        {
            try
            {
                if (!File.Exists(DataFile))
                {
                    await Reset();
                    return;
                }

                var encBytes = await File.ReadAllBytesAsync(DataFile); // Might change...
                if (encBytes.Length == 0)
                {
                    await Reset();
                    return;
                }
                
                var rawBytes = ProtectedData.Unprotect(encBytes, null, DataProtectionScope.CurrentUser);
                var rawString = Encoding.ASCII.GetString(rawBytes);
                var json = JObject.Parse(rawString);
                _localSaveList = json.ContainsKey("save_list")
                    ? LocalSaveList.Deserialize(json.GetValue("save_list").ToObject<string>())
                    : new LocalSaveList();
                _settings = json.ContainsKey("settings")
                    ? Settings.Deseriaize(json.GetValue("settings").ToObject<JObject>().ToString())
                    : new Settings();
                // Reset if null
                _localSaveList ??= new LocalSaveList();
                _settings ??= new Settings();

                var hasServer = json.TryGetValue("server", out var serverJson);
                if (!hasServer || serverJson == null)
                {
                    _server = null; // No server was found
                    return;
                }
                var serverJsonObject = serverJson.ToObject<JObject>();
                var hasType = serverJsonObject.TryGetValue("type", out var serverType);
                if (!hasType) return;

                var server = Server.GetServerFromType(serverType.ToObject<string>());
                if (server == null) return;

                var hasData = serverJsonObject.TryGetValue("data", out var serverData);
                if (hasData)
                {
                    await server.DeserializeData(serverData.ToObject<JObject>());
                }
                _server = server;
            } catch (Exception ex)
            {
                if (OtherUtils.IsTesting()) return;
#if DEBUG
                var res = PopupDialog.ErrorPrompt("Data is corrupted or out of date. Would you like to reset it?\n" + ex.Message);
#else
                var res = PopupDialog.ErrorPrompt("Data is corrupted or out of date. Would you like to reset it?");
#endif
                if (res == System.Windows.Forms.DialogResult.Yes)
                {
                    await Reset();
                    return;
                }
            }
        }

        /// <summary>
        /// Resets all data to default and saves to storage
        /// </summary>
        /// <returns>Task representing asynchronous operation</returns>
        private async Task Reset()
        {
            _localSaveList = new LocalSaveList();
            _server = null;
            _settings = new Settings();
            await Save();
        }

        private void CreateDataFile()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DataFile));
            using var _ = File.Open(DataFile, FileMode.OpenOrCreate);           
        }
    }
}