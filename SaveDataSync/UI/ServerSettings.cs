using SaveDataSync.Servers;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SaveDataSync.UI
{
    internal partial class ServerSettings : Form
    {
        public SaveDataSyncEngine engine;
        public Server server;

        private string dropboxVerifier;
        public ServerSettings(SaveDataSyncEngine engine)
        {
            this.engine = engine;
            this.server = engine.GetServer();
            InitializeComponent();
            if (server != null)
            {
                var serverName = server.Name();
                switch (serverName)
                {
                    case "Dropbox":
                        var apiKey = ((DropboxServer)server).GetServerApiKey();
                        if (apiKey != null) dropboxApiKey.Text = apiKey;
                        break;
                }
            }
            
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                switch (settingsTabs.SelectedTab.AccessibleName)
                {
                    case "dropbox":
                        var key = dropboxApiKey.Text;
                        var newServer = DropboxServer.Build(key, dropboxVerifier);
                        if (newServer == null)
                        {
                            MessageBox.Show("An erorr occured building the dropbox server. " +
                                "Please try getting a new API key. " +
                                "If this persists, please submit an issue on GitHub.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        var serverOnline = newServer.ServerOnline();
                        if (!serverOnline) throw new Exception("Server is not online!");
                        engine.SetServer(newServer);
                        break;

                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void loginWithDropboxButton_Click(object sender, EventArgs e)
        {
            if (dropboxVerifier == null) dropboxVerifier = DropboxServer.GenerateVerifier();
            string url = "https://www.dropbox.com/oauth2/authorize" +
                        "?response_type=code&token_access_type=offline" +
                        "&redirect_uri=http://localhost:1235" +
                        "&client_id=" + DropboxServer.APP_ID +
                        "&code_challenge=" + DropboxServer.GenerateCodeChallenge(dropboxVerifier) +
                        "&code_challenge_method=S256";
            Process.Start(url);
            string key = DropboxServer.GetApiKey();
            if (key != null) dropboxApiKey.Text = key;
        }
    }
}
