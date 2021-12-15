namespace BrickLink.Client
{
    using System.Net.Http;
    using System.Threading.Tasks;
    
    using Models.Response;
    
    public static class Endpoints
    {
        public static async Task<Orders> GetOrders(Session session) =>
            await Session.SendRequest<Orders>(
                session.ConstructRequest(HttpMethod.Get, "orders"));
    }
}
