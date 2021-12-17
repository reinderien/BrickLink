namespace BrickLink.Client.Scrape
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using HtmlAgilityPack;
    
    using Models;

    public static class Pages
    {
        public static async Task<IEnumerable<Item>> Search(int categoryID)
        {
            HtmlDocument doc = await Session.SendRequest(
                Session.ConstructRequest(
                    HttpMethod.Get,
                    "catalogList.asp",
                    new()
                    {
                        {"catType", "M"},
                        {"catString", categoryID.ToString()}
                    }
                )
            );

            HtmlNode resultTable = doc.DocumentNode.SelectSingleNode(
                "//form[@id='ItemEditForm']//table//table");

            IReadOnlyDictionary<string, int> headings =
                resultTable.SelectNodes("./tr[1]/td")
                    .Select((node, index) => new KeyValuePair<string, int>(node.InnerText, index))
                    .ToImmutableDictionary();

            return resultTable.SelectNodes("./tr[position() > 1]")
                    .Select(row => Item.FromNode(row, headings, Session.BaseURI));
        }
    }
}
