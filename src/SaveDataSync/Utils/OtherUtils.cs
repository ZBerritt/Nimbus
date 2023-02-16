using System.Diagnostics;

namespace SaveDataSync.Utils
{
    public static class OtherUtils
    {
        public static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}