namespace BrickLink.Client.Models.Response
{
    public record Payment(
        string method,
        string currency_code
    );
}