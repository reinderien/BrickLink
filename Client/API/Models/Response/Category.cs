namespace BrickLink.Client.API.Models.Response
{
    public record Category(
        // The ID of the category 
        int category_id,
        // The name of the category 
        string category_name,
        // The ID of the parent category in category hierarchies (0 if this category is root)
        int parent_id
    );
}
