using System.Runtime.Serialization;

namespace NimbusApp.Settings
{
    /// <summary>
    /// Represents all application settings
    /// </summary>
    public class AppSettings
    {
        public THEME Theme { get; set; }

        /// <summary>
        /// Default constructor. Sets all settings to their defaults
        /// </summary>
        public AppSettings()
        {
            Theme = THEME.LIGHT;
        }

        /// <summary>
        /// Clones the settings class, used for modification
        /// </summary>
        /// <returns>A new instance of identical settings</returns>
        public AppSettings Clone()
        {
            return MemberwiseClone() as AppSettings;
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