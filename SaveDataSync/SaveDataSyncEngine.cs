using System;

namespace SaveDataSync
{
    internal class SaveDataSyncEngine
    {

        private LocalSaveList localSaveList;
        private Server server;

        public static SaveDataSyncEngine CreateInstance()
        {
            return new SaveDataSyncEngine();
        }

        public SaveDataSyncEngine()
        {
            localSaveList = DataManagement.GetLocalSaveList();
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
    }
}
