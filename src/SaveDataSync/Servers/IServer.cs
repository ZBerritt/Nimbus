using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace SaveDataSync
{
    /// <summary>
    /// Represents a remote data server used for importing and exporting saves
    /// </summary>
    public interface IServer

    {
        // The name/type of host
        public string Name { get; }

        // The server being hosted on (may vary if selfhosted)
        public string Host { get; }

        // An array of all saves on the server
        public Task<string[]> SaveNames();

        // Gets the specificed save data
        public Task GetSaveData(string name, string destination);

        // Uploads local save data
        public Task UploadSaveData(string name, string source);

        // Is the server online?
        public Task<bool> ServerOnline();

        // Get remote save hash
        public Task<string> GetRemoteSaveHash(string name);

        // Get local save has to compare (different servers may have different methods)
        public Task<string> GetLocalSaveHash(string archiveLocation);

        // Server data in JSON format
        public Task<JObject> Serialize();
    }
}