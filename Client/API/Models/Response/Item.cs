namespace BrickLink.Client.API.Models.Response
{
    using System.Text.Json.Serialization;

    public record Item(
        // Item's identification number in BL catalog 
        string no,
        // The main category of the item
        int category_id
    )
    {
        /// The name of the item 
        [JsonConverter(typeof(HtmlConverter))]
        public string name { get; init; }

        /// The type of the item 
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ItemType type { get; init; }
    }
}
