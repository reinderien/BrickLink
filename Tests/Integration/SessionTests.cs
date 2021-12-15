namespace BrickLink.Tests.Integration
{
    using System.Net.Http;
    using NUnit.Framework;
    
    using Client;
    
    public class SessionTests
    {
        private readonly ConfiguredSession _session = new();

        [Test]
        public void Request()
        {
            HttpRequestMessage request = _session.ConstructRequest(
                method: HttpMethod.Get, path: "orders"
            );
            Session.SendRequest(request);
        }
    }
}
