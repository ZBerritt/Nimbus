using NimbusApp.Models;
using NimbusApp.Models.Servers;
using NimbusApp.UI;
using NimbusApp.Utils;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NimbusApp.Controllers
{
    /// <summary>
    /// Class for connecting the UI to the back end
    /// </summary>
    public class NimbusAppEngine
    {
        public string DataFile { get; set; }

        public LocalSaveList LocalSaveList { get; set; }

        public Server Server { get; set; }

        public Settings Settings { get; set; }

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

        [JsonConstructor]
        public NimbusAppEngine() { } // this really shouldn't be public

        private NimbusAppEngine(string dataFile)
        {
            DataFile = dataFile;
            LocalSaveList = new LocalSaveList();
            Settings = new Settings();
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
            CreateDataFile(DataFile);

            // Serialize the data as JSON
            var json = JsonSerializer.Serialize(this);

            // Encrypt and write data 
            var jsonAsBytes = Encoding.ASCII.GetBytes(json.ToString());
            using var fsStream = File.Open(DataFile, FileMode.OpenOrCreate);
            var encData = ProtectedData.Protect(jsonAsBytes, null, DataProtectionScope.CurrentUser);
            await fsStream.WriteAsync(encData);
        }

        public static async Task<NimbusAppEngine> Load()
        {
            return await Load(DefaultLocations.DataFile);
        }

        /// <summary>
        /// Attempts to load the engine given the data file
        /// </summary>
        /// <returns>The engine instance loaded</returns>
        public static async Task<NimbusAppEngine> Load(string DataFile)
        {
            if (!File.Exists(DataFile))
            {
                CreateDataFile(DataFile);
                var engine = new NimbusAppEngine(DataFile);
                await engine.Save();
                return engine;
            }

            try
            {
                var encBytes = File.ReadAllBytes(DataFile);
                if (encBytes.Length == 0)
                {
                    throw new LoadException("Data file is empty");
                }
                var rawBytes = ProtectedData.Unprotect(encBytes, null, DataProtectionScope.CurrentUser);
                var rawString = Encoding.ASCII.GetString(rawBytes);
                var engine = JsonSerializer.Deserialize<NimbusAppEngine>(rawString);
                return engine;
            } catch (LoadException ex)
            {
                var res = PopupDialog.ErrorPrompt($"Data is corrupted or out of date. Would you like to reset it?\nError: {ex.Message}");
                if (res == DialogResult.Yes)
                {
                    var engine = new NimbusAppEngine(DataFile);
                    await engine.Save();
                    return engine;
                }

                Application.Exit();
                return null; // Doesn't matter the app is closed
            }


        }

        private static void CreateDataFile(string DataFile)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DataFile));
            using var _ = File.Open(DataFile, FileMode.OpenOrCreate);
        }
    }
}