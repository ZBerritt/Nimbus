using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SaveDataSync
{
    internal class SaveList
    {
        private Dictionary<string, string> saveGameLocations = new Dictionary<string, string>();

        public void AddSave(string name, string location)
        {
            if (saveGameLocations.ContainsKey(name)) throw new Exception("Save game with name " + name + " already exists!");
            if (saveGameLocations[location] != null) throw new Exception("Save game with location " + location + " already exists");
            saveGameLocations[name] = location;
        }

        public void RemoveSave(string name)
        {
            if (!saveGameLocations.ContainsKey(name)) throw new Exception("Game list does not contain the save " + name);
            saveGameLocations.Remove(name);
        }

        public JObject ToJson()
        {
            JObject json = new JObject();
            json.Add("saves", new JArray(saveGameLocations));
            return json;
        }
    }
}
