namespace BrickLink.Client.Scrape.Pages;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

using Models;

public static partial class PriceGuide
{
    private static readonly Regex itemParamsPat = new(
        pattern: @"
            ^\s*,?\s*             (?# start, optional comma)
            (?<key>[^:]+?):\s*    (?# key, colon)
            '?(?<value>[^']*?)'?  (?# optional quote, value, optional quote)
            \s*$                  (?# end)",
        options: RegexOptions.Compiled 
               | RegexOptions.Multiline 
               | RegexOptions.IgnorePatternWhitespace
    );

    private static HttpRequestMessage MakeOuterRequest(
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

    private static ImmutableDictionary<string, string> ParseJSParameters(HtmlDocument outerDoc)
    {
        /*
        outer document contains an anonymous, global script with this dictionary:
            
        var		_var_item		=	{
            idItem:			7525
            ,	type:			'M'
            ,	typeName:		'Minifigure'
            ,	itemno:			'sw0003'
            ,	itemnoBase:		'sw0003'
            ,	itemStatus:		'A'
            ,	invStatus:		'A'
            ,	itemSeq:		'0'
            ,	idColorDefault:	0
            ,	typeImgDefault:	'J'
            ,	catID:			'257'
            ,	idColorForPG:	0
            ,	strMainSImgUrl:	'//img.bricklink.com/ItemImage/MT/0/sw0003.t1.png'
            ,	strMainLImgUrl:	'//img.bricklink.com/ItemImage/MN/0/sw0003.png'
            ,	strLegacyLargeImgUrl:		'//img.bricklink.com/ItemImage/ML/sw0003.png'
            ,	strLegacyLargeThumbImgUrl:	'//img.bricklink.com/ItemImage/ML/sw0003.png'
            ,	strAssoc1ImgSUrl:			''
            ,	strAssoc1ImgLUrl:			''
            ,	strAssoc2ImgSUrl:			''
            ,	strAssoc2ImgLUrl:			''
            ,	strItemName:				'Darth Maul'
        };
        */
        HtmlNode scriptNode = outerDoc.DocumentNode.SelectSingleNode(
            "/html/head/script[contains(text(), '_var_item')]");
        string script = scriptNode.InnerText;
        int start = script.IndexOf('\n', startIndex: script.IndexOf("_var_item")),
            end = script.IndexOf("};", startIndex: start);
        string itemParamsBody = script.Substring(start, end - start);

        return itemParamsPat
            .Matches(itemParamsBody)
            .Select(match => new KeyValuePair<string, string>(
                match.Groups["key"].Value, match.Groups["value"].Value))
            .ToImmutableDictionary();
    }

    private static HttpRequestMessage MakeInnerRequest(
        IReadOnlyDictionary<string, string> itemParams,
        bool excludeIncomplete
    )
    {
        /*
        for example
        https://www.bricklink.com/v2/catalog/catalogitem.page?M=sw0003#T=P
        
        has JavaScript that constructs a query URL; abbreviated:
        
        var		srcUrl	= "catalogitem_pgtab.page?idItem=" + _var_item.idItem;
        srcUrl	= srcUrl + "&idColor=" + idSelectedColor;
        if ( sorttype == 2 )			srcUrl	+= '&st=1&gm=0';
        else if ( sorttype == 3 )		srcUrl	+= '&st=2&gm=0';
        else							srcUrl	+= '&st=2&gm=1';
        srcUrl	+= '&gc=' + ( $( "#_idchkPGGroupByCurrency" ).prop( "checked" ) ? 1 : 0 );
        srcUrl	+= '&ei=' + ( $( "#_idchkPGExcludeIncomplete" ).prop( "checked" ) ? 1 : 0 );
        srcUrl	+= '&prec=' + _var_pg_option.precision;
        srcUrl	+= '&loc=' + _var_pg_option.country_filter;
        srcUrl	+= '&reg=' + _var_pg_option.region_filter;
        srcUrl	+= '&showflag=' + _var_pg_option.show_flag;
        srcUrl	+= '&showbulk=' + _var_pg_option.show_bulk;
        srcUrl	+= '&currency=' + _var_pg_option.display_currency;
        
        _var_item is a literal dictionary in a /head/script.

        This loads
        https://www.bricklink.com/v2/catalog/catalogitem_pgtab.page?
            idItem=7525    Possible serial ID up to (currently) 9987
            &st=1          Sort: 1 qty desc, anything else - price asc
            &gm=1          0/1, whether to separate by month
            &gc=0          0/1, whether to separate by source currency
            &ei=0          0/1, whether to exclude incomplete
            &prec=2        Price precision, up to 4
            &showflag=1    Show store country flag in items for sale
            &showbulk=0    Affects items for sale
            &currency=32   32=CAD, ...
        */

        return Session.ConstructRequest(
            HttpMethod.Get,
            path: "v2/catalog/catalogitem_pgtab.page",
            query: new NameValueCollection()
            {
                {"idItem", itemParams["idItem"]},
                {"gm", "1"},
                {"gc", "1"},
                {"ei", excludeIncomplete ? "1" : "0"},
                {"prec", "4"},
            }
        );
    }

    public static async Task<PriceGuideDocument> LoadAsync(
        ItemType type, string number, bool excludeIncomplete = false
    )
    {
        HtmlDocument outerDoc = await Session.SendRequestAsync(
            MakeOuterRequest(type, number)
        );
        ImmutableDictionary<string, string> itemParams = ParseJSParameters(outerDoc);
        HtmlDocument innerDoc = await Session.SendRequestAsync(
            MakeInnerRequest(itemParams, excludeIncomplete)
        );
        
        return new PriceGuideDocument(innerDoc);
    }
}
