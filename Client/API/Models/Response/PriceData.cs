namespace BrickLink.Client.API.Models.Response
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public record PriceItem(
        // Item's identification number in BL catalog
        string no
    )
    {
        /// The type of the item
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ItemType type { get; init; }
    }

    public record PriceDetail(
        // The number of the items in the inventory 
        int quantity,
        // Indicates whether or not the seller ships to your country(based on the user profile) 
        bool shipping_available
    )
    {
        /// The original price of this item per sale unit 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal unit_price { get; init; }
        
        /// The number of the items in the inventory 
        [Obsolete("lol")] 
        public int? qunatity { get; init; }
    }

    public record PriceData(
        // An object representation of the item 
        PriceItem item,
        // The currency code of the price 
        string currency_code,
        // The number of times the item has been sold for last 6 months
        int unit_quantity,
        // The number of items has been sold for last 6 months 
        int total_quantity,
        IReadOnlyList<PriceDetail> price_detail
    )
    {
        /// Indicates whether the price guide is for new or used 
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UsedStatusNetwork new_or_used { get; init; }
        
        /// The lowest price of the item (in stock / that was sold for last 6 months ) 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal min_price { get; init; }
        
        /// The highest price of the item (in stock / that was sold for last 6 months ) 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal max_price { get; init; }
        
        /// The average price of the item (in stock / that was sold for last 6 months ) 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal avg_price { get; init; }
        
        /// The average price of the item (in stock / that was sold for last 6 months ) by quantity 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal qty_avg_price { get; init; }
    }
}
