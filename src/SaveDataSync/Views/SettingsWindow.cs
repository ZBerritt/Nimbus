using SaveDataSync.Controllers;
using SaveDataSync.Models;
using System;
using System.Windows.Forms;

namespace SaveDataSync.UI
{
    internal partial class SettingsWindow : Form
    {
        private readonly SaveDataSyncEngine engine;
        private Settings settingsCopy;
        public bool ShouldReload { get; private set; } = false;

        public SettingsWindow(SaveDataSyncEngine engine)
        {
            this.engine = engine;
            Settings currentSettings = engine.Settings;
            settingsCopy = currentSettings.Clone(); // Clone all data to new instance

            // Initialize
            InitializeComponent();

            // Populate
            switch (settingsCopy.Theme)
            {
                case THEME.LIGHT:
                    themeSelect.SelectedIndex = 0;
                    break;
                case THEME.DARK: 
                    themeSelect.SelectedIndex = 1; 
                    break;
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            // Get the new settings from the UI here
            await engine.SetSettings(settingsCopy);
            await engine.Save();
            ShouldReload = true;
            Close();
        }

        private void themeSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (themeSelect.SelectedIndex)
            {
                case 0:
                    settingsCopy.Theme = THEME.LIGHT;
                    break;
                case 1:
                    settingsCopy.Theme = THEME.DARK;
                    break;
            }
        }
    }
}