using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace BusInfo
{
    public class DirectionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Direction);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader is JTokenReader jtokenreader)
            {
                return new Direction(jtokenreader.CurrentToken.ToString());
            }

            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
