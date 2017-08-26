using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToolKit.Models.Packages
{
    [Serializable]
    public class FolderInfo
    {
        public string name;
        public string path;
        public Hash packageID;
        public List<FolderInfo> folders = new List<FolderInfo>();
        public List<FileInfo> files = new List<FileInfo>();
    }
}
