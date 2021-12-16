namespace BrickLink.Client.API.Models.Response
{
    using System;
    using System.Text.Json.Serialization;

    public record Payment(
        string method,
        string currency_code,
        string status
    )
    {
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime date_paid { get; init; }
    }
}
