namespace BrickLink.Client.Models.Response
{
    using System.Text.Json.Serialization;
    
    public record OrderDetails(
        string buyer_email,
        bool require_insurance,
        bool is_invoiced,
        int buyer_order_count,
        bool drive_thru_sent,
        Shipping shipping,
        CostDetails cost,
        CostDetails disp_cost
    ) : BaseOrder
    {
        [JsonConverter(typeof(DecimalConverter))]
        public decimal total_weight { get; init; }
    }
}