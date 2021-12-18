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

        public static async Task<OrderDetailResponse> GetOrder(
            Session session, int orderID
        ) => await Session.SendRequestAsync<OrderDetailResponse>(
            session.ConstructRequest(HttpMethod.Get, $"orders/{orderID}"));

        public static async Task<OrderItemsResponse> GetOrderItems(
            Session session, int orderID
        ) => await Session.SendRequestAsync<OrderItemsResponse>(
            session.ConstructRequest(HttpMethod.Get, $"orders/{orderID}/items"));
            
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
                    $"items/{itemType}/{itemNumber}/subsets"
                )
            );
        }

        public static async Task<PriceResponse> GetPrice(
            ItemType itemType,
            string number,
            int? colorID = null,
            GuideType guideType = GuideType.stock
        )
        {
            throw new NotImplementedException();
        }
    }
}
