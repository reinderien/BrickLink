namespace BrickLink.Tests.Integration
{
    using System.Net.Http;
    using NUnit.Framework;
    
    using Client;
    using Client.Models.Response;
    
    public class SessionTests
    {
        private readonly ConfiguredSession _session = new();

        [Test]
        public void Request()
        {
            HttpRequestMessage request = _session.ConstructRequest(
                method: HttpMethod.Get, path: "orders"
            );
            Orders orders = Session.SendRequest<Orders>(request);
        }
    }
}
