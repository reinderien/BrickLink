namespace BrickLink.Client.API.Models
{  
    using System;
    using System.Globalization;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public const string Format = "yyyy-MM-dd'T'HH:mm:ss.FFFK";
        
        public override DateTime Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options
        ) => DateTime.ParseExact(
            s: reader.GetString() ?? 
               throw new NullReferenceException("Date must not be null"),
            format: Format,
            provider: CultureInfo.InvariantCulture,
            style: DateTimeStyles.AdjustToUniversal
        );

        public override void Write(
            Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options
        ) => value.ToString(format: Format);
    }
}
