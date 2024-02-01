using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OPC_tagslist
{
    public class Message
    {
        public string Header { get; set; }
        public List<string> tags;

        public Message() 
        {
            this.tags = new List<string>();
        }
        public Message(List<string> tags) 
        {
            this.tags = tags;
        }

    }
}
