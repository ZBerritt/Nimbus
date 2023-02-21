using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace SaveDataSync.Models
{
    /// <summary>
    /// Represents all application settings
    /// </summary>
    public class Settings
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public THEME Theme { get; set; }

        /// <summary>
        /// Default constructor. Sets all settings to their defaults
        /// </summary>
        public Settings()
        {
            Theme = THEME.LIGHT;
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
            var json = JsonConvert.SerializeObject(this);
            return json.ToString();
        }

        /// <summary>
        /// Deserializes the settins from JSON string
        /// </summary>
        /// <param name="json">JSON stirng to deserialize</param>
        /// <returns>Settings object representing the json</returns>
        public static Settings Deseriaize(string jsonString)
        {
            var settings = JsonConvert.DeserializeObject<Settings>(jsonString);
            return settings;
        }
    }



    public enum THEME
    {
        [EnumMember(Value = "light")]
        LIGHT,
        [EnumMember(Value = "dark")]
        DARK
    }
}