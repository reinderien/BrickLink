namespace BrickLink.Client.API.Models
{
    using System;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Web;

    public class HtmlConverter : JsonConverter<string>
    {
        public override string Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options
        ) => HttpUtility.HtmlDecode(
            reader.GetString() ??
            throw new NullReferenceException("String must not be null")
        );

        public override void Write(
            Utf8JsonWriter writer, string value, JsonSerializerOptions options
        ) => writer.WriteStringValue(
            HttpUtility.HtmlEncode(value));
    }
}
