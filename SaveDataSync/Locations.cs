using System;
using System.IO;

namespace SaveDataSync
{
    internal class Locations
    {
        public static string Assets = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString(), "Assets");
#if DEBUG

        public static string DataDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveDataSync-DEBUG");
#else
        public static string DataDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveDataSync");
#endif
    }
}