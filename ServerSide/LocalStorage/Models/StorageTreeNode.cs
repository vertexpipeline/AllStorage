using System;
using System.Collections.Generic;
using System.Text;

namespace ToolKit.Models
{
    public class StorageTreeNode
    {
        public FileInfo info;
        public List<StorageTreeNode> children;
    }
}
