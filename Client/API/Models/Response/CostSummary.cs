namespace BrickLink.Client.API.Models.Response
{
    using System.Text.Json.Serialization;
    
    public record CostSummary
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
