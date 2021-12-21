using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;

namespace BrickLink.Acquisition
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Analysis;
    using Client.API;
    using Client.Scrape.Pages;
    using ApiModels = Client.API.Models.Response;
    using ScrapeModels = Client.Scrape.Models;
  
    public class Program
    {
        private readonly ConfiguredSession _session = new();

        private class DateGrouper
        {
            private DateOnly? date;
            private const string DateFormat = "MMMM yyyy";
            
            public DateOnly? GetKey(HtmlNode node)
            {
                HtmlNode text = node.SelectSingleNode("./td[@class='pcipgSubHeader']");

                if (text != null)
                {
                    date = DateOnly.ParseExact(
                        s: text.InnerText,
                        format: DateFormat);
                }
    
                return date;
            }
        }

        private static IEnumerable<ScrapeModels.OrderLot> GroupToLots(
            IGrouping<DateOnly?, HtmlNode> group
        )
        {
            if (group.Key is null) yield break;
            
            foreach (HtmlNode node in @group)
            {
                yield return null;
            }
        }

        public static void Main()
        {



            HtmlDocument doc = new HtmlDocument();
            using (StreamReader reader = new StreamReader(
                @"C:\Users\gtoom\AppData\Roaming\JetBrains\Rider2021.2\scratches\price-guide-child.html"))
                doc.Load(reader);


            var grouper = new DateGrouper();


            var grouped = doc.DocumentNode
                .SelectNodes("//td[@class='pcipgOddColumn'][1]/table/tr")
                .GroupBy(grouper.GetKey)
                .SelectMany(GroupToLots)
                .ToList();
            //.Select(node => node.OuterHtml);




            // new Program().PriceDemo();
        }

        public void PriceDemo()
        {
            int nnull = 0, total = 0;
            foreach (var row in CatalogueItem.FromTsv())
            {
                if (row.Weight is null)
                    nnull++;
                total++;
            }
        }

        public void ScrapeDemo()
        {
            ApiModels.CategoryResponse categories = Endpoints.GetCategories(_session).Result;
            ApiModels.Category category = categories.data
                .First(cat => 
                    cat.category_name.ToLower() == "star wars");

            Search search = new()
            {
                Query = "Luke",
                CategoryID = category.category_id,
                Type = ScrapeModels.ItemType.Minifigure
            };
            
            PrintSearch(search).Wait();
        }

        private static async Task PrintSearch(Search search)
        {
            await foreach (ScrapeModels.Item item in search.SearchDepaginateAsync())
                Console.Out.WriteLine(item);
        }
    }
}
