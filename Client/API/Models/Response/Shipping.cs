namespace BrickLink.Client.API.Models.Response
{
    using System;
    using System.Text.Json.Serialization;
    
    public record Shipping(
        // Shipping method ID 
        int method_id,
        // Shipping method name 
        string method,
        // The object representation of the shipping address 
        Address address,
        
        #region Not typically seen from the server
        // Tracking numbers for the shipping 
        string? tracking_no,
        // URL for tracking the shipping.
        // API-only field. It is not shown on the BrickLink pages. 
        string? tracking_link
        #endregion
    )
    {
        /// Shipping date.
        /// API-only field. It is not shown on the BrickLink pages. 
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime date_shipped { get; init; }
    }
}
