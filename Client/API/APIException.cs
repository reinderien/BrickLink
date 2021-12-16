namespace BrickLink.Client.API
{
    using Models.Response;

    public class APIException : ClientException
    {
        public APIException(string message) : base(message) {}
    }

    public class InvalidConfigurationException : APIException
    {
        public InvalidConfigurationException(string message) : base(message) {}
    }

    public class ResponseException : APIException
    {
        public readonly Meta Meta;

        public ResponseException(Meta meta) : base(
            $"code={meta.code} message={meta.message} description={meta.description}"
        )
        { Meta = meta; }
    }
}
