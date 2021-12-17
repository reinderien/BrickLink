namespace BrickLink.Acquisition
{
    using System;
    using System.Linq;
    
    using Client.API;
    using Client.Scrape;
    using ApiModels = Client.API.Models.Response;
    using ScrapeModels = Client.Scrape.Models;
    
    public static class Program
    {
        public static void Main()
        {
            ConfiguredSession session = new();

            ApiModels.CategoryResponse categories = Endpoints.GetCategories(session).Result;
            ApiModels.Category sw_cat = categories.data
                .First(cat => 
                    cat.category_name.ToLower() == "star wars");

            foreach (ScrapeModels.Item item in Pages.Search(sw_cat.category_id).Result)
                Console.Out.WriteLine(item);
        }
    }
}
