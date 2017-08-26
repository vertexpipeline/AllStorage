using System;
using System.Collections.Generic;
using System.Text;

namespace ToolKit.Models.Packages
{
    [Serializable]
    public class ScanResult
    {
        public List<FileInfo> files = new List<FileInfo>();
        public List<FolderInfo> folder = new List<FolderInfo>();
        public string path;
    }
}
