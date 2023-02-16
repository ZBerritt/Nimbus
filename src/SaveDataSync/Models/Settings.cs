using Newtonsoft.Json.Linq;

namespace SaveDataSync.Models
{
    /// <summary>
    /// Represents all application settings
    /// </summary>
    public class Settings
    {
        public Settings()
        {
        }

        /// <summary>
        /// Clones the settings class, used for modification
        /// </summary>
        /// <returns>A new instance of identical settings</returns>
        public Settings Clone()
        {
            return MemberwiseClone() as Settings;
        }

        /// <summary>
        /// Serializes the settings to JSON format
        /// </summary>
        /// <returns>A JSON object representation of the settings</returns>
        public string Serialize()
        {
            var json = new JObject();
            return json.ToString();
        }

        /// <summary>
        /// Deserializes the settins from JSON string
        /// </summary>
        /// <param name="json">JSON stirng to deserialize</param>
        /// <returns>Settings object representing the json</returns>
        public static Settings Deseriaize(string json)
        {
            return new Settings();
        }
    }
}