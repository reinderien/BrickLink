namespace BrickLink.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    

    public class Session
    {
        private static string ConsumerKey =>
            ConfigurationManager.AppSettings["ConsumerKey"] ??
            throw new InvalidConfigurationException("ConsumerKey must be set");

        private static string TokenValue =>
            ConfigurationManager.AppSettings["TokenValue"] ??
            throw new InvalidConfigurationException("TokenValue must be set");

        private static string ConsumerSecret =>
            ConfigurationManager.AppSettings["ConsumerSecret"] ??
            throw new InvalidConfigurationException("ConsumerSecret must be set");

        private static string TokenSecret =>
            ConfigurationManager.AppSettings["TokenSecret"] ??
            throw new InvalidConfigurationException("TokenSecret must be set");

        private const string Realm = "";

        private static readonly Uri BaseURI = new("https://api.bricklink.com/api/store/v3/");
        
        private static readonly HttpClient Client = new()
        {
            BaseAddress = BaseURI,
            DefaultRequestHeaders = {},
        };

        public Session()
        {
        }

        private static KeyValuePair<string, string> EncodeKV(KeyValuePair<string, string> kv)
        {
            return new(
                key: HttpUtility.UrlEncode(kv.Key),
                value: HttpUtility.UrlEncode(kv.Value)
            );
        }

        private static SortedDictionary<string, string> baseOAuthParams => new() {
            {"oauth_consumer_key", ConsumerKey},
            {
                "oauth_nonce",
                BitConverter.ToString(
                    RandomNumberGenerator.GetBytes(16) 
                ).Replace("-", "")
            },
            {"oauth_signature_method", "HMAC-SHA1"},
            {
                "oauth_timestamp", 
                ((DateTimeOffset) DateTime.UtcNow)
                .ToUnixTimeSeconds()
                .ToString()
            },
            {"oauth_token", TokenValue},
            {"oauth_version", "1.0"},
        };

        private static string NormaliseUri(Uri uri)
        {
            UriBuilder builder = new()
            {
                Scheme = uri.Scheme.ToLower(),
                Host = uri.Host,
                Path = uri.AbsolutePath
            };
            if (!uri.IsDefaultPort)
                builder.Port = uri.Port;
            return WebUtility.UrlEncode(builder.Uri.ToString());
        }

        private void OAuthParams(
            NameValueCollection queryAndFormParams,
            out string forSignature,
            out string forHeader
        )
        {
            // See https://oauth.net/core/1.0 for parameter normalisation
            
            // This will throw if a reserved key in baseDict appears in queryAndFormParams

            queryAndFormParams.Cast<string>();
            
            IDictionary<string, string> merged = baseOAuthParams
                // .Concat(
                //     queryAndFormParams.Cast<string>()
                // )
                .Select(EncodeKV)
                .ToImmutableSortedDictionary();

            forSignature = string.Join(
                '&',
                merged
                    // We skip this because we're already using SortedDictionary, and we assume that
                    // there are no parameter dupes (even though this is technically possible in HTTP).
                    // .OrderBy(kv => kv.Key)
                    // .ThenBy(kv => kv.Value)
                    .Select(kv => kv.Key + '=' + kv.Value)
            );
            forHeader = string.Join(
                ", ",
                merged.Select(
                    kv => kv.Key + "=\"" + kv.Value + '"'
                )
            );
        }

        private string OAuthHeader(string method, Uri url)
        {
            // This is home-grown rather than using an off-the-shelf lib because
            // https://www.bricklink.com/v3/api.page?page=auth
            // states that this is "OAuth-like but simpler flow".

            // We assume that we'll never POST form-encoded parameters
            OAuthParams(
                HttpUtility.ParseQueryString(url.Query),
                out string forSignature, out string forHeader);

            string baseString = method.ToUpper()
                                + '&' + NormaliseUri(url)
                                + '&' + forSignature;
            
            string key = ConsumerSecret + '&' + TokenSecret;
            HMACSHA1 hash = new(key: Encoding.ASCII.GetBytes(key));
            byte[] binarySig = hash.ComputeHash(
                Encoding.ASCII.GetBytes(baseString)
            );
            string sig = WebUtility.UrlEncode(
                Convert.ToBase64String(binarySig)
            );

            string header = "OAuth realm=\"" + Realm + "\", "
                            + forHeader
                            + ", oauth_signature=\"" + sig + '"';
            return header;
        }

        public void Request()
        {
            HttpRequestMessage request = new(
                method: HttpMethod.Get, 
                requestUri: new Uri(BaseURI, "orders")
            );
            request.Headers.Add(
                "Authorization",
                OAuthHeader(
                    method: request.Method.Method,
                    url: request.RequestUri
                )
            );
            request.Headers.Add("Accept", "application/json");

            HttpResponseMessage response = Client.Send(request);
            response.EnsureSuccessStatusCode();
            CheckFakeRedirects(response);
        }

        private static void CheckFakeRedirects(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Found)
            {
                Uri? location = response.Headers.Location;
                if (location != null 
                    && location.AbsolutePath.EndsWith("error.page"))
                {
                    NameValueCollection query = HttpUtility.ParseQueryString(location.Query);
                    
                    HttpStatusCode? maybeCode = null;
                    if (Enum.TryParse<HttpStatusCode>(
                        query["code"], out HttpStatusCode code
                    ))
                        maybeCode = code;

                    string desc = "The response redirected to an error page";
                    string? message = query["msg"];
                    if (message != null)
                        desc += ": " + message;
                    
                    throw new HttpRequestException(
                        message: desc,
                        statusCode: maybeCode,
                        inner: null
                    );
                }
            }
        }
    }
}
