using System;
using System.Collections.Generic;
using System.Text;

namespace ToolKit.Models
{
    class NodeInfo
    {
        public string name;
        public System.Net.IPEndPoint address;
        public NodeInfo[] nodes;
        public Hash accessKey;
        public Hash ID; // hash of name 
    }
}
