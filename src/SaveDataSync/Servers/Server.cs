using Newtonsoft.Json;
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
        /// <summary>
        /// The display name of the server
        /// </summary>
        public abstract string Type { get; }

        /// <summary>
        /// The base host URI of the server for display purposes
        /// </summary>
        public abstract string Host { get; }

        /// <summary>
        /// Gets a list of all remote save files
        /// </summary>
        /// <returns>An asynchronous task resulting in the list of remote save names</returns>
        public abstract Task<string[]> SaveNames();

        /// <summary>
        /// Gets the remote save data from the server
        /// </summary>
        /// <param name="name">The name of the save</param>
        /// <param name="destination">The destination to store the save data</param>
        /// <returns>Task representing asynchronous operation</returns>
        public abstract Task GetSaveData(string name, string destination);

        /// <summary>
        /// Uploads save data from local storage to remote server
        /// </summary>
        /// <param name="name">The name of the save</param>
        /// <param name="source">The source directory of the data to upload</param>
        /// <returns>Task representing asynchronous operation</returns>
        public abstract Task UploadSaveData(string name, string source);

        /// <summary>
        /// Checks the server status to determine if the server is online
        /// </summary>
        /// <returns>An asynchronous task resulting in a boolean that represents if the server is online</returns>
        public abstract Task<bool> GetOnlineStatus();

        /// <summary>
        /// Gets the save hash from the remote server
        /// </summary>
        /// <param name="name">The name of the save</param>
        /// <returns>An asynchronous task resulting in the save hash of the remote save</returns>
        public abstract Task<string> GetRemoteSaveHash(string name);

        /// <summary>
        /// Gets the local representation of the save hash 
        /// Different servers may implement their remote hashes differently, this function must be identical
        /// </summary>
        /// <param name="archiveLocation">Location of the ZIP archive to hash</param>
        /// <returns>An asynchronous task resulting in the save hash of the local file</returns>
        public abstract Task<string> GetLocalSaveHash(string archiveLocation);

        /// <summary>
        /// Serializes the server data to JSON
        /// </summary>
        /// <returns>A JSON object representing the server data</returns>
        public abstract Task<JObject> SerializeData();

        /// <summary>
        /// Deserializes the server from a JSON object
        /// </summary>
        /// <param name="json">The JSON object to deserialize</param>
        /// <returns>Task representing asynchronous operation</returns>
        public abstract Task DeserializeData(JObject data);

        /// <summary>
        /// Builds a new server object from an empty instance
        /// </summary>
        /// <returns>Task representing asynchronous operation</returns>
        public abstract Task Build();

        /* Static methods */

        /// <summary>
        /// Gets an empty server object given the server type
        /// </summary>
        /// <param name="type">The server type (Server.Name)</param>
        /// <returns>An empty Server object of the specified type</returns>
        public static Server GetServerFromType(string type)
        {
            return type switch
            {
                "Dropbox" => new DropboxServer(),
                _ => null,
            };
        }

        public async Task<string> Serialize()
        {
            var data = await SerializeData();
            var json = new JObject
            {
                { "type", Type },
                { "data", data }
            };

            return json.ToString();
        }

        public static async Task<Server> Deserialize(string input)
        {
            JObject serverJson;
            try
            {
                serverJson = JObject.Parse(input);
            }
            catch (JsonReaderException)
            {
                return null; // Cannot parse server data, return
            }
            var hasServerType = serverJson.TryGetValue("type", out var serverType);
            var hasServerData = serverJson.TryGetValue("data", out var serverData);
            if (!hasServerData || !hasServerType)
            {
                return null; // Invalid server object
            }

            Server server = GetServerFromType(serverType?.ToString());
            if (server == null)
            {
                return null; // Invalid type
            }

            var data = serverData.ToObject<JObject>();
            await server.DeserializeData(data);

            return server;
        }
    }
}