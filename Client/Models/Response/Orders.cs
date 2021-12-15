using System.Collections.Generic;

namespace BrickLink.Client.Models.Response
{
    public record Orders(
        Meta meta,
        IReadOnlyCollection<Order> data
    ) : Response(meta) {}
}
