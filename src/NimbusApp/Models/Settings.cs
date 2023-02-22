using System.Runtime.Serialization;
using System.Text.Json;

namespace NimbusApp.Models
{
    /// <summary>
    /// Represents all application settings
    /// </summary>
    public class Settings
    {
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
    }



    public enum THEME
    {
        [EnumMember(Value = "light")]
        LIGHT,
        [EnumMember(Value = "dark")]
        DARK
    }
}