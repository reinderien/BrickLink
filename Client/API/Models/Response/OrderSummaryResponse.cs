namespace BrickLink.Client.API.Models.Response
{
    using System.Collections.Generic;

    public record OrderSummaryResponse(
        Meta meta,
        IReadOnlyCollection<OrderSummary> data
    ) : Response(meta);
}
