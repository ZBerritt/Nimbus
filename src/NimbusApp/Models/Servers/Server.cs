using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NimbusApp.Servers;
using System;
using System.Threading.Tasks;

namespace NimbusApp.Models.Servers
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
        /// Runs the main build function for an individual server
        /// </summary>
        /// <param name="args">The respective server type's arguments</param>
        /// <returns>Task representing asynchronous operation</returns>
        protected abstract Task Build(params string[] args);

        /* Static methods */

        /// <summary>
        /// Gets a server object given of specified type with arguments
        /// </summary>
        /// <typeparam name="T">The server class needed</typeparam>
        /// <param name="args">The arguments for the build function</param>
        /// <returns>Task representing asynchronous operation which returns the server instance</returns>
        public static async Task<Server> Create<T>(params string[] args) where T : Server
        {
            var instance = Activator.CreateInstance<T>();
            await instance.Build(args);
            return instance;
        }

        /// <summary>
        /// Gets an empty server object given the server type
        /// </summary>
        /// <typeparam name="T">The server class needed</typeparam>
        /// <returns>Task representing asynchronous operation which returns the server instance</returns>
        public static Server Create<T>() where T : Server
        {
            var instance = Activator.CreateInstance<T>();
            return instance;
        }

        public static async Task<Server> CreateFromString(string name, params string[] args)
        {
            return name switch
            {
                "Dropbox" => await Create<DropboxServer>(args),
                "WebDAV" => await Create<WebDAVServer>(args),
                _ => throw new ArgumentException("Invalid server class name"),
            };
        }

        public static Server CreateFromString(string name)
        {
            return name switch
            {
                "Dropbox" => Create<DropboxServer>(),
                "WebDAV" => Create<WebDAVServer>(),
                _ => throw new ArgumentException("Invalid server class name"),
            };
        }

        /// <summary>
        /// Converts serialzed data into the form used to store
        /// </summary>
        /// <returns>A task that returns the string representation of the json object</returns>
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

        /// <summary>
        /// Converts serialized data 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

            Server server = CreateFromString(serverType?.ToString());

            var data = serverData.ToObject<JObject>();
            await server.DeserializeData(data);

            return server;
        }
    }
}