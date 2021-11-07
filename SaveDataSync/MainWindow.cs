using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SaveDataSync
{
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
            engine = SaveDataSyncEngine.CreateInstance();
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
                long saveSize = (attr & FileAttributes.Directory) == FileAttributes.Directory
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
        private void newSaveFile_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Create Save File");
            engine.CreateSaveFile();
        }

        private void export_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Export");
            engine.ExportSaveData();
        }

        private void import_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Import");
            engine.ImportSaveData();
        }

        private void settings_Click(object sender, EventArgs e)
        {

        }

        private void saveFileList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
