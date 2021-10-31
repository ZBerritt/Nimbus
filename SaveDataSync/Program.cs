using System;
using System.Diagnostics;
using System.Windows.Forms;
using SaveDataSync.Servers;

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
            /* string verifier = DropboxServer.GenerateVerifier();
            Console.WriteLine(verifier);
            string target = "https://www.dropbox.com/oauth2/authorize" +
                        "?response_type=code&token_access_type=offline" +
                        "&redirect_uri=http://localhost:1235" +
                        "&client_id=" + DropboxServer.APP_ID +
                        "&code_challenge=" + DropboxServer.GenerateCodeChallenge(verifier) +
                        "&code_challenge_method=S256";
            Process.Start(target); */
            Server server = new DropboxServer("", "");
            Console.WriteLine(server.ServerOnline());
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}
