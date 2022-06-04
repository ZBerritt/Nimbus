using Newtonsoft.Json.Linq;

namespace SaveDataSync
{
    // TODO: handle import/export errors properly
    public interface IServer

    {
        // The name/type of host
        public string Name { get; }

        // The server being hosted on (may vary if selfhosted)
        public string Host { get; }

        // An array of all saves on the server
        public string[] SaveNames();

        // Gets the specificed save data
        public byte[] GetSaveData(string name);

        // Uploads local save data
        public void UploadSaveData(string name, byte[] data);

        // Is the server online?
        public bool ServerOnline();

        // Get remote save hash
        public string GetRemoteSaveHash(string name);

        // Get local save has to compare (different servers may have different methods)
        public string GetLocalSaveHash(byte[] data);

        // Server data in JSON format
        public JObject ToJson();
    }
}