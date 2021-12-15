namespace BrickLink.Tests.Integration
{
    using NUnit.Framework;
    
    using Client;
    using Client.Models.Response;
    
    public class SessionTests
    {
        private readonly ConfiguredSession _session = new();

        [Test]
        public void Request()
        {
            Orders orders = Endpoints.GetOrders(_session).Result;
        }
    }
}
