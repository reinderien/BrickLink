namespace BrickLink.Client.API.Models.Response
{
    public record ItemMap(
        // An object representation of the item 
        Item item,
        // Color ID of the item 
        int color_id,
        // Color name of the item 
        string color_name,
        // Element ID of the item in specific color 
        string element_id
    );
}
