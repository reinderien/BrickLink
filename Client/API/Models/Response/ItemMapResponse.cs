namespace BrickLink.Client.API.Models.Response
{
    using System.Collections.Generic;
    public record ItemMapResponse(
        Meta meta,
        IReadOnlyList<ItemMap> data
    ) : Response(meta);
}
