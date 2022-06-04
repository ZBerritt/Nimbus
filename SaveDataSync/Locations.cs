using System;
using System.IO;

namespace SaveDataSync
{
    internal class Locations
    {
        public static string DataDirectory { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SaveDataSync");
    }
}