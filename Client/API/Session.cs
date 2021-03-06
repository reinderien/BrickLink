namespace BrickLink.Client.API
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    
    using Client;
    
    public record Session : HttpPool
    {
        private const string Realm = "https://api.bricklink.com/api/store/v3/";
        private static readonly Uri BaseURI = new(Realm);
        
        private readonly string _consumerKey, _tokenValue, _consumerSecret, _tokenSecret;
            
        /// <summary>
        /// Construct a session providing all of the authentication parameters given to you when
        /// you make a token on the BrickLink site. For convenience, if you want these to be filled
        /// from a config file, use ConfiguredSession instead.
        /// </summary>
        public Session(string consumerKey, string tokenValue, string consumerSecret, string tokenSecret)
        {
            _consumerKey = consumerKey;
            _tokenValue = tokenValue;
            _consumerSecret = consumerSecret;
            _tokenSecret = tokenSecret;
        }

        /// <summary>
        /// Make a request object authenticated and ready to be sent.
        /// </summary>
        /// <param name="method">Any method.</param>
        /// <param name="path">
        /// A relative path after the version component of the URL. Don't include a query in this.
        /// </param>
        /// <param name="query">Optional query parameters; will be encoded.</param>
        /// <returns>A request message intended for use in SendRequest().</returns>
        public HttpRequestMessage ConstructRequest(HttpMethod method, string path, NameValueCollection? query = null)
        {
            Uri url = BuildUri(path, query);
            HttpRequestMessage request = ConstructRequest();
            request.Method = method;
            request.RequestUri = url;
            request.Headers.Authorization = OAuthHeader(method, url, query);
            
            // The API server probably ignores these, and doesn't give a Content-Type back;
            // but let's be good citizens anyway
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Accept-Charset", "UTF-8");

            return request;
        }

        /// <summary>
        /// Asynchronously send an OAuth-authenticated request. 
        /// </summary>
        /// <typeparam name="TResponse">The response type to deserialise.</typeparam>
        /// <param name="request">A request from ConstructRequest().</param>
        /// <returns>A response POCO including all metadata.</returns>
        /// <exception cref="APIException">On internal JSON deserialisation failure</exception>
        /// <exception cref="ResponseException">if the JSON body includes an error code</exception>
        /// <exception cref="HttpRequestException">
        /// if the HTTP request itself failed with an error code, or if the server lied about a
        /// redirect to an error page with an error code.
        /// </exception>
        public static async System.Threading.Tasks.Task<TResponse> 
            SendRequestAsync<TResponse>(HttpRequestMessage request)
            where TResponse: Models.Response.Response
        {
            TResponse? response;
            using (HttpResponseMessage message = await Client.SendAsync(request))
            {
                message.EnsureSuccessStatusCode();
                CheckFakeRedirects(message);
                await using (System.IO.Stream stream = await message.Content.ReadAsStreamAsync())
                {
                    response = await System.Text.Json.JsonSerializer.DeserializeAsync<TResponse>(
                        stream
                    );
                }
            }

            if (response == null)
                throw new APIException("Failed to deserialise JSON");
            if (!response.meta.IsSuccess)
                throw new ResponseException(response.meta);
            return response;
        }

        private SortedDictionary<string, string> BaseOAuthParams => new() {
            {"oauth_consumer_key", _consumerKey},
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
                .ToUnixTimeSeconds().ToString()
            },
            {"oauth_token", _tokenValue},
            {"oauth_version", "1.0"}
        };

        private void OAuthParams(
            NameValueCollection? queryAndFormParams,
            out string signatureParams,
            out string headerParams
        )
        {
            // See https://oauth.net/core/1.0 for parameter normalisation

            IEnumerable<KeyValuePair<string, string>> merged = BaseOAuthParams;
            if (queryAndFormParams != null)
                merged = merged.Concat(FlattenNameValue(queryAndFormParams));

            merged = merged
                .Select(EncodePair)
                .OrderBy(kv => kv.Key)
                .ThenBy(kv => kv.Value)
                .ToList();

            signatureParams = string.Join('&', merged.Select(PairToString));
            headerParams = string.Join(
                ", ",
                merged.Select(
                    kv => kv.Key + "=\"" + kv.Value + '"'
                )
            );
        }

        /// <summary>
        /// Create OAuth authentication information based on parameters of the HTTP request and
        /// authentication strings.
        /// We assume that we'll never POST form-encoded body parameters.
        /// This is home-grown rather than using an off-the-shelf lib because
        /// https://www.bricklink.com/v3/api.page?page=auth
        /// states that this is "OAuth-like but simpler flow".
        /// </summary>
        private System.Net.Http.Headers.AuthenticationHeaderValue
            OAuthHeader(HttpMethod method, Uri url, NameValueCollection? query)
        {
            OAuthParams(query, out string forSignature, out string forHeader);

            string baseString = 
                method.Method.ToUpper()
                + '&' + WebUtility.UrlEncode(NormaliseUri(url).ToString())
                + '&' + WebUtility.UrlEncode(forSignature);
            
            string key = _consumerSecret + '&' + _tokenSecret;

            byte[] binarySig;
            using (HMACSHA1 hash = new(key: Encoding.ASCII.GetBytes(key)))
            {
                binarySig = hash.ComputeHash(
                    Encoding.ASCII.GetBytes(baseString)
                );
            }
            string sig = WebUtility.UrlEncode(
                Convert.ToBase64String(binarySig)
            );

            string param = "realm=\"" + Realm + "\", "
                           + forHeader
                           + ", oauth_signature=\"" + sig + '"';
            return new(scheme: "OAuth", parameter: param);
        }

        private static Uri BuildUri(string path, NameValueCollection? query)
        {
            Uri preQuery = new(baseUri: BaseURI, relativeUri: path);
            if (!string.IsNullOrEmpty(preQuery.Query))
                throw new ClientException($"Path {path} must not contain a query");
            if (query == null) return preQuery;
            
            return BuilderForQuery(preQuery, query).Uri;
        }
        
        private static Uri NormaliseUri(Uri uri)
        {
            UriBuilder builder = new()
            {
                Scheme = uri.Scheme.ToLower(),
                Host = uri.Host,
                Path = uri.AbsolutePath
            };
            if (!uri.IsDefaultPort)
                builder.Port = uri.Port;
            return builder.Uri;
        }

        private static void CheckFakeRedirects(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.Found)
                return;
            
            Uri? location = response.Headers.Location;
            if (location == null || !location.AbsolutePath.EndsWith("error.page"))
                return;
    
            NameValueCollection query = HttpUtility.ParseQueryString(location.Query);
            
            HttpStatusCode? maybeCode = null;
            if (Enum.TryParse(query["code"], out HttpStatusCode code))
                maybeCode = code;

            string desc = "The response redirected to an error page";
            string? message = query["msg"];
            if (message != null)
                desc += ": " + message;
            
            throw new HttpRequestException(
                message: desc, statusCode: maybeCode, inner: null
            );
        }
    }
}
