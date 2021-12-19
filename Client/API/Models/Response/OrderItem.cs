namespace BrickLink.Client.API.Models.Response
{
    using System;
    using System.Text.Json.Serialization;
    
    public enum CompletenessNetwork
    {
        C, B, S,
    }

    public enum Completeness
    {
        Complete, Incomplete, Sealed
    }
    
    public record OrderItem(
        // The ID of the inventory that includes the item 
        long inventory_id,
        // An object representation of the item 
        ItemWithCategory item,
        // The ID of the color of the item 
        int color_id,
        // Color name of the item (upcoming feature)
        string color_name,
        // The number of items purchased in this order 
        int quantity,
        // The currency code of the price, by ISO 4217
        string currency_code,
        // The display currency code of the user, by ISO 4217
        string disp_currency_code,
        // User remarks of the order item 
        string remarks,
        // User description of the order item 
        string description
    )
    {
        /// Indicates whether the item is new or used 
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UsedStatusNetwork new_or_used { get; init; }
        
        /// Indicates whether the set is complete or incomplete
        /// (This value is valid only for SET type) 
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CompletenessNetwork completeness { get; init; }

        /// The original price of this item per sale unit 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal unit_price { get; init; }

        /// The unit price of this item after applying tiered pricing policy 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal unit_price_final { get; init; }

        /// The original price of this item per sale unit in display currency of the user 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal disp_unit_price { get; init; }

        /// The unit price of this item after applying tiered pricing policy in display currency of the user 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal disp_unit_price_final { get; init; }
        
        /// The weight of the item that overrides the catalog weight (upcoming feature)
        [JsonConverter(typeof(DecimalConverter))]
        public decimal weight { get; init; }
        
        [Obsolete("Undocumented")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal? order_cost { get; init; }
    }
}
