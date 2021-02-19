using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Protacon.NetCore.WebApi.TestUtil.Tests
{
    public class CustomTestObjectConverter : JsonConverter<CustomTestObject>
    {
        public override CustomTestObject Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var jsonDoc = JsonDocument.ParseValue(ref reader))
            {
                return new CustomTestObject(jsonDoc.RootElement.GetRawText());
            }
        }

        public override void Write(Utf8JsonWriter writer, CustomTestObject value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Content);
        }
    }
}