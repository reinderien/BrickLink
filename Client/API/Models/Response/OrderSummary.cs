namespace BrickLink.Client.API.Models.Response
{
    public record OrderSummary(
        // Cost information for this order 
        CostSummary cost,
        // Cost information for this order in DISPLAY currency 
        CostSummary disp_cost
    ) : BaseOrder;
}
