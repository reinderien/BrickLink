namespace BrickLink.Client.Models.Response
{
    using System;
    using System.Text.Json.Serialization;
  
    // https://www.bricklink.com/help.asp?helpID=41&q=order+status
    public enum OrderStatus
    {
        PENDING,
        UPDATED,
        PROCESSING,
        READY,
        PAID,
        PACKED,
        SHIPPED,
        RECEIVED,
        COMPLETED,
        OCR,  // Order cancel request
        NPB,  // Non-paying buyer alert
        NPX,  // Non-paying buyer alert, accepted
        NRS,  // Non-responding seller alert
        NSS,  // Non-shipping seller alert
        CANCELLED
    }
    
    public record Order {
        public int order_id { get; init; }
        
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime date_ordered { get; init; }
        
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime date_status_changed { get; init; }

        public string seller_name { get; init; }
        public string store_name { get; init; }
        public string buyer_name { get; init; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatus status { get; init; }
        
        public int total_count { get; init; }
        public int unique_count { get; init; }
        
        public bool is_filed { get; init; }
        public bool salesTax_collected_by_bl { get; init; }
        public bool vat_collected_by_bl { get; init; }
        
        public Payment payment { get; init; }
        public Cost cost { get; init; }
        public Cost disp_cost { get; init; }
    }
}