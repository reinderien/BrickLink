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
        /// Order has been submitted and not yet acknowledged by the seller.
        PENDING,
        /// Items have been added to or removed from the order.
        UPDATED,
        /// Order has been acknowledged by the seller but not yet ready to be invoiced.
        PROCESSING,
        /// Items in order verified, shipping or any additional costs added. Seller sends an Invoice to the buyer.
        READY, 
        /// Payment received.
        PAID, 
        /// Package has been sealed but not yet shipped.
        PACKED,
        /// Package has been shipped but not yet received by buyer.
        SHIPPED,
        /// Package has been received but the buyer has not yet verified the contents on the package.
        RECEIVED,
        /// Package received, contents verified, the order is completed.
        COMPLETED,
        /// Order has been Cancelled.
        CANCELLED,
        /// Order Cancel Request has been initiated by the buyer or seller.
        ORDER_CANCEL_REQUEST,
        /// Non-Paying Buyer Alert has been initiated by the seller.
        NON_PAYING_BUYER,
        /// Non-Paying Buyer Alert has been initiated by the seller. Buyer wishes to cancel the order and accept penalty.
        NON_PAYING_BUYER_ACCEPTED,
        /// Non-Responding Seller Alert has been initiated by the buyer.
        NON_RESPONDING_SELLER,
        /// Non-Shipping Seller Alert has been initiated by the buyer.
        NON_SHIPPING_SELLER
    }

    public abstract record BaseOrder {
        /// Unique identifier for this order for internal use 
        public int order_id { get; init; }
        
        /// The time the order was created 
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime date_ordered { get; init; }
        
        /// The time the order status was last modified 
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime date_status_changed { get; init; }

        /// The username of the seller in BL 
        public string seller_name { get; init; }
        /// The store name displayed on BL store pages 
        public string store_name { get; init; }
        /// The username of the buyer in BL
        public string buyer_name { get; init; }
        
        /// The status of an order
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrderStatusNetwork status { get; init; }
        
        /// The total number of items included in this order 
        public int total_count { get; init; }
        /// The unique number of items included in this order 
        public int unique_count { get; init; }
        
        /// Indicates whether the order is filed 
        public bool is_filed { get; init; }
        /// Indicates if sales tax are collected by BL or not 
        public bool salesTax_collected_by_bl { get; init; }
        
        [Obsolete("undocumented")]
        public bool vat_collected_by_bl { get; init; }
        
        /// Information related to the payment of this order 
        public Payment payment { get; init; }
    }
}
