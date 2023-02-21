using System.Diagnostics;

namespace NimbusApp.Utils
{
    public static class OtherUtils
    {
        public static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }
}