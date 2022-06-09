using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace SaveDataSync
{
    public static class Utils
    {
        public static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }

        /* String Utils - https://stackoverflow.com/questions/5617320/given-full-path-check-if-path-is-subdirectory-of-some-other-path-or-otherwise */

        public static string WithEnding(this string str, string ending)
        {
            if (str == null)
                return ending;

            string result = str;

            // Right() is 1-indexed, so include these cases
            // * Append no characters
            // * Append up to N characters, where N is ending length
            for (int i = 0; i <= ending.Length; i++)
            {
                string tmp = result + ending.Right(i);
                if (tmp.EndsWith(ending))
                    return tmp;
            }

            return result;
        }

        public static string Right(this string value, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, "Length is less than zero");
            }

            return (length < value.Length) ? value[^length..] : value;
        }

        public static async Task HandleOAuth2Redirect(HttpListener http, Uri RedirectUri)
        {
            var context = await http.GetContextAsync();

            while (context.Request.Url.AbsolutePath != RedirectUri.AbsolutePath)
            {
                context = await http.GetContextAsync();
            }

            context.Response.ContentType = "text/html";

            var htmlPath = Path.Combine(Locations.WorkingDirectory, "oauth.html");
            using var file = File.Open(htmlPath, FileMode.Open, FileAccess.Read, FileShare.Read); // TODO: Change this
            file.CopyTo(context.Response.OutputStream);
            context.Response.OutputStream.Close();
        }

        public static async Task<Uri> HandleJSCodeRequest(HttpListener http, Uri JSRedirectUri)
        {
            var context = await http.GetContextAsync();

            while (context.Request.Url.AbsolutePath != JSRedirectUri.AbsolutePath)
            {
                context = await http.GetContextAsync();
            }

            var redirectUri = new Uri(context.Request.QueryString["url_with_fragment"]);

            return redirectUri;
        }
    }
}