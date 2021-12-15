namespace BrickLink.Client.Models.Response
{
    using System.Collections.Generic;

    public record Orders(
        Meta meta,
        IReadOnlyCollection<Order> data
    ) : Response(meta) {}
}
