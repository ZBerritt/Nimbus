using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SaveDataSync.Models;
using SaveDataSync.Models.Servers;
using SaveDataSync.UI;
using SaveDataSync.Utils;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SaveDataSync.Controllers
{
    /// <summary>
    /// Singleton class for connecting UI to app logic
    /// </summary>
    public class SaveDataSyncEngine
    {
        public string DataFile { get; init; }

        private LocalSaveList _localSaveList;
        private Server _server;
        private Settings _settings;

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

        private SaveDataSyncEngine(string dataFile)
        {
            DataFile = dataFile;
        }

        /// <summary>
        /// Starts an instance of the engine using the default data directory
        /// </summary>
        /// <returns>The instance of the SaveDataSyncEngine</returns>
        public async static Task<SaveDataSyncEngine> Start()
        {
            return await Start(DefaultLocations.DataFile);
        }

        /// <summary>
        /// Starts an instance of the engine
        /// </summary>
        /// <param name="dataLocation">The directory to store all app data</param>
        /// <returns>The instance of the SaveDataSyncEngine</returns>
        public async static Task<SaveDataSyncEngine> Start(string dataFile)
        {
            var engine = new SaveDataSyncEngine(dataFile);
            try
            {
                await engine.Load();
            }
            catch (LoadException ex)
            {
#if DEBUG
                var res = PopupDialog.ErrorPrompt("Data is corrupted or out of date. Would you like to reset it?\n" + ex.Message);
#else
                var res = PopupDialog.ErrorPrompt("Data is corrupted or out of date. Would you like to reset it?");
#endif
                if (res == System.Windows.Forms.DialogResult.Yes)
                {
                    engine.Reset();
                }
            }

            return engine;
        }

        /// <summary>
        /// Gets the save manager used to manage save game data
        /// </summary>
        /// <returns>The instance save manager</returns>
        public SaveManager GetSaveManager()
        {
            return new SaveManager(this);
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

        public async Task<string[]> ExportSaves(string[] saves, ProgressBarControl progress) =>
            await GetSaveManager().ExportSaves(saves, progress);

        public async Task<string[]> ImportSaves(string[] saves, ProgressBarControl progress) =>
            await GetSaveManager().ImportSaves(saves, progress);

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

        public async Task<string> GetRemoteHash(string save) => await Server.GetRemoteSaveHash(save);

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
            if (!File.Exists(DataFile))
            {
                CreateDataFile();
                Reset();
                return;
            }

            var encBytes = await File.ReadAllBytesAsync(DataFile);
            if (encBytes.Length == 0)
            {
                Reset();
                return;
            }
            var rawBytes = ProtectedData.Unprotect(encBytes, null, DataProtectionScope.CurrentUser);
            var rawString = Encoding.ASCII.GetString(rawBytes);
            JObject json;
            try
            {
                json = JObject.Parse(rawString);
            }
            catch (JsonReaderException)
            {
                throw new LoadException("Could not parse JSON from data file.");
            }

            // Reset everything before in case something is being removed
            Reset();

            // Save List
            if (json.ContainsKey("save_list"))
            {
                var saveListData = json.GetValue("save_list").ToString();
                var list = LocalSaveList.Deserialize(saveListData);
                _localSaveList = list;
            }

            // Settings
            if (json.ContainsKey("settings"))
            {
                var settingsData = json.GetValue("settings").ToString();
                var settings = Settings.Deseriaize(settingsData);
                _settings = settings;
            }

            // Server
            if (json.ContainsKey("server"))
            {
                var serverJsonString = json.GetValue("server").ToString();
                var server = await Server.Deserialize(serverJsonString);
                _server = server;
            }
        }

        /// <summary>
        /// Resets all data to default and saves to storage
        /// </summary>
        /// <returns>Task representing asynchronous operation</returns>
        private void Reset()
        {
            _localSaveList = new LocalSaveList();
            _server = null;
            _settings = new Settings();
        }

        private void CreateDataFile()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DataFile));
            using var _ = File.Open(DataFile, FileMode.OpenOrCreate);
        }
    }
}