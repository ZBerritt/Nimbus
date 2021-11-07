using System;

namespace SaveDataSync
{
    internal class SaveDataSyncEngine
    {
        private static SaveDataSyncEngine instance;
        private LocalSaveList localSaveList;
        private Server server;

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
        }

        public LocalSaveList GetLocalSaveList()
        {
            return localSaveList;
        }

        public Server GetServer()
        {
            return server;
        }

        public void SetServer(Server server)
        {
            this.server = server;
        }

        // Button actions
        public void CreateSaveFile()
        {

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
        }
    }
}
