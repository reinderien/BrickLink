namespace BrickLink.Client.API.Models.Response
{
    using System.Collections.Generic;

    public record OrderSummaryResponse(
        Meta meta,
        IReadOnlyList<OrderSummary> data
    ) : Response(meta);
}
