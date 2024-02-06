using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC_tagslist
{
    public class Config
    {
        public string POST { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public OPC Opc { get; set; }
        public Config() { } 
    }
}
