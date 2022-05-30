using System;

namespace SaveDataSync
{
    internal class SaveDataSyncEngine
    {
        private static SaveDataSyncEngine instance;
        private LocalSaveList localSaveList;
        private Server server;
        private Settings settings;

        public static SaveDataSyncEngine CreateInstance()
        {
            if (instance != null) throw new Exception("Engine instance already exists!");
            instance = new SaveDataSyncEngine();
            return instance;
        }

        private SaveDataSyncEngine()
        {
            localSaveList = DataManagement.GetLocalSaveList();
            server = DataManagement.GetServerData();
            settings = DataManagement.GetSettings();
        }

        public LocalSaveList GetLocalSaveList()
        {
            return localSaveList;
        }
        
        public Settings GetSettings()
        {
            return settings;
        }

        public void SetSettings(Settings settings)
        {
            this.settings = settings;
            Save();
        }

        public Server GetServer()
        {
            return server;
        }

        public void SetServer(Server server)
        {
            this.server = server;
            Save();
        }

        // Button actions
        public void CreateSaveFile(string name, string location)
        {
            localSaveList.AddSave(name, location);
            Save();
        }

        public void ExportSaveData()
        {

        }

        public void ImportSaveData()
        {

        }

        public void Save()
        {
            DataManagement.SaveLocalSaveList(localSaveList);
            DataManagement.SaveServerData(server);
            DataManagement.SaveSettings(settings);
        }
    }
}
