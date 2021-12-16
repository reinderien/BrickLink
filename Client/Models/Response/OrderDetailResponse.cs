namespace BrickLink.Client.Models.Response
{
    public record OrderDetailResponse(
        Meta meta, 
        OrderDetails data
    ) : Response(meta) {}
}
