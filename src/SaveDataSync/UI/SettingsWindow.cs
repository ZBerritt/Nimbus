using System;
using System.Windows.Forms;

namespace SaveDataSync.UI
{
    internal partial class SettingsWindow : Form
    {
        private SaveDataSyncEngine engine;
        private Settings settingsCopy;
        public bool ShouldReload { get; private set; } = false;

        public SettingsWindow(SaveDataSyncEngine engine)
        {
            this.engine = engine;
            // TODO: Copy settings to clone
            Settings currentSettings = engine.Settings;
            settingsCopy = currentSettings.Clone(); // Clone all data to new instance
            InitializeComponent();
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            // Get the new settings from the UI here
            engine.Settings = settingsCopy;
            await engine.SaveAllData();
            ShouldReload = true;
            Close();
        }
    }
}