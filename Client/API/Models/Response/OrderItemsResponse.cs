namespace BrickLink.Client.API.Models.Response
{
    using System.Collections.Generic;
    
    public record OrderItemsResponse(
        Meta meta,
        IReadOnlyList<IReadOnlyList<OrderItem>> data
    ) : Response(meta);
}
