using Newtonsoft.Json.Linq;
using SaveDataSync.Servers;
using System.Threading.Tasks;

namespace SaveDataSync
{
    /// <summary>
    /// Represents a remote data server used for importing and exporting saves
    /// </summary>
    public abstract class Server

    {
        // The name/type of host
        public abstract string Name { get; }

        // The server being hosted on (may vary if selfhosted)
        public abstract string Host { get; }

        // An array of all saves on the server
        public abstract Task<string[]> SaveNames();

        // Gets the specificed save data
        public abstract Task GetSaveData(string name, string destination);

        // Uploads local save data
        public abstract Task UploadSaveData(string name, string source);

        // Is the server online?
        public abstract Task<bool> ServerOnline();

        // Get remote save hash
        public abstract Task<string> GetRemoteSaveHash(string name);

        // Get local save has to compare (different servers may have different methods)
        public abstract Task<string> GetLocalSaveHash(string archiveLocation);

        // Server data in JSON format
        public abstract Task<JObject> Serialize();

        // JSON to server data
        public abstract Task Deserialize(JObject json);

        // Builds a new instance of the server and stores its data
        public abstract Task Build();

        // Gets an empty class given the server type
        public static Server GetServerFromType(string type)
        {
            return type switch
            {
                "Dropbox" => new DropboxServer(),
                _ => null,
            } ;
        }
    }
}