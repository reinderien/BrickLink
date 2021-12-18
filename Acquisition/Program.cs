namespace BrickLink.Acquisition
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Client.API;
    using Client.Scrape.Pages;
    using ApiModels = Client.API.Models.Response;
    using ScrapeModels = Client.Scrape.Models;
    
    public static class Program
    {
        public static void Main()
        {
            ConfiguredSession session = new();

            ApiModels.CategoryResponse categories = Endpoints.GetCategories(session).Result;
            ApiModels.Category category = categories.data
                .First(cat => 
                    cat.category_name.ToLower() == "star wars");

            Search search = new()
            {
                Query = "Luke",
                CategoryID = category.category_id,
                Type = ItemType.Minifigure
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
