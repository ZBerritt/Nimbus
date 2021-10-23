using System;

namespace SaveDataSync
{
    internal class SaveDataSyncEngine
    {

        private LocalSaveList localSaveList;

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
    }
}
