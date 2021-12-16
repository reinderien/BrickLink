namespace BrickLink.Client.Models.Response
{
    using System.Text.Json.Serialization;
    
    public record CostDetails: CostSummary
    {
        [JsonConverter(typeof(DecimalConverter))]
        public decimal etc1 { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal etc2 { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal insurance { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal shipping { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal credit { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal coupon { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal salesTax { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal vat { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal vat_rate { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal vat_amount { get; init; }
    }
}
