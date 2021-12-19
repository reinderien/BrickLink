namespace BrickLink.Tests.Integration
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using NUnit.Framework;
    
    using Client.Scrape.Models;
    using Client.Scrape.Pages;
    
    public class ScrapeTests
    {
        [Test]
        public void Search()
        {
            const int starwars = 65;
            Search search = new()
            {
                Query = "Luke",
                CategoryID = starwars,
                Type = ItemType.Minifigure
            };
            
            List<Item> items = PrintSearch(search).Result;
        }

        private static async Task<List<Item>> PrintSearch(Search search)
        {
            List<Item> items = new();
            await foreach (Item item in search.SearchDepaginateAsync())
                items.Add(item);
            return items;
        }

        [Test]
        public void TestPriceGuide()
        {
            IEnumerable<OrderLot> lots = PriceGuide.LoadAsync(
                type: ItemType.Minifigure,
                number: "sw0003"
            ).Result;
        }
    }
}
