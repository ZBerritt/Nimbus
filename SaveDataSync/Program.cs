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
            string json = @"{
            'saves': [
                { 'name': 'pee', 'location': 'poo' },
                { 'name': '123', 'location': '456' }
        ]
    }";
            LocalSaveList list = LocalSaveList.FromJson(json);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
