using System;
using System.Collections.Generic;
using System.Text;

namespace ToolKit.Models.Nodes
{
    [Serializable]
    public class NodeInfo
    {
        public string name;
        public string address;
        public NodeInfo[] nodes;
        public Hash accessKey;
        public bool needKey = false;
        public Hash ID; // hash of name 
    }
}
