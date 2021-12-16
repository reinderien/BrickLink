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

        public static async Task<CategoryResponse> GetCategories(
            Session session
        ) => await Session.SendRequest<CategoryResponse>(
            session.ConstructRequest(HttpMethod.Get, "categories"));
    }
}
