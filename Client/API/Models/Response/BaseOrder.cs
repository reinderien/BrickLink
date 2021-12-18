namespace BrickLink.Client.API.Models.Response
{
    using System;
    using System.Text.Json.Serialization;
  
    // https://www.bricklink.com/help.asp?helpID=41&q=order+status
    public enum OrderStatusNetwork
    {
        PENDING, UPDATED, PROCESSING, READY, PAID, PACKED, SHIPPED, RECEIVED, COMPLETED, CANCELLED,
        OCR, NPB, NPX, NRS, NSS,
    }
    
    public enum OrderStatus
    {
        PENDING, UPDATED, PROCESSING, READY, PAID, PACKED, SHIPPED, RECEIVED, COMPLETED, CANCELLED,
        ORDER_CANCEL_REQUEST,
        NON_PAYING_BUYER,
        NON_PAYING_BUYER_ACCEPTED,
        NON_RESPONDING_SELLER,
        NON_SHIPPING_SELLER
    }

    public abstract record BaseOrder {
        public int order_id { get; init; }
        
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime date_ordered { get; init; }
        
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime date_status_changed { get; init; }

        public string seller_name { get; init; }
        public string store_name { get; init; }
        public string buyer_name { get; init; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatusNetwork status { get; init; }
        
        public int total_count { get; init; }
        public int unique_count { get; init; }
        
        public bool is_filed { get; init; }
        public bool salesTax_collected_by_bl { get; init; }
        public bool vat_collected_by_bl { get; init; }
        
        public Payment payment { get; init; }
    }
}
