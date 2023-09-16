using NimbusApp.Controllers;
using NimbusApp.Settings;
using System;
using System.Windows.Forms;

namespace NimbusApp.UI
{
    internal partial class SettingsWindow : Form
    {
        private readonly NimbusAppEngine engine;
        private AppSettings settingsCopy;
        public bool ShouldReload { get; private set; } = false;

        public SettingsWindow(NimbusAppEngine engine)
        {
            this.engine = engine;
            AppSettings currentSettings = engine.Settings;
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
            engine.Settings = settingsCopy;
            await engine.Serialize();
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