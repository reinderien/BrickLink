namespace BrickLink.Client.API.Models.Response
{
    public record Address(
        // An object representation of a person's name
        Name name,
        // The full address, not-well-formatted
        string full,
        // The first line of the address. It is provided only if a buyer updated his/her address and name as a
        // normalized form 
        string address1,
        // The second line of the address. It is provided only if a buyer updated his/her address and name as a
        // normalized form 
        string address2,
        // The country code by ISO 3166-1 alpha-2 (exception: UK instead of GB)
        string country_code,
        // The city. It is provided only if a buyer updated his/her address and name as a normalized form
        string city,
        // The state. It is provided only if a buyer updated his/her address and name as a normalized form
        string state,
        // The postal code. It is provided only if a buyer updated his/her address and name as a normalized form 
        string postal_code
    );
}
