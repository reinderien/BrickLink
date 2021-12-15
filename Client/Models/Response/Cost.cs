using System.Text.Json.Serialization;

namespace BrickLink.Client.Models.Response
{
    public record Cost
    {
        public string currency_code { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal subtotal { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal grand_total { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal final_total { get; init; }
    }
}