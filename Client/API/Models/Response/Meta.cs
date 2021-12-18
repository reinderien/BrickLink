namespace BrickLink.Client.API.Models.Response
{
    public record Meta(
        string description,
        string message,
        // A value of 2xx indicates that no errors occurred, and the transaction was successful.
        // A value other than 2xx indicates an error.
        int code
    )
    {
        public bool IsSuccess =>
            code >= 200 && code < 300;
    }
}
