using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC_tagslist
{
    public class OPC
    {
        
        public string Server { get; set; }
        public List<string> tags;
        public OPC() 
        {
           tags = new List<string>();
        }

    }
}
