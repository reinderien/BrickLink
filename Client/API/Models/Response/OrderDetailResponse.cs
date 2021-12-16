namespace BrickLink.Client.API.Models.Response
{
    public record OrderDetailResponse(
        Meta meta,
        OrderDetails data
    ) : Response(meta);
}
