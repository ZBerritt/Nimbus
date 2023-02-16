using System;
using System.IO;

namespace SaveDataSync.Utils
{
    internal class DefaultLocations
    {
        public static string Assets = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString(), "Assets");
#if DEBUG

        public static string DataFile { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveDataSync-DEBUG", "Data.dat");
#else
        public static string DataFile { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveDataSync", "Data.dat");
#endif
    }
}