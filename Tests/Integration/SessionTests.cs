
namespace BrickLink.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using Client;

    public class SessionTests
    {
        private readonly Session session = new();
    
        [Test]
        public void RequestWithReservedKey()
        {
            // try
            // {
            //     session.OAuthHeader(
            //         method: "GET",
            //         url: new Uri("http://foo?oauth_nonce=conflicting_key"
            //     );
            //     Assert.Fail();
            // }
            // catch (ArgumentException ex) 
            //     when (ex.Message.StartsWith("An element with the same key but a different value already exists."))
            // { }
        }

        [Test]
        public void Request()
        {
            session.Request();
        }
    }
}
