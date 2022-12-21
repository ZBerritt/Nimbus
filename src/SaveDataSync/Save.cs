using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaveDataSync
{
    public class Save
    {
        public string Name { get; }
        public string Location { get; }

        public Save(string name, string location)
        {
            Name = name;
            Location = location;
        }
    }
}
