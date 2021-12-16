namespace BrickLink.Client.Scrape
{
    using System.Net.Http;
    using HtmlAgilityPack;

    public static class Pages
    {
        public static void Search(int categoryID)
        {
            HtmlDocument doc = Session.SendRequest(
                Session.ConstructRequest(
                    HttpMethod.Get,
                    "catalogList.asp",
                    new()
                    {
                        {"catType", "M"},
                        {"catString", categoryID.ToString()}
                    }
                )
            ).Result;

            HtmlNode resultTable = doc.DocumentNode.SelectSingleNode(
                "//form[@id='ItemEditForm']//table//table");

        }
    }
}
