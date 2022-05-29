using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SaveDataSync.UI
{
    internal partial class SettingsWindow : Form
    {
        private SaveDataSyncEngine engine;
        private Settings settingsCopy;
        public SettingsWindow(SaveDataSyncEngine engine)
        {
            this.engine = engine;
            this.settingsCopy = engine.GetSettings().Clone();
            InitializeComponent();
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            // Get the new settings from the UI here
            engine.SetSettings(settingsCopy);
            engine.Save();
            Close();
        }
    }
}
