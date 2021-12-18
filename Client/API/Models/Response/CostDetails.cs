namespace BrickLink.Client.API.Models.Response
{
    using System;
    using System.Text.Json.Serialization;
    
    public record CostDetails: CostSummary
    {
        /// Extra charge for this order (tax, packing, etc.) 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal etc1 { get; init; }
        
        /// Extra charge for this order (tax, packing, etc.)
        [JsonConverter(typeof(DecimalConverter))]
        public decimal etc2 { get; init; }
        
        /// Insurance cost 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal insurance { get; init; }
        
        /// Shipping cost 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal shipping { get; init; }
        
        /// Credit applied to this order 
        [JsonConverter(typeof(DecimalConverter))]
        public decimal credit { get; init; }
        
        /// Amount of coupon discount
        [JsonConverter(typeof(DecimalConverter))]
        public decimal coupon { get; init; }
        
        [Obsolete("undocumented")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal salesTax { get; init; }
        
        [Obsolete("undocumented")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal vat { get; init; }
        
        /// VAT percentage applied to this order (upcoming feature)
        [JsonConverter(typeof(DecimalConverter))]
        public decimal vat_rate { get; init; }
        
        /// Total amount of VAT included in the grand_total price (upcoming feature)
        [JsonConverter(typeof(DecimalConverter))]
        public decimal vat_amount { get; init; }

        #region Documented but typically not seen from server

        /// Sales tax collected by BL, if that applies
        [JsonConverter(typeof(DecimalConverter))]
        public decimal? salesTax_collected_by_BL { get; init; }
        
        /// The display currency code of the user
        [Obsolete("deprecated")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal? disp_currency_code { get; init; }

        /// The subtotal price in display currency of the user
        [Obsolete("deprecated")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal? disp_subtotal { get; init; }

        /// The grand total price in display currency of the user
        [Obsolete("deprecated")]
        [JsonConverter(typeof(DecimalConverter))]
        public decimal? disp_grand_total { get; init; }

        #endregion
    }
}
