using System;
using System.Linq;
using System.IO;

namespace SaveDataSync
{
    internal class DataManagement
    {
        public static LocalSaveList GetLocalSaveList()
        {
            return GetLocalSaveList(Locations.DataDirectory());
        }
        public static LocalSaveList GetLocalSaveList(string location)
        {
            if (!Directory.Exists(location) || !File.Exists(Path.Combine(location, "local_saves.json")))
                return new LocalSaveList();
            using (FileStream localSaveStream = File.Open(Path.Combine(location, "local_saves.json"), FileMode.Open))
            {
                using (StreamReader sr = new StreamReader(localSaveStream))
                {
                    LocalSaveList localSaveList = LocalSaveList.FromJson(sr.ReadToEnd());
                    return localSaveList;
                }
            }
        }

        public static void SaveLocalData(LocalSaveList localSaveList)
        {
            SaveLocalData(Locations.DataDirectory(), localSaveList);
        }

        public static void SaveLocalData(string location, LocalSaveList localSaveList)
        {
            // Check to see if the location exists, otherwise create it
            if (!Directory.Exists(location)) Directory.CreateDirectory(location);

            using (FileStream localSaveStream = File.Open(Path.Combine(location, "local_saves.json"), FileMode.OpenOrCreate))
            {
                using (StreamWriter localSaveStreamWriter = new StreamWriter(localSaveStream))
                {
                    localSaveStreamWriter.Write(localSaveList.ToJson());
                }
            }
        }
    }
}
