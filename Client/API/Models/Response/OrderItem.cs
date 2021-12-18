namespace BrickLink.Client.API.Models.Response
{
    using System;
    using System.Text.Json.Serialization;

    public enum UsedStatus
    {
        // todo - precedences are non-deterministic
        // Actual API names
        N, U,
        // Friendly aliases
        New = N,
        Used = U
    }

    public enum Completeness
    {
        // todo - precedences are non-deterministic
        C, B, S,
        Complete = C,
        Incomplete = B,
        Sealed = S
    }
    
    public record OrderItem(
        long inventory_id,
        Item item,
        int color_id,
        string color_name,
        int quantity,
        
        string currency_code,
        string disp_currency_code,
        string remarks,
        string description
    )
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UsedStatus new_or_used { get; init; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Completeness completeness { get; init; }

        [JsonConverter(typeof(DecimalConverter))]
        public decimal unit_price { get; init; }

        [JsonConverter(typeof(DecimalConverter))]
        public decimal unit_price_final { get; init; }

        [JsonConverter(typeof(DecimalConverter))]
        public decimal disp_unit_price { get; init; }

        [JsonConverter(typeof(DecimalConverter))]
        public decimal disp_unit_price_final { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal weight { get; init; }
        
        [Obsolete("Undocumented")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal? order_cost { get; init; }
    }
}
