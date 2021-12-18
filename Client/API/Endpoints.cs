namespace BrickLink.Client.API
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    
    using Models;
    using Models.Request;
    using Models.Response;

    public static class Endpoints
    {
        /// <summary>
        /// This method retrieves a list of orders you received or placed.
        /// </summary>
        /// <param name="direction">The direction of the order to get</param>
        /// <param name="filed">Indicates whether the result retries filed or un-filed orders</param>
        /// <param name="includedStatuses">The status of the order to include</param>
        /// <param name="excludedStatuses">The status of the order to exclude</param>
        /// <returns>
        /// If successful, this method returns a list of the the summary of an order resource as "data" in the response
        /// body.
        /// </returns>
        public static async Task<OrderSummaryResponse> GetOrders(
            Session session,
            OrderDirection direction = OrderDirection.@in,
            bool filed = false,
            IEnumerable<OrderStatus>? includedStatuses = null,
            IEnumerable<OrderStatus>? excludedStatuses = null
        )
        {
            NameValueCollection query = new()
            {
                {"direction", direction.ToString()},
                {"filed", filed.ToString().ToLower()}
            };

            if (includedStatuses != null || excludedStatuses != null)
            {
                var empty = Enumerable.Empty<OrderStatus>();
                string statusFilter = string.Join(
                    ',',
                    (includedStatuses ?? empty)
                    .Select(status => ((OrderStatusNetwork)status).ToString())
                    .Concat(
                        (excludedStatuses ?? empty)
                        .Select(status => '-' + ((OrderStatusNetwork)status).ToString())
                    )
                );
                query.Set("status", statusFilter);
            }

            return await Session.SendRequestAsync<OrderSummaryResponse>(
                session.ConstructRequest(HttpMethod.Get, "orders", query));
        }

        /// <summary>
        /// This method retrieves the details of a specific order.
        /// </summary>
        /// <param name="orderID">The ID of the order to get</param>
        /// <returns>
        /// If successful, this method returns an order resource as "data" in the response body.
        /// </returns>
        public static async Task<OrderDetailResponse> GetOrder(
            Session session, int orderID
        ) => await Session.SendRequestAsync<OrderDetailResponse>(
            session.ConstructRequest(HttpMethod.Get, $"orders/{orderID}"));

        /// <summary>
        /// This method retrieves a list of items for the specified order.
        /// </summary>
        /// <param name="orderID">The ID of the order</param>
        /// <returns>
        /// If successful, this method returns a list of items batch list as "data" in the response body. An inner list
        /// indicates items included in one batch of the order (order item batch).
        /// </returns>
        public static async Task<OrderItemsResponse> GetOrderItems(
            Session session, int orderID
        ) => await Session.SendRequestAsync<OrderItemsResponse>(
            session.ConstructRequest(HttpMethod.Get, $"orders/{orderID}/items"));
            
        /// <summary>
        /// This method retrieves a list of the categories defined within BrickLink catalog.
        /// </summary>
        /// <returns>
        /// If successful, this method returns a list of category resource as "data" in the response body.
        /// </returns>
        public static async Task<CategoryResponse> GetCategories(
            Session session
        ) => await Session.SendRequestAsync<CategoryResponse>(
            session.ConstructRequest(HttpMethod.Get, "categories"));

        public static async Task<SubsetResponse> GetSubsets(
            Session session,
            ItemType itemType,
            string itemNumber,
            int? colorID = null,
            bool? box = null,
            bool? instruction = null,
            bool? breakMinifigs = null,
            bool? breakSubsets = null
        )
        {
            NullDroppingQuery query = new()
            {
                {"color_id", colorID},
                {"box", box?.ToString()?.ToLower()},
                {"instruction", instruction?.ToString()?.ToLower()},
                {"break_minifigs", breakMinifigs?.ToString()?.ToLower()},
                {"break_subsets", breakSubsets?.ToString()?.ToLower()},
            };

            return await Session.SendRequestAsync<SubsetResponse>(
                session.ConstructRequest(
                    HttpMethod.Get,
                    $"items/{itemType}/{itemNumber}/subsets",
                    query
                )
            );
        }

        /// <summary>
        /// This method returns the price statistics of the specified item in BrickLink catalog.
        /// </summary>
        /// <param name="itemType">The type of the item</param>
        /// <param name="number">Identification number of the item </param>
        /// <param name="colorID">The color of the item</param>
        /// <param name="guideType">Indicates which statistics to be provided</param>
        /// <param name="usedStatus">Indicates the condition of items that are included in the statistics</param>
        /// <param name="countryCode">
        /// The result includes only items in stores which are located in specified country.
        /// If you don't specify both country_code and region, this method retrieves the price information regardless of
        /// the store's location
        /// </param>
        /// <param name="region">
        /// The result includes only items in stores which are located in specified region.
        /// If you don't specify both country_code and region, this method retrieves the price information regardless of
        /// the store's location 
        /// </param>
        /// <param name="currencyCode">
        /// This method returns price in the specified currency code
        /// If you don't specify this value, price is retrieved in the base currency of the user profile's 
        /// </param>
        /// <param name="taxInclusion">
        /// Indicates that price will include VAT for the items of VAT enabled stores.
        /// </param>
        /// <returns>
        /// If successful, this method returns a price guide resource as "data" in the response body.
        /// </returns>
        public static async Task<PriceResponse> GetPrice(
            Session session,
            ItemType itemType,
            string number,
            int? colorID = null,
            GuideType guideType = GuideType.stock,
            UsedStatus usedStatus = UsedStatus.New,
            string? countryCode = null,
            Region? region = null,
            string? currencyCode = null,
            TaxInclusion taxInclusion = TaxInclusion.ExcludeVAT
        )
        {
            NullDroppingQuery query = new()
            {
                {"color_id", colorID},
                {"guide_type", guideType},
                {"new_or_used", (UsedStatusNetwork) usedStatus},
                {"country_code", countryCode},
                {"region", region},
                {"currency_code", currencyCode},
                {"vat", (TaxInclusionNetwork)taxInclusion},
            };

            return await Session.SendRequestAsync<PriceResponse>(
                session.ConstructRequest(
                    HttpMethod.Get,
                    $"items/{itemType}/{number}/price",
                    query
                )
            );
        }
    }
}
