namespace BrickLink.Client.Models.Response
{
    using System.Text.Json.Serialization;

    public enum ItemType
    {
        MINIFIG,
        PART,
        SET,
        BOOK,
        GEAR,
        CATALOG,
        INSTRUCTION,
        UNSORTED_LOT,
        ORIGINAL_BOX
    }

    public record Item(
        string no,
        int category_id
    )
    {
        [JsonConverter(typeof(HtmlConverter))]
        public string name { get; init; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ItemType type { get; init; }
    }
}
