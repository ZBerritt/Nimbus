using System;
using System.IO;

namespace NimbusApp.Utils
{
    internal class DefaultLocations
    {
        public static string Assets = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString(), "Assets");
#if DEBUG

        public static string DataFile { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NimbusApp-DEBUG", "Data.dat");
#else
        public static string DataFile { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NimbusApp", "Data.dat");
#endif
    }
}