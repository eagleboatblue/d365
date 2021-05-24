using Newtonsoft.Json;
using System;

namespace Nov.Caps.Int.D365.Crm.Core.OData
{
    public class ReferenceValueConverter : JsonConverter<Guid?>
    {
        private readonly string url;

        public ReferenceValueConverter(string url)
        {
            this.url = url;
        }

        public override Guid? ReadJson(JsonReader reader, Type objectType, Guid? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var json = reader.Value;

            if (json == null)
            {
                return null;
            }

            return Guid.Parse(json as string);
        }

        public override void WriteJson(JsonWriter writer, Guid? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();

                return;
            }

            writer.WriteValue($"/{this.url}({value})");
        }
    }
}
