using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NimbusApp.Utils
{
    internal class DropboxUtils
    {
        public static async Task HandleOAuth2Redirect(HttpListener http, Uri RedirectUri)
        {
            var context = await http.GetContextAsync();

            while (context.Request.Url.AbsolutePath != RedirectUri.AbsolutePath)
            {
                context = await http.GetContextAsync();
            }

            context.Response.ContentType = "text/html";

            var htmlPath = Path.Combine(DefaultLocations.Assets, "oauth.html");
            using var file = File.Open(htmlPath, FileMode.Open, FileAccess.Read, FileShare.Read);
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
