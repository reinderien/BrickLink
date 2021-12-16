namespace BrickLink.Client
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
                query.Add("status", statusFilter);
            }

            return await Session.SendRequest<OrderSummaryResponse>(
                session.ConstructRequest(HttpMethod.Get, "orders", query));
        }

        public static async Task<OrderDetailResponse> GetOrder(
            Session session, int orderID
        ) => await Session.SendRequest<OrderDetailResponse>(
            session.ConstructRequest(HttpMethod.Get, $"orders/{orderID}"));

        public static async Task<OrderItemsResponse> GetOrderItems(
            Session session, int orderID
        ) => await Session.SendRequest<OrderItemsResponse>(
            session.ConstructRequest(HttpMethod.Get, $"orders/{orderID}/items"));
            
        public static async Task<CategoryResponse> GetCategories(
            Session session
        ) => await Session.SendRequest<CategoryResponse>(
            session.ConstructRequest(HttpMethod.Get, "categories"));

        public static async Task<SubsetResponse> GetSubsets(
            Session session,
            ItemType itemType,
            int itemID,
            int? colorID = null,
            bool? box = null,
            bool? instruction = null,
            bool? breakMinifigs = null,
            bool? breakSubsets = null
        )
        {
            NameValueCollection query = new();
            if (colorID != null) 
                query.Add("color_id", colorID.ToString());
            if (box != null) 
                query.Add("box", box.Value.ToString().ToLower());
            if (instruction != null)
                query.Add("instruction", instruction.Value.ToString().ToLower());
            if (breakMinifigs != null)
                query.Add("break_minifigs", breakMinifigs.Value.ToString().ToLower());
            if (breakSubsets != null)
                query.Add("break_subsets", breakSubsets.Value.ToString().ToLower());

            return await Session.SendRequest<SubsetResponse>(
                session.ConstructRequest(
                    HttpMethod.Get,
                    $"items/{itemType}/{itemID}/subsets"
                )
            );
        }
    }
}
