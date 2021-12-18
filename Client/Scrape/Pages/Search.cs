namespace BrickLink.Client.Scrape.Pages
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net.Http;
    using HtmlAgilityPack;
    
    using Models;

    public enum ItemType
    {
        Set, Part, Minifigure, Book, Gear, Catalog, Instruction, OriginalBox
    }

    public enum SortBy
    {
        Name, Number, Date, Year, PartCount
    }

    public enum SortDirection
    {
        Ascending, Descending,    
    }
    
    public record Search(
        // "Whole-word field" omitted - it redirects to a single item page, sometimes when it shouldn't
        int? CategoryID = null,
        int? ColourID = null,
        int ItemsPerPage = 50,  // Possible max of 200
        int MinifigMin = 0,
        int? MinifigMax = null,
        int PartMin = 0,
        int? PartMax = null,
        string? Query = null,
        bool QueryName = true,
        bool QueryNumber = false,
        SortBy SortBy = SortBy.Name,
        SortDirection SortDirection = SortDirection.Ascending,
        ItemType? Type = null,
        int? Year = null
    )
    {
        private const bool EnableImages = true;
        
        private static readonly ImmutableDictionary<ItemType, char> ItemTypeNames =
            new Dictionary<ItemType, char>()
        {
            { ItemType.Set        , 'S' },
            { ItemType.Part       , 'P' },
            { ItemType.Minifigure , 'M' },
            { ItemType.Book       , 'B' },
            { ItemType.Gear       , 'G' },
            { ItemType.Catalog    , 'C' },
            { ItemType.Instruction, 'I' },
            { ItemType.OriginalBox, 'O' },
        }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<SortBy, char> SortByNames =
            new Dictionary<SortBy, char>()
        {
            { SortBy.Name     , 'N' },
            { SortBy.Number   , 'I' },
            { SortBy.Date     , 'D' },
            { SortBy.Year     , 'Y' },
            { SortBy.PartCount, 'P' },
        }.ToImmutableDictionary();

        private static readonly ImmutableDictionary<SortDirection, char> SortDirectionNames = 
            new Dictionary<SortDirection, char>()
        {                                                                                     
            { SortDirection.Ascending , 'A' },                                                        
            { SortDirection.Descending, 'D' },                                                                                                                                                                                                                          
        }.ToImmutableDictionary();   
        

        public async IAsyncEnumerable<Item> SearchDepaginateAsync()
        {
            NameValueCollection query = MakeQuery();
            
            for (int page = 1;; page++)
            {
                query.Set("pg", page.ToString());
                HtmlDocument doc = await Session.SendRequestAsync(MakeRequest(query));

                foreach (Item item in LoadItems(doc))
                    yield return item;

                if (IsLastPage(doc)) break;
            }
        }
        
        private NameValueCollection MakeQuery()
        {
            Dictionary<string, object?> pairs = new()
            {
                {"sz", ItemsPerPage},
                {"v", EnableImages ? 1 : 0},
                {"catID", CategoryID},  // also maybe catString?
                {"colorPart", ColourID},
                {"figMin", MinifigMin},
                {"figMax", MinifigMax},
                {"partMin", PartMin},
                {"partMax", PartMax},
                {"q", Query},
                {"searchName", QueryName ? 'Y' : 'N'},
                {"searchNo", QueryNumber ? 'Y' : 'N'},
                {"sortBy", SortByNames[SortBy]},
                {"sortAsc", SortDirectionNames[SortDirection]},
                {"catType", Type == null ? null : ItemTypeNames[Type.Value]},
                {"itemYear", Year},
            };
            IEnumerable<KeyValuePair<string, string>> asStrings =
                pairs.Where(pair => pair.Value != null)
                    .Select(pair => 
                        new KeyValuePair<string, string>(pair.Key, pair.Value!.ToString()!));

            NameValueCollection query = new();
            foreach (KeyValuePair<string, string> pair in asStrings)
                query.Set(pair.Key, pair.Value);

            return query;
        }
        
        private HttpRequestMessage MakeRequest(NameValueCollection query) =>
            Session.ConstructRequest(
                HttpMethod.Get, path: "catalogList.asp", query: query);

        private static IEnumerable<Item> LoadItems(HtmlDocument doc)
        {
            HtmlNode resultTable = doc.DocumentNode.SelectSingleNode(
                "//form[@id='ItemEditForm']//table//table");

            IReadOnlyDictionary<string, int> headings =
                resultTable.SelectNodes("./tr[1]/td")
                    .Select((node, index) => new KeyValuePair<string, int>(node.InnerText, index))
                    .ToImmutableDictionary();

            return resultTable.SelectNodes("./tr[position() > 1]")
                    .Select(row => Item.FromNode(row, headings, Session.BaseURI));
        }

        private static bool IsLastPage(HtmlDocument doc)
        {
            HtmlNode next = doc.DocumentNode.SelectSingleNode("//a[text() = 'Next']");
            return next is null;
        }
    }
}
