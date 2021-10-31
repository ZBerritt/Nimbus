namespace SaveDataSync
{
    internal abstract class Server
    {

        public Server()
        {

        }
        public abstract string[] SaveNames();

        public abstract byte[] GetSaveData(string name);

        public abstract void UploadSaveData(string name, byte[] data);

        public abstract bool ServerOnline();
    }
}
