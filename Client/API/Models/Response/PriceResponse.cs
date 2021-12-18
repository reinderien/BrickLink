namespace BrickLink.Client.API.Models.Response
{
    public record PriceResponse(
        Meta meta,
        PriceData data
        ) : Response(meta);
}
