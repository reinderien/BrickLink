namespace BrickLink.Client.Scrape.Pages;

using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Models;
using HtmlAgilityPack;

internal class CurrencyGroup
{
    private class DateGrouper
    {
        private DateOnly? _date;
        private const string DateFormat = "MMMM yyyy";
        
        public DateOnly? GetKey(HtmlNode node)
        {
            HtmlNode text = node.SelectSingleNode("./td[@class='pcipgSubHeader']");

            if (text != null)
            {
                _date = DateOnly.ParseExact(
                    s: text.InnerText,
                    format: DateFormat);
            }

            return _date;
        }
    }

    private static readonly Regex PricePat = new(
        pattern: @"
            ^\s*~?               (?# Start, optional approx-tilde)
            .+?                  (?# Currency, like CA $)
            (?<amount>\d*\.\d*)  (?# Decimal amount)
            \s*$                 (?# end)",
        options: RegexOptions.Compiled 
               | RegexOptions.IgnorePatternWhitespace
    );

    private static readonly ICollection<HtmlNode> Empty =
        new HtmlNode[]{};
    
    private readonly IImmutableDictionary<string, int> _salesIndex, _usedIndex;
    private readonly string _currency;
    private readonly HtmlNodeCollection _columns;

    public CurrencyGroup(
        IImmutableDictionary<string, int> salesIndex,
        IImmutableDictionary<string, int> usedIndex,
        string currency,
        IEnumerable<HtmlNode> group)
    {
        _currency = currency;
        _salesIndex = salesIndex;
        _usedIndex = usedIndex;

        // First row - currency (we already have it)
        // Second row - summary statistics (ignore them)
        // Third row - has new sold, used sold, new avail, used avail
        _columns = group.ToArray()[2].SelectNodes("./td");
    }

    private bool AtEnd => _columns.Count < 4;

    public IEnumerable<SoldOrderLot> SoldLots =>
        NewAndUsed(LoadSoldColumn, "Last 6 Months Sales:");

    public IEnumerable<ForSaleOrderLot> ForSaleLots =>
        NewAndUsed(LoadForSaleColumn, "Current Items for Sale:");

    private IEnumerable<TLot> NewAndUsed<TLot>(
        Func<HtmlNode, bool, IEnumerable<TLot>> loadColumn,
        string head
    )
    {
        if (AtEnd) return Enumerable.Empty<TLot>();

        int offset = 2 * _salesIndex[head];

        IEnumerable<TLot>
            newLots = loadColumn(
                _columns[offset + _usedIndex["New"]], false
            ),
            usedLots = loadColumn(
                _columns[offset + _usedIndex["Used"]], true
            );

        return newLots.Concat(usedLots);
    }

    private IEnumerable<SoldOrderLot> LoadSoldColumn(
        HtmlNode column, bool used
    ) => (column.SelectNodes($".//tr") ?? Empty)
            .GroupBy(new DateGrouper().GetKey)
            .Where(group => group.Key is not null)
            .SelectMany(group => 
                GroupToLots(group, used)
                .Select(lot => 
                    new SoldOrderLot(lot.Item1, Month: group.Key!.Value))
            );

    private IEnumerable<ForSaleOrderLot> LoadForSaleColumn(
        HtmlNode column, bool used
    ) => GroupToLots(
            (column.SelectNodes(".//tr") ?? Empty), used
        )
        .Select(ForSaleLotFromRow);

    private IEnumerable<Tuple<OrderLot, HtmlNodeCollection>> GroupToLots(
        IEnumerable<HtmlNode> group, bool used
    )
    {
        Queue<HtmlNode> rows = new(group);
        if (rows.Count < 2)
            yield break;
        
        HtmlNode dateRow = rows.Dequeue(),
            heads = rows.Dequeue();
        foreach (HtmlNode row in rows)
        {
            HtmlNodeCollection cells = row.SelectNodes("./td");
            const int nMinCols = 3;
            if (cells.Count < nMinCols)
                break;

            yield return new(
                LotFromRow(cells, used), cells);
        }
    }

    private OrderLot LotFromRow(IList<HtmlNode> cells, bool used)
    {
        // The MakeIndex pattern doesn't work here because the markup is nonsense.
        // There are more headers than there are cell columns, and they don't match up.
        const int quantityIdx = 1, eachIdx = 2;
        string each = cells[eachIdx].InnerText,
            quantity = cells[quantityIdx].InnerText;
        Match eachFields = PricePat.Match(each);
        return new OrderLot(
            Currency: _currency, Used: used,
            Quantity: int.Parse(quantity),
            UnitPrice: decimal.Parse(eachFields.Groups["amount"].Value)
        );
    }

    private ForSaleOrderLot ForSaleLotFromRow(Tuple<OrderLot, HtmlNodeCollection> pair)
    {
        HtmlNode storeAnchor = pair.Item2[0].SelectSingleNode("./a"),
            storeImage = storeAnchor.SelectSingleNode("./img");
        
        return new ForSaleOrderLot(
            Lot: pair.Item1,
            ShipsToYou: storeImage.Attributes["src"].Value.EndsWith("Y.png"),
            StoreItemLink: new Uri(storeAnchor.Attributes["href"].Value),
            StoreName: storeImage.Attributes["title"].Value
        );

    }
}
