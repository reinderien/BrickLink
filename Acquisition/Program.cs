namespace BrickLink.Acquisition
{
    using System.Linq;
    
    using Client.API;
    using Client.API.Models.Response;
    using Client.Scrape;
    
    public static class Program
    {
        public static void Main()
        {
            ConfiguredSession session = new();

            // Works!
            CategoryResponse categories = Endpoints.GetCategories(session).Result;
            Category sw_cat = categories.data
                .First(cat => 
                    cat.category_name.ToLower() == "star wars");

            Pages.Search(sw_cat.category_id);
        }
    }
}
