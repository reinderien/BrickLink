namespace BrickLink.Client.API.Models.Response
{
    using System.Text.Json.Serialization;
    
    public record OrderDetails(
        // E-mail address of the buyer
        string buyer_email,
        // Indicates whether the buyer requests insurance for this order 
        bool require_insurance,
        // Indicates whether the order invoiced 
        bool is_invoiced,
        // Total count of all orders placed by the buyer in the seller's store. Includes the order just placed and also
        // purged orders 
        int buyer_order_count,
        // Indicates whether "Thank You, Drive Thru!" email has been sent 
        bool drive_thru_sent,
        // Information related to the shipping 
        Shipping shipping,
        // Cost information for this order 
        CostDetails cost,
        // Cost information for this order in DISPLAY currency 
        CostDetails disp_cost,
        // User remarks for this order. Not typically seen.
        string? remarks
    ) : BaseOrder
    {
        // The total weight of the items ordered.
        // It applies the seller's custom weight when present to override the catalog weight.
        // 0 if the order includes at least one item without any weight information or incomplete set 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal total_weight { get; init; }
    }
}
