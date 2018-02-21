using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BusInfo
{
	public class Direction
	{
		public string Vector { get; set; }

		public Direction(string v)
		{
			this.Vector = v ?? throw new ArgumentNullException(nameof(v));
		}
	}

	public class DirectionConverter:JsonConverter
	{
		public override bool CanConvert(Type objectType) => objectType == typeof(Direction);

		public override object ReadJson(JsonReader reader,Type objectType,object existingValue,JsonSerializer serializer)
		{
			if(reader is JTokenReader jtokenreader)
			{
				return new Direction(jtokenreader.CurrentToken.ToString());
			}

			return null;
		}

		public override void WriteJson(JsonWriter writer,object value,JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}