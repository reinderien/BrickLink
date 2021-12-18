namespace BrickLink.Client.API.Models.Response
{
    public record Name(
        // The full name of this person, including middle names, suffixes, etc. 
        string full,
        // The given name (first name) of this person.
        // It is provided only if a buyer updated his/her address and name as a normalized form 
        string first,
        // The family name (last name) of this person.
        // It is provided only if a buyer updated his/her address and name as a normalized form 
        string last
    );
}
