namespace BrickLink.Tests.Integration
{
    using NUnit.Framework;
    
    using Client;
    using Client.Models.Response;
    
    public class SessionTests
    {
        private readonly ConfiguredSession _session = new();

        [Test]
        public void GetOrders()
        {
            OrderSummaryResponse orderSummary = Endpoints.GetOrders(
                _session,
                excludedStatuses: new[] {OrderStatus.COMPLETED}
            ).Result;
        }

        [Test]
        public void GetOrder()
        {
            OrderDetailResponse orderDetails = Endpoints.GetOrder(_session, orderID: 17318681).Result;
        }
    }
}
