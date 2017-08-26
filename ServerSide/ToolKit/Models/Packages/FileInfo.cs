using System;
using System.Collections.Generic;
using System.Text;

namespace ToolKit.Models.Packages
{
    [Serializable]
    public class FileInfo
    {
        public string name;
        public string extension;
        public string path;
        public long size;
        public Hash fileID;
        public Hash packageID;
        public Hash dataHash;
        public DateTime modDate;
        [Newtonsoft.Json.JsonConverter(typeof(ToolKit.EnumJsonConverter))]
        public FileState state;
    }
}
