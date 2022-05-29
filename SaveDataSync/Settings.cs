using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SaveDataSync
{
    internal class Settings
    {
        public Settings()
        {

        }

        public Settings(JObject json)
        {

        }

        public Settings Clone()
        {
            return (Settings)MemberwiseClone();
        }

        public JObject ToJSON()
        {
           var json = new JObject();
            return json;
        }
    }
}
