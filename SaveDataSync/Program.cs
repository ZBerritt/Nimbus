using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            LocalSaveList list = new LocalSaveList();
            list.AddSave("test", "C:\\Users\\nitro\\Desktop\\test");
            list.AddSave("test2", "C:\\Users\\nitro\\Desktop\\test2");
            byte[] data = list.GetSaveZipData("test");
            list.WriteData("test2", data);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
