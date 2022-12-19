using SaveDataSync.UI;
using SaveDataSync.Utils;
using System.Threading.Tasks;

namespace SaveDataSync
{
    public class SaveDataSyncEngine
    {
        public static SaveDataSyncEngine Instance { get; } = new SaveDataSyncEngine();

        private DataManager DataManager { get; set; }

        private LocalSaves _localsaves;
        private IServer _server;
        private Settings _settings;

        // TODO: This stuff doesn't use await. No clue how to fix...
        public LocalSaves LocalSaves
        {
            get => _localsaves;
            set
            {
                _localsaves = value;
                DataManager.SaveLocalSaves(_localsaves);
            }
        }

        public IServer Server
        {
            get => _server;
            set
            {
                _server = value;
                DataManager.SaveServerData(_server);
            }
        }

        public Settings Settings
        {
            get => _settings;
            set
            {
                _settings = value;
                DataManager.SaveSettings(_settings);
            }
        }

        public static SaveDataSyncEngine Start()
        {
            return Start(Locations.DataDirectory);
        }

        public static SaveDataSyncEngine Start(string dataLocation)
        {
            Instance.DataManager = new DataManager(dataLocation);

            Instance.LocalSaves = Instance.DataManager.GetLocalSaves();
            Instance.Server = Instance.DataManager.GetServerData();
            Instance.Settings = Instance.DataManager.GetSettings();

            return Instance;
        }

        public SaveManager GetSaveManager()
        {
            return new SaveManager(_server, _localsaves);
        }

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

        public async Task SaveAllData()
        {
            await DataManager.SaveAll(_localsaves, _server, _settings);
        }
    }
}