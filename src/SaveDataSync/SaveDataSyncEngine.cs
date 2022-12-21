using SaveDataSync.UI;
using SaveDataSync.Utils;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace SaveDataSync
{
    /// <summary>
    /// Singleton class for connecting UI to app logic
    /// </summary>
    [SupportedOSPlatform("windows7.0")]
    public class SaveDataSyncEngine
    {
        public static SaveDataSyncEngine Instance { get; } = new SaveDataSyncEngine();

        private DataManager DataManager { get; set; }

        private LocalSaveList _localsaves;
        private Server _server;
        private Settings _settings;

        // TODO: This stuff doesn't use await. No clue how to fix...
        public LocalSaveList LocalSaves
        {
            get => _localsaves;
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
            _localsaves = saveList;
            await DataManager.SaveLocalSaves(saveList);
        }

        public async Task SetServer(Server server)
        {
            _server = server;
            await DataManager.SaveServerData(server);
        }

        public async Task SetSettings(Settings settings)
        {
            _settings = settings;
            await DataManager.SaveSettings(settings);
        }

        /// <summary>
        /// Starts the instance of the engine using the default data directory
        /// </summary>
        /// <returns>The single instance of SaveDataSyncEngine</returns>
        public static async Task<SaveDataSyncEngine> Start()
        {
            return await Start(Locations.DataDirectory);
        }

        /// <summary>
        /// Starts the instance of the engine
        /// </summary>
        /// <param name="dataLocation">The directory to store all app data</param>
        /// <returns>The single instance of SaveDataSyncEngine</returns>
        public static async Task<SaveDataSyncEngine> Start(string dataLocation)
        {
            Instance.DataManager = new DataManager(dataLocation);

            await Instance.SetLocalSaveList(Instance.DataManager.GetLocalSaves());
            await Instance.SetServer(Instance.DataManager.GetServerData());
            await Instance.SetSettings(Instance.DataManager.GetSettings());

            return Instance;
        }

        /// <summary>
        /// Gets the save manager used to manage save game data
        /// </summary>
        /// <returns>The instance save manager</returns>
        public SaveManager GetSaveManager()
        {
            return new SaveManager(_server, _localsaves);
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
            await SaveAllData();
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
            await LocalSaves.ArchiveSaveData(save, tmpFile.FilePath);
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
        public async Task SaveAllData()
        {
            await DataManager.SaveAll(_localsaves, _server, _settings);
        }
    }
}