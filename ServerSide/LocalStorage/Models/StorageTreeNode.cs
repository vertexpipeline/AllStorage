using System;
using System.Collections.Generic;
using System.Text;
using ToolKit.Models.Packages;

namespace ToolKit.Models
{
    public class StorageTreeNode
    {
        public FileInfo info;
        public List<StorageTreeNode> children;
    }
}
