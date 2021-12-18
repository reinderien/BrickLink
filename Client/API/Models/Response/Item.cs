namespace BrickLink.Client.API.Models.Response
{
    using System.Text.Json.Serialization;

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
