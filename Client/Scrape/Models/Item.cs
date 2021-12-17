namespace BrickLink.Client.Scrape.Models
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Web;
    using HtmlAgilityPack;

    public record Category(
        string Id,
        string Name,
        int Level,
        Uri Link
    )
    {
        public static Category FromNode(HtmlNode anchor, int level, Uri root) 
        {
            Uri link = new(baseUri: root, relativeUri: anchor.Attributes["href"].Value);
            return new(
                Id: HttpUtility.ParseQueryString(link.Query)["catString"] ??
                    throw new ClientException("Missing category ID"),
                Name: anchor.InnerText,
                Level: level,
                Link: link
            );
        }
    }
    
    public record Item(
        string Name,
        string Number,
        Uri? InventoryLink,
        Uri ItemLink,
        Uri Image,
        Uri TypeLink,
        char TypeCode,
        string TypeName,
        IReadOnlyList<Category> Categories
    )
    {
        public static Item FromNode(HtmlNode row, IReadOnlyDictionary<string, int> headings, Uri root)
        {
            HtmlNodeCollection cells = row.SelectNodes("./td");

            ParseImage(
                imageCell: cells[headings["Image"]],
                out Uri image);

            ParseItem(
                itemCell: cells[headings["Item No."]], root: root,
                out Uri itemLink, out string number, out Uri? inventoryLink);
                
            ParseDesc(
                descCell: cells[headings["Description"]], root: root,
                out string name, out Uri typeLink, out string typeName,
                out char typeCode, out IReadOnlyList<Category> categories);
                
            return new(
                name, number, inventoryLink, itemLink, 
                image, typeLink, typeCode, typeName, categories
            );
        }

        private static void ParseImage(
            HtmlNode imageCell,
            out Uri image)
        {
            image = new(
                imageCell
                .SelectSingleNode(".//img")
                .Attributes["src"].Value);
        }

        private static void ParseItem(
            HtmlNode itemCell,
            Uri root,
            out Uri itemLink,
            out string number,
            out Uri? inventoryLink
        )
        {
            itemLink = new(
                baseUri: root, 
                relativeUri: itemCell
                    .SelectSingleNode(".//a[1]")
                    .Attributes["href"].Value);
            
            number = (
                itemCell.SelectSingleNode(".//a[1]")
            ).InnerText;

            inventoryLink = null;
            HtmlNode inventoryAnchor = itemCell.SelectSingleNode(".//a[text() = 'Inv']");
            if (inventoryAnchor != null)
                inventoryLink = new(
                    baseUri: root,
                    relativeUri: inventoryAnchor.Attributes["href"].Value
                );
        }

        private static void ParseDesc(
            HtmlNode descCell,
            Uri root,
            out string name,
            out Uri typeLink,
            out string typeName,
            out char typeCode,
            out IReadOnlyList<Category> categories)
        {
            name = HttpUtility.HtmlDecode(
                (
                    descCell.SelectSingleNode(".//strong")
                ).InnerText
            );

            HtmlNode typeAnchor = descCell.SelectSingleNode(".//a[starts-with(@href, 'catalogTree.asp')]");
            typeLink = new(
                baseUri: root,
                relativeUri: typeAnchor.Attributes["href"].Value
            );
            
            typeName = typeAnchor.InnerText;
            
            string? typeCodeStr = HttpUtility.ParseQueryString(typeLink.Query)["itemType"];
            if (typeCodeStr is not {Length: 1})
                throw new ClientException($"Unexpected type code '{typeCodeStr}'");
            typeCode = typeCodeStr[0];

            categories = descCell.SelectNodes(
                    ".//a[starts-with(@href, '/catalogList.asp')]"
                ).Select((anchor, index) => Category.FromNode(anchor, index, root))
                .ToImmutableList();
        }
    }
}
