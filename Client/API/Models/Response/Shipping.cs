namespace BrickLink.Client.API.Models.Response
{
    using System;
    using System.Text.Json.Serialization;
    
    public record Shipping(
        int method_id,
        string method,
        Address address,
        // Not typically seen
        string? tracking_no,
        string? tracking_link
    )
    {
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime date_shipped { get; init; }
    }
}
