namespace BrickLink.Client
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;
    using System.Web;
    
    using Models.Response;


    public record Session
    {
        private const string Realm = "https://api.bricklink.com/api/store/v3/";
        private static readonly Uri BaseURI = new(Realm);
        
        private static readonly HttpClient Client = new()
        {
            BaseAddress = BaseURI,
            DefaultRequestHeaders =
            {
                {"Accept", "application/json"}
            },
        };

        private readonly string
            _consumerKey,
            _tokenValue,
            _consumerSecret,
            _tokenSecret;
            
        public Session(string consumerKey, string tokenValue, string consumerSecret, string tokenSecret)
        {
            _consumerKey = consumerKey;
            _tokenValue = tokenValue;
            _consumerSecret = consumerSecret;
            _tokenSecret = tokenSecret;
        }

        private static KeyValuePair<string, string> EncodeKV(KeyValuePair<string, string> kv)
        {
            return new(
                key: HttpUtility.UrlEncode(kv.Key),
                value: HttpUtility.UrlEncode(kv.Value)
            );
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
                .ToUnixTimeSeconds()
                .ToString()
            },
            {"oauth_token", _tokenValue},
            {"oauth_version", "1.0"},
        };

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

        private static IEnumerable<KeyValuePair<string, string>> FlattenNameValue(NameValueCollection collection)
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

        private void OAuthParams(
            NameValueCollection queryAndFormParams,
            out string signatureParams,
            out string headerParams
        )
        {
            // See https://oauth.net/core/1.0 for parameter normalisation
            
            List<KeyValuePair<string, string>> merged =
                FlattenNameValue(queryAndFormParams)
                .Concat(BaseOAuthParams)
                .Select(EncodeKV)
                .OrderBy(kv => kv.Key)
                .ThenBy(kv => kv.Value)
                .ToList();

            signatureParams = string.Join(
                '&',
                merged.Select(
                    kv => kv.Key + '=' + kv.Value
                )
            );
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
        /// <returns>The value to go in the Authentication: header.</returns>
        private AuthenticationHeaderValue OAuthHeader(HttpMethod method, Uri url)
        {
            OAuthParams(
                HttpUtility.ParseQueryString(url.Query),
                out string forSignature, out string forHeader);

            string baseString = 
                method.Method.ToUpper()
                + '&' + WebUtility.UrlEncode(NormaliseUri(url).ToString())
                + '&' + WebUtility.UrlEncode(forSignature);
            
            string key = _consumerSecret + '&' + _tokenSecret;
            HMACSHA1 hash = new(key: Encoding.ASCII.GetBytes(key));
            byte[] binarySig = hash.ComputeHash(
                Encoding.ASCII.GetBytes(baseString)
            );
            string sig = WebUtility.UrlEncode(
                Convert.ToBase64String(binarySig)
            );

            string param = "realm=\"" + Realm + "\", "
                            + forHeader
                            + ", oauth_signature=\"" + sig + '"';
            return new(scheme: "OAuth", parameter: param);
        }

        public HttpRequestMessage ConstructRequest(HttpMethod method, string path)
        {
            Uri url = new(baseUri: BaseURI, relativeUri: path);
            HttpRequestMessage request = new(method: method, requestUri: url);
            request.Headers.Authorization = OAuthHeader(method: method, url: url);
            return request;
        }

        public static void SendRequest(HttpRequestMessage request)
        {
            HttpResponseMessage response = Client.Send(request);
            CheckErrors(response);
        }

        private static void CheckErrors(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            CheckFakeRedirects(response);
            CheckMeta(response);
        }

        private static void CheckMeta(HttpResponseMessage message)
        {
            Response? response = JsonSerializer.Deserialize<Response>(
                message.Content.ReadAsStream()
            );
            if (response == null)
                throw new APIException("Failed to deserialise JSON");
            if (!response.meta.IsSuccess)
                throw new ResponseException(response.meta);
        }

        private static void CheckFakeRedirects(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.Found)
                return;
            
            Uri? location = response.Headers.Location;
            if (location == null
                || !location.AbsolutePath.EndsWith("error.page"))
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
                message: desc,
                statusCode: maybeCode,
                inner: null
            );
        }
    }
}
