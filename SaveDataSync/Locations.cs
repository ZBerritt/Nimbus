using System;
using System.IO;

namespace SaveDataSync
{
    internal class Locations
    {
#if DEBUG
        public static string DataDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveDataSync-DEBUG");
#else
        public static string DataDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveDataSync");
#endif
    }
}