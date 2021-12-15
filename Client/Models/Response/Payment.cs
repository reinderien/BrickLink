namespace BrickLink.Client.Models.Response
{
    using System;
    using System.Text.Json.Serialization;

    public record Payment
    {
        public string method { get; init; }
        public string currency_code { get; init; }
        
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime date_paid { get; init; }
        
        public string status { get; init; }
    }
}
