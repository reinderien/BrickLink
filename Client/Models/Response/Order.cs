namespace BrickLink.Client.Models.Response
{
    public record Order(
        int order_id,
        string seller_name,
        string store_name,
        string buyer_name,
        int total_count,
        int unique_count,
        bool is_filed,
        bool salesTax_collected_by_bl,
        bool vat_collected_by_bl,
        Payment payment,
        Cost cost,
        Cost disp_cost
    )
    {
        
    }
}