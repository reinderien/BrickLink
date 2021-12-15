namespace BrickLink.Tests.Integration
{
    using NUnit.Framework;
    using Client;
    using System.Net.Http;
    
    public class SessionTests
    {
        private readonly ConfiguredSession _session = new();
    
        [Test]
        public void Request()
        {
            HttpRequestMessage request = _session.ConstructRequest(
                method: HttpMethod.Get, path: "orders"
            );
            _session.SendRequest(request);
        }
    }
}
