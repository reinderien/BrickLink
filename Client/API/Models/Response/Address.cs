namespace BrickLink.Client.API.Models.Response
{
    public record Address(
        Name name,
        string full,
        string address1,
        string address2,
        string country_code,
        string city,
        string state,
        string postal_code
    );
}
