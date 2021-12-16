namespace BrickLink.Acquisition
{
    using System.Linq;
    
    using Client;
    using Client.Models.Response;

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

            // Empty. Probably because "catalog" and "category" are not the same thing
            SubsetResponse subsets = Endpoints.GetSubsets(
                session: session,
                itemType: ItemType.CATALOG,
                itemID: sw_cat.category_id).Result;
        }
    }
}
