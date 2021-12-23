namespace BrickLink.Client.Scrape.Models;

using System;

public record OrderLot(
    bool Used,
    int Quantity,
    string Currency,
    decimal UnitPrice
);

public record SoldOrderLot(
    OrderLot Lot,
    DateOnly Month
);

public record ForSaleOrderLot(
    OrderLot Lot,
    bool ShipsToYou,
    Uri StoreItemLink,
    string StoreName
);
