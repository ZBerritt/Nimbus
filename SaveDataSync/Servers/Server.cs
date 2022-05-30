using Newtonsoft.Json.Linq;

namespace SaveDataSync
{
    internal abstract class Server
    {
        public Server()
        {
        }

        // The name/type of host
        public abstract string Name();

        // The server being hosted on (may vary if selfhosted)
        public abstract string Host();

        // An array of all saves on the server
        public abstract string[] SaveNames();

        // Gets the specificed save data
        public abstract byte[] GetSaveData(string name);

        // Uploads local save data
        public abstract void UploadSaveData(string name, byte[] data);

        // Is the server online?
        public abstract bool ServerOnline();

        // Server data in JSON format
        public abstract JObject ToJson();
    }
}