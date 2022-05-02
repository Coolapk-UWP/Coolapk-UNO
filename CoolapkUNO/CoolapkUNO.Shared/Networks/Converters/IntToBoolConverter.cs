using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoolapkUNO.Networks.Converters
{
    public class IntToBoolConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => true;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return false;
            if (reader.Value is string) return (string)reader.Value == "1";
            return (long)reader.Value >= 1;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if ((bool)value == true) writer.WriteValue(1); else writer.WriteValue(0);
        }
    }
}
