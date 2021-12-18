namespace BrickLink.Client.API
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    
    using Models.Response;

    public enum OrderDirection
    {
        @in,
        @out
    }
    
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
                    .Select(status => status.ToString())
                    .Concat(
                        (excludedStatuses ?? empty)
                        .Select(status => '-' + status.ToString())
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
            NameValueCollection query = new();
            if (colorID != null) 
                query.Set("color_id", colorID.ToString());
            if (box != null) 
                query.Set("box", box.Value.ToString().ToLower());
            if (instruction != null)
                query.Set("instruction", instruction.Value.ToString().ToLower());
            if (breakMinifigs != null)
                query.Set("break_minifigs", breakMinifigs.Value.ToString().ToLower());
            if (breakSubsets != null)
                query.Set("break_subsets", breakSubsets.Value.ToString().ToLower());

            return await Session.SendRequestAsync<SubsetResponse>(
                session.ConstructRequest(
                    HttpMethod.Get,
                    $"items/{itemType}/{itemNumber}/subsets"
                )
            );
        }
    }
}
