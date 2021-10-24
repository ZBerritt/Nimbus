using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        private void saveTable_Paint(object sender, PaintEventArgs e)
        {
            
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
    }
}
