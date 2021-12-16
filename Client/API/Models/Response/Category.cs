namespace BrickLink.Client.API.Models.Response
{
    public record Category(
        int category_id,
        string category_name,
        int parent_id
    );
}
