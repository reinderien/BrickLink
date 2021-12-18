namespace BrickLink.Client.API.Models.Response
{
    using System.Text.Json.Serialization;
    
    public record CostSummary
    {
        // The currency code of the transaction by ISO 4217
        public string currency_code { get; init; }
        
        // The total price for the order exclusive of shipping and other costs
        // (This must equal the sum of all the items)
        [JsonConverter(typeof(DecimalConverter))]
        public decimal subtotal { get; init; }
        
        // The total price for the order inclusive of tax, shipping and other costs
        [JsonConverter(typeof(DecimalConverter))]
        public decimal grand_total { get; init; }
        
        // Grand total - Sales tax collected by BL
        [JsonConverter(typeof(DecimalConverter))]
        public decimal final_total { get; init; }
    }
}
