﻿namespace BrickLink.Client.Models.Response
{
    public record OrderSummary(
        CostSummary cost,
        CostSummary disp_cost
    ) : BaseOrder;
}
