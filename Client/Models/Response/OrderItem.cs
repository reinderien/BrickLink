namespace BrickLink.Client.Models.Response
{
    using System.Text.Json.Serialization;

    public enum UsedStatus
    {
        // Actual API names
        N, U,
        // Friendly aliases
        New = N,
        Used = U
    }
    
    public record OrderItem(
        long inventory_id,
        Item item,
        int color_id,
        string color_name,
        int quantity,
        string description,
        string remarks,
        string currency_code,
        string disp_currency_code
    )
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UsedStatus new_or_used { get; init; }
    }
}
