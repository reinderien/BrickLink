namespace BrickLink.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net.Http;
    using System.Web;
    
    public abstract record HttpPool
    {
        private static readonly Version MinHttpVersion = new(2, 0);
        
        /*
        .NET really wants one of these per application, mainly because this is where connection
        pooling occurs. Unfortunately, this makes it difficult to separate out cookie jars; the
        cookie container lives on the handler created implicitly on client construction. The
        cookies - currently AWSALB and AWSALBCORS (both clients) and ASPSESSIONIDSCATACQR and
        BLNEWSESSIONID (scrape) - are for AWS load-balancing and unauthenticated ASP session 
        respectively, so should not be user-specific.
        If this changes and cookies are introduced that have some meaning for an individual
        authenticated user, we'd have to get more creative and probably make our own handler.
        */
        protected static readonly HttpClient Client = new()
        {
            // .NET ignores these when we make our own messages,
            // but we set it here for completeness
            DefaultRequestVersion = MinHttpVersion,
            DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrHigher
        };
        
        protected static HttpRequestMessage ConstructRequest() => new()
        {
            // This is required because the client's default version stuff is
            // ignored when we make our own message (rather than GetAsync etc.)
            Version = MinHttpVersion
        };
        
        protected static IEnumerable<KeyValuePair<string, string>> FlattenNameValue(NameValueCollection collection)
        {
            return collection
                .Cast<string>()
                .SelectMany(
                    key =>
                        // We're already iterating over keys, so this will never be null; hence !
                        collection.GetValues(key)!
                            .Select(
                                value => new KeyValuePair<string, string>(key, value)
                            )
                );
        }

        protected static KeyValuePair<string, string> EncodePair(KeyValuePair<string, string> pair)
        {
            return new(
                key: HttpUtility.UrlEncode(pair.Key),
                value: HttpUtility.UrlEncode(pair.Value)
            );
        }

        protected static string PairToString(KeyValuePair<string, string> pair) =>
            pair.Key + '=' + pair.Value;
        
        protected static UriBuilder BuilderForQuery(Uri basis, NameValueCollection query) => new(basis)
        {
            Query = string.Join(
                '&',
                FlattenNameValue(query)
                .Select(EncodePair)
                .Select(PairToString)
            )
        };
    }
}
