namespace BrickLink.Client.Models.Response
{
    public record Meta(
        string description,
        string message,
        int code
    )
    {
        public bool IsSuccess =>
            code >= 200 && code < 300;
    }
}
