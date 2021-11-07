using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;



namespace SaveDataSync
{
    /// <summary>
    /// Represents the main GUI window used in the app
    /// </summary>
    public partial class MainWindow : Form
    {

        private SaveDataSyncEngine engine;
        public MainWindow()
        {
            InitializeComponent();
        }

        // Loading Events
        private void OnLoad(object sender, EventArgs e)
        {
            // Grabs the engine which allows communication with the backend 
            engine = SaveDataSyncEngine.CreateInstance();

            // BEGIN TEST
            // engine.GetLocalSaveList().AddSave("test3", "C:\\Users\\nitro\\Desktop\\test3.txt");
            // engine.GetLocalSaveList().AddSave("test1", "C:\\Users\\nitro\\Desktop\\test");
            // engine.GetLocalSaveList().AddSave("test2", "C:\\Users\\nitro\\Desktop\\test2");
            // END TEST

            // Auto sizes the last column of the save list
            saveFileList.Columns[saveFileList.Columns.Count - 1].Width = -2;

            // Loads the save list with the imported data from the engine
            ReloadSaveList();
        }

        public void ReloadSaveList()
        {

            saveFileList.Items.Clear();
            var saves = engine.GetLocalSaveList().GetSaves();
            foreach (var save in saves)
            {
                ListViewItem saveItem = new ListViewItem(save.Key);
                saveItem.SubItems.Add(save.Value);

                // Get file size
                FileAttributes attr = File.GetAttributes(save.Value);
                long saveSize = attr.HasFlag(FileAttributes.Directory)
                    ? new DirectoryInfo(save.Value).EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(fi => fi.Length)
                    : new FileInfo(save.Value).Length; // The size of the file/folder in bytes
                string[] sizes = { "Bytes", "kB", "MB", "GB", "TB" };
                int order = 0;
                while (saveSize >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    saveSize = saveSize / 1024;
                }
                saveItem.SubItems.Add(string.Format("{0:0.##} {1}", saveSize, sizes[order]));

                // Get file sync status
                saveItem.SubItems.Add("Not implemented");

                // Add to the table
                saveFileList.Items.Add(saveItem);
            }
        }

        // Click Events
        private void NewSaveFile_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Create Save File");
            engine.CreateSaveFile();
        }

        private void Export_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Export");
            engine.ExportSaveData();
        }

        private void Import_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Import");
            engine.ImportSaveData();
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Settings");
        }

        private void SaveFileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = saveFileList.FocusedItem;
            Console.WriteLine("Selection Change: {0}", selectedItem.Text);
        }

        private void SaveFileList_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var selectedItem = saveFileList.FocusedItem;
                if (selectedItem == null) return;
                Console.WriteLine("Open context menu.");
                SaveFileContextMenu(selectedItem.Text).Show(saveFileList, new Point(e.X, e.Y));

            }
        }

        private ContextMenuStrip SaveFileContextMenu(string name)
        {
            var menu = new ContextMenuStrip();
            var goToLocation = menu.Items.Add("Open File Location");
            goToLocation.Click += (object sender2, EventArgs e2) =>
            {
                string savePath = engine.GetLocalSaveList().GetSavePath(name);
                Process.Start("explorer.exe", string.Format("/select, \"{0}\"", savePath));
            };

            var quickExport = menu.Items.Add("Quick Export");
            quickExport.Click += (object sender3, EventArgs e3) =>
            {
                throw new NotImplementedException();
            };

            var quickImport = menu.Items.Add("Quick Import");
            quickExport.Click += (object sender4, EventArgs e4) =>
            {
                throw new NotImplementedException();
            };

            var removeSave = menu.Items.Add("Remove Save");
            removeSave.Click += (object sender5, EventArgs e5) =>
            {
                var confirm = MessageBox.Show("Are you sure you want to remove this save file?", 
                    "Confirm", 
                    MessageBoxButtons.YesNo, 
                    MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    engine.GetLocalSaveList().RemoveSave(name);
                    ReloadSaveList();

                }
            };
            return menu;
        }
    }
}
