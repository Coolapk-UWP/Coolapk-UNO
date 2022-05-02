using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoolapkUNO.Networks.Converters
{
    public class StringToIntConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string) || objectType == typeof(int);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                return (uint)uint.Parse(reader.Value?.ToString() ?? "0");
            }
            catch (Exception _)
            {
                return (uint)0;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}
