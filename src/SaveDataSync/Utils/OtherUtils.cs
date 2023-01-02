using System;
using System.Diagnostics;
using System.Reflection;

namespace SaveDataSync.Utils
{
    public static class OtherUtils
    {
        public static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        public static bool IsTesting()
        {
            foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies())
            {

                if (assem.FullName.ToLowerInvariant().StartsWith("xunit"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}