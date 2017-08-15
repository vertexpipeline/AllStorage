using System;
using System.Collections.Generic;
using System.Text;

namespace ToolKit.Models
{
    public class FileInfo
    {
        public string name;
        public string extension;
        public string path;
        public long size;
        public Hash fileID;
        public Hash dataHash;
        [Newtonsoft.Json.JsonConverter(typeof(ToolKit.EnumJsonConverter))]
        public FileState state;
    }
}
