using System;
using System.Collections.Generic;
using System.Text;

namespace ToolKit.Models
{
    [Newtonsoft.Json.JsonConverter(typeof(ToolKit.EnumJsonConverter))]
    public enum OperationState
    {
        success,
        file_exist,
        folder_not_found,
        invalid_key
    }
}
