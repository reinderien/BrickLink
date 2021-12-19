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

        public static void Main()
        {
            new Program().PriceDemo();
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
