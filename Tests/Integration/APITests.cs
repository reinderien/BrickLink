namespace BrickLink.Tests.Integration
{
    using NUnit.Framework;
    
    using Client.API;
    using Client.API.Models;
    using Client.API.Models.Response;
    
    public class APITests
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
        
        [Test]
        public void GetOrderItems()
        {
            OrderItemsResponse items = Endpoints.GetOrderItems(_session, orderID: 17318681).Result;
        }

        [Test]
        public void GetCategories()
        {
            CategoryResponse categories = Endpoints.GetCategories(_session).Result;
        }

        [Test]
        public void GetPrice()
        {
            PriceResponse prices = Endpoints.GetPrice(
                session: _session,
                itemType: ItemType.MINIFIG,
                number: "sh011",
                currencyCode: "CAD").Result;
        }

        [Test]
        public void GetElementID()
        {
            ItemMapResponse items = Endpoints.GetElementID(
                session: _session,
                number: "3626cpb1516").Result;
        }
    }
}
