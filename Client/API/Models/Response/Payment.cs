namespace BrickLink.Client.API.Models.Response
{
    using System;
    using System.Text.Json.Serialization;

    // https://www.bricklink.com/help.asp?helpID=121
    public enum PaymentStatus
    {
        /// Payment not yet sent by buyer. This is the default status created when an order is submitted or updated.
        None,
        /// Payment is on its way. The buyer sets the payment status to this on their orders placed page.
        Sent,
        /// Payment has been received by seller but might have not cleared the bank yet.
        Received,
        /// Payment has been received by seller and is clearing.
        Clearing,
        /// Seller has returned payment back to buyer without cashing it.
        Returned,
        /// Payment failed to clear the bank.
        Bounced,
        /// Payment has been completed.
        Completed
    }
    
    public record Payment(
        // The payment method for this order 
        string method,
        // Currency code of the payment, by ISO 4217
        string currency_code,
        // Payment status
        string status
    )
    {
        /// The time the buyer paid 
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime date_paid { get; init; }
    }
}
