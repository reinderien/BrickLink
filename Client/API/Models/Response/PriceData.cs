namespace BrickLink.Client.API.Models.Response
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public record PriceItem(
        string no,
        ItemType type
    );

    public record PriceDetail(
        int quantity,
        bool shipping_available
    )
    {
        [JsonConverter(typeof(DecimalConverter))]
        public decimal unit_price { get; init; }
        
        [Obsolete("lol")] 
        public int qunatity { get; init; }
    }

    public record PriceData(
        Item item,
        string currency_code,
        int unit_quantity,
        int total_quantity,
        IReadOnlyList<PriceDetail> price_detail
    )
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UsedStatusNetwork new_or_used { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal min_price { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal max_price { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal avg_price { get; init; }
        
        [JsonConverter(typeof(DecimalConverter))]
        public decimal qty_avg_price { get; init; }
    }
}
