using SaveDataSync.Servers;
using System;
using System.Windows.Forms;

namespace SaveDataSync
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // little test
            string verifier = DropboxServer.GenerateVerifier();
            string target = "https://www.dropbox.com/oauth2/authorize" +
                        "?response_type=code&token_access_type=offline" +
                        "&redirect_uri=http://localhost:1235/callback" +
                        "&client_id=" + DropboxServer.APP_ID +
                        "&code_challenge=" + DropboxServer.GenerateCodeChallenge(verifier) +
                        "&code_challenge_method=S256";
            OAuthToken oauth = new OAuthToken(target);
            string token = oauth.GetCode();
            Server server = DropboxServer.Build(token, verifier);
            Console.WriteLine(server.ServerOnline());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}
