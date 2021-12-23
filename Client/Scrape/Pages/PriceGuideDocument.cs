namespace BrickLink.Client.Scrape.Pages;

using System.Collections.Immutable;
using HtmlAgilityPack;
using Models;

public class PriceGuideDocument
{
    private class CurrencyGrouper
    {
        private string? _currency;

        public string? GetKey(HtmlNode node)
        {
            HtmlNode text = node.SelectSingleNode("./td[@class='pcipgCurrencyHeader']");
            if (text != null)
                _currency = text.InnerText.Trim();
            return _currency;
        }
    }

    private readonly IEnumerable<CurrencyGroup> _groups;
    
    public PriceGuideDocument(HtmlDocument doc)
    {
        HtmlNode root = doc.DocumentNode;

        HtmlNodeCollection headerRows = root.SelectNodes(
            "//tr[td[@class='pcipgMainHeader']]");

        ImmutableDictionary<string, int>
            salesIndex = MakeIndex(headerRows[0]),
            usedIndex = MakeIndex(headerRows[1], "./td[position() <= 2]");

        _groups = root.SelectNodes(
                "//table[@class='pcipgMainTable']/tr"
            )
            .GroupBy(new CurrencyGrouper().GetKey)
            .Where(group => group.Key is not null)
            .Select(group =>
                new CurrencyGroup(
                    salesIndex, usedIndex, currency: group.Key!, group
                )
            );
    }

    public IEnumerable<SoldOrderLot> SoldLots =>
        _groups.SelectMany(group => group.SoldLots);

    public IEnumerable<ForSaleOrderLot> ForSaleLots =>
        _groups.SelectMany(group => group.ForSaleLots);
    
    internal static ImmutableDictionary<string, int> MakeIndex(
        HtmlNode parent,
        string xpath = "./td"
    ) => parent.SelectNodes(xpath)
        .Select((node, index) =>
            new KeyValuePair<string, int>(node.InnerText.Trim(), index))
        .Where(pair => !string.IsNullOrWhiteSpace(pair.Key))
        .ToImmutableDictionary();
}
