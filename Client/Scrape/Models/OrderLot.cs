namespace BrickLink.Client.Scrape.Models
{
    using System;

    public record OrderLot(
        DateOnly month,
        bool used,
        int quantity,
        string currency,
        decimal unitPrice
    )
    {
        
        
    }
}
