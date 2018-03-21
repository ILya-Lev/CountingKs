using CountingKs.Models;
using Newtonsoft.Json;
using System;

namespace CountingKs.Converters
{
    public class LinkModelConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var linkModel = value as LinkModel;
            if (linkModel == null)
            {
                writer.WriteComment($"{nameof(LinkModelConverter)} was invoked with object not of type {nameof(LinkModel)}");
                return;
            }

            writer.WriteStartObject();

            writer.WritePropertyName("href");
            writer.WriteValue(linkModel.Href);

            writer.WritePropertyName("rel");
            writer.WriteValue(linkModel.Relation);

            if (!linkModel.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
            {
                writer.WritePropertyName("method");
                writer.WriteValue(linkModel.Method.ToUpper());
            }

            if (linkModel.IsTemplated)
            {
                writer.WritePropertyName("isTemplated");
                writer.WriteValue(linkModel.IsTemplated);
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(LinkModel);
        }
    }
}