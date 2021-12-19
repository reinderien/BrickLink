namespace BrickLink.Client.Scrape.Pages
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Net.Http;
    using System.Threading.Tasks;
    using HtmlAgilityPack;

    using Models;

    public static class PriceGuide
    {
        /* for example
        https://www.bricklink.com/v2/catalog/catalogitem.page?M=sw0003#T=P
        loads
        https://www.bricklink.com/v2/catalog/catalogitem_pgtab.page?
            idItem=7525    Possible serial ID up to (currently) 9987
            &st=1          Sort: 1 qty desc, anything else - price asc, 
            &gm=1          0/1, whether to separate by month
            &gc=0          0/1, whether to separate by source currency
            &ei=0          No effect?
            &prec=2        Price precision, up to 4
            &showflag=1    Show store country flag in items for sale
            &showbulk=0    Affects items for sale
            &currency=32   32=CAD, ...
        */
        
        private static HttpRequestMessage MakeRequest(
            ItemType type, string number
        )
        {
            string typeCode = ((ItemTypeNetwork) type).ToString();

            return Session.ConstructRequest(
                HttpMethod.Get,
                path: "v2/catalog/catalogitem.page",
                query: new NameValueCollection()
                {
                    {typeCode, number},
                    {"T", "P"}
                }
            );
        }

        private static IEnumerable<OrderLot> LoadDoc(HtmlDocument doc)
        {
            HtmlNode mainDiv = doc.DocumentNode.SelectSingleNode("//div[@id='_idPGContents']");
            return null;
        }

        public static async Task<IEnumerable<OrderLot>> LoadAsync(
            ItemType type, string number
        )
        {
            HtmlDocument doc = await Session.SendRequestAsync(
                MakeRequest(type, number)
            );
            return LoadDoc(doc);
        }
    }
}
