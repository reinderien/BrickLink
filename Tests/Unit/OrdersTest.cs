namespace BrickLink.Tests.Unit
{
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using NUnit.Framework;
    
    using Client.Models.Response;

    public class OrdersTest
    {
        [Test]
        public void DeserialiseJson()
        {
            OrderSummaryResponse? orders;
            using (FileStream file = new(
                path: Path.Join(
                    TestContext.CurrentContext.TestDirectory,
                    "Unit/Data/orders.json"
                ),
                mode: FileMode.Open
            ))
                orders = JsonSerializer.Deserialize<OrderSummaryResponse>(file);
            if (orders == null)
                throw new AssertionException("Failed to deserialise JSON");
                
            Assert.AreEqual(200, orders.meta.code);
            Assert.AreEqual("OK", orders.meta.description);
            Assert.AreEqual("OK", orders.meta.message);
            
            OrderSummary order = orders.data.Single();
            
            Assert.AreEqual(17318681, order.order_id);
        }
    }
}
