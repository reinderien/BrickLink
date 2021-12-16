namespace BrickLink.Client.API.Models.Response
{
    using System.Collections.Generic;
    
    public record OrderItemsResponse(
        Meta meta,
        IReadOnlyCollection<IReadOnlyCollection<OrderItem>> data
    ) : Response(meta);
}
