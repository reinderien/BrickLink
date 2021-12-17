namespace BrickLink.Client.Scrape
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net.Http;
    using HtmlAgilityPack;

    using Client;

    public record Session : HttpPool
    {
        public static readonly Uri BaseURI = new("https://www.bricklink.com/");

        public static HttpRequestMessage ConstructRequest(HttpMethod method, string path,
            NameValueCollection? query = null)
        {
            Uri url = new(baseUri: BaseURI, relativeUri: path);
            if (query != null)
                url = BuilderForQuery(url, query).Uri;

            HttpRequestMessage message = ConstructRequest();
            message.Method = method;
            message.RequestUri = url;
            message.Headers.Add("Accept", "text/html");

            return message;
        }

        public static async System.Threading.Tasks.Task<HtmlDocument>
            SendRequest(HttpRequestMessage request)
        {
            HtmlDocument doc = new();

            using (HttpResponseMessage response = await Client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();

                await using (System.IO.Stream stream = await response.Content.ReadAsStreamAsync())
                    doc.Load(stream);
            }

            return doc;
        }
    }
}
