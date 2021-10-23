using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveDataSync
{
    internal abstract class Server
    {
        public abstract string[] SaveNames();

        public abstract byte[] GetSaveData(string name);

        public abstract void UploadSaveData(string name, byte[] data);

        public abstract bool ServerOnline();
    }
}
