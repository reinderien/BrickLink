namespace BrickLink.Client.Models
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class DecimalConverter : JsonConverter<decimal>
    {
        public override decimal Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options
        ) => decimal.Parse(reader.GetString());

        public override void Write(
            Utf8JsonWriter writer, decimal value, JsonSerializerOptions options
        ) => writer.WriteStringValue($"{value:f2}");
    }
}
