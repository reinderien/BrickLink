namespace BrickLink.Client.Models.Response
{
    using System;
    using System.Text.Json.Serialization;
    
    public record Shipping(
        int method_id,
        string method,
        Address address
    )
    {
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime date_shipped { get; init; }
    }
}
