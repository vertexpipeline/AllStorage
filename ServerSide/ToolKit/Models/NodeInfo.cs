using System;
using System.Collections.Generic;
using System.Text;

namespace ToolKit.Models
{
    [Serializable]
    public class NodeInfo
    {
        public string name;
        public string address;
        public NodeInfo[] nodes;
        public Hash accessKey;
        public Hash ID; // hash of name 
    }
}
