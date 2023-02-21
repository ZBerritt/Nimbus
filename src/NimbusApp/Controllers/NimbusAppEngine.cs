using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NimbusApp.Models;
using NimbusApp.Models.Servers;
using NimbusApp.UI;
using NimbusApp.Utils;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NimbusApp.Controllers
{
    /// <summary>
    /// Class for connecting the UI to the back end
    /// </summary>
    public class NimbusAppEngine
    {
        public string DataFile { get; init; }

        public LocalSaveList LocalSaveList { get; private set; }

        public Server Server { get; private set; }

        public Settings Settings { get; private set; }

        // Asynchronous setters
        public async Task SetLocalSaveList(LocalSaveList saveList)
        {
            LocalSaveList = saveList;
            await Save();
        }

        public async Task SetServer(Server server)
        {
            Server = server;
            await Save();
        }

        public async Task SetSettings(Settings settings)
        {
            Settings = settings;
            await Save();
        }

        private NimbusAppEngine(string dataFile)
        {
            DataFile = dataFile;
        }

        /// <summary>
        /// Starts an instance of the engine using the default data directory
        /// </summary>
        /// <returns>The instance of the NimbusAppEngine</returns>
        public async static Task<NimbusAppEngine> Start()
        {
            return await Start(DefaultLocations.DataFile);
        }

        /// <summary>
        /// Starts an instance of the engine
        /// </summary>
        /// <param name="dataLocation">The directory to store all app data</param>
        /// <returns>The instance of the NimbusAppEngine</returns>
        public async static Task<NimbusAppEngine> Start(string dataFile)
        {
            var engine = new NimbusAppEngine(dataFile);
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
            if (LocalSaveList != null)
            {
                json.Add("save_list", LocalSaveList.Serialize());
            }
            if (Settings != null)
            {
                json.Add("settings", Settings.Serialize());
            }
            if (Server != null)
            {
                json.Add("server", Server.Serialize());
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
                LocalSaveList = list;
            }

            // Settings
            if (json.ContainsKey("settings"))
            {
                var settingsData = json.GetValue("settings").ToString();
                var settings = Settings.Deseriaize(settingsData);
                Settings = settings;
            }

            // Server
            if (json.ContainsKey("server"))
            {
                var serverJsonString = json.GetValue("server").ToString();
                var server = Server.Deserialize(serverJsonString);
                Server = server;
            }
        }

        /// <summary>
        /// Resets all data to default and saves to storage
        /// </summary>
        /// <returns>Task representing asynchronous operation</returns>
        private void Reset()
        {
            LocalSaveList = new LocalSaveList();
            Server = null;
            Settings = new Settings();
        }

        private void CreateDataFile()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DataFile));
            using var _ = File.Open(DataFile, FileMode.OpenOrCreate);
        }
    }
}