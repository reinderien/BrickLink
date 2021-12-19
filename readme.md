# Data Sources

There are three data sources for BrickLink:

- the [official API](https://www.bricklink.com/v3/api.page?page=started), 
  supported in `BrickLink.Client.API`
- the [official data dumps](https://www.bricklink.com/catalogDownload.asp), 
  supported in `BrickLink.Analysis`
- scraping the website, supported in `BrickLink.Client.Scrape`

Data from these sources overlap but no one source is complete.

## API

Some alternate client implementations are:

- [gebirgslok/BricklinkSharp](https://github.com/gebirgslok/BricklinkSharp)
  in .NET standard 2.0
- [ryansh100/bricklink-api](https://github.com/ryansh100/bricklink-api)
  in node.js
- [Some others](https://github.com/search?o=desc&q=bricklink&s=stars&type=Repositories)

This implementation focuses on 

- ergonomic interfaces,
- strong types, including complete enumerations;
- modern C# 10/.NET 6;
- asynchronous networking support, good connection pooling, and HTTP/2.0;
- reasonably low external dependency footprint; and
- good inline documentation and annotation for obsolete or undocumented elements.

The API is authenticated and only accessible for registered seller accounts.
Authentication uses a simplified subset of OAuth 1.0 and is implemented
internally rather than pulling in an external library.

Authentication parameters need to be entered in `app.config`. To get these,
follow the [starting instructions](https://www.bricklink.com/v3/api.page?page=started).

## Dump downloads

These require signing in with a BrickLink account. `BrickLink.Analysis`
requires that you download and save these on your own, and then provide a path.

## Scraping

The website is pretty fast to scrape, doesn't require any tricky header 
manipulation, and works just fine with the well-known
[HTML Agility Pack](https://html-agility-pack.net). `BrickLink.Scrape` uses
that but with its own network layer rather than `HtmlAgilityPack.HtmlWeb` as
the latter doesn't have HTTP/2.0 support and mis-uses 
`System.Net.Http.HttpClient`.
