namespace BrickLink.Client.API.Models.Response
{
    using System.Collections.Generic;

    public record CategoryResponse(
        Meta meta,
        IReadOnlyCollection<Category> data
    ) : Response(meta);
}
