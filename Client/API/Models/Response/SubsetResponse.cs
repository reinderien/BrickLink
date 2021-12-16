namespace BrickLink.Client.API.Models.Response
{
    public record SubsetResponse(
        Meta meta,
        Subset data // todo
    ) : Response(meta);
}
