using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ToolKit
{
    class HashJsonConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType) => objectType == typeof(ToolKit.Hash) ? true : false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try {
                var str = reader.Value;
                var hash = Hash.FromBase64(str as string);
                return hash;
            }catch(Exception ex) {
                return null;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
