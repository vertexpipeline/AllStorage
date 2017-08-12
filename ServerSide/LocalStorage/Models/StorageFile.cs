using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToolKit;

namespace LocalStorage.Models
{
    [Serializable]
    public class StorageFile
    {
        public Hash hash;
        public string name;
        public string path;
        public ToolKit.Models.FileState state;
    }
}
