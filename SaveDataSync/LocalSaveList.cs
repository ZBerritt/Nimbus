using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SaveDataSync
{
    internal class LocalSaveList
    {
        private Dictionary<string, string> saveGameLocations = new Dictionary<string, string>();

        public void AddSave(string name, string location)
        {
            if (saveGameLocations.ContainsKey(name)) throw new Exception("Save game with name " + name + " already exists!");
            if (saveGameLocations.ContainsKey(location)) throw new Exception("Save game with location " + location + " already exists");
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
            JArray saves = new JArray();
            foreach (KeyValuePair<string, string> pair in saveGameLocations)
            {
                JObject saveObject = new JObject();
                saveObject.Add("name", pair.Key);
                saveObject.Add("location", pair.Value);
                saves.Add(saveObject);
            }
            json.Add("saves", saves);
            return json;
        }

        public static LocalSaveList FromJson(string json)
        {
            JObject deserializedJson = JsonConvert.DeserializeObject<JObject>(json);
            LocalSaveList list = new LocalSaveList();
            JArray saves = (JArray) deserializedJson.GetValue("saves");
            foreach (JObject save in saves)
            {
                string name = save.GetValue("name").ToString();
                string location = save.GetValue("location").ToString();
                list.AddSave(name, location);
            }

            return list;
        }
    }
}
