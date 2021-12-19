namespace BrickLink.Client.API.Models.Response
{
    using System.Collections.Generic;

    public record CategoryResponse(
        Meta meta,
        IReadOnlyList<Category> data
    ) : Response(meta);
}
