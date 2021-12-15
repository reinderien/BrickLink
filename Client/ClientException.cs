namespace BrickLink.Client
{
    using System;
    
    using Models.Response;

    public class ClientException : Exception
    {
        public ClientException(string message) : base(message) {}
    }
    
    public class InvalidConfigurationException : ClientException
    {
        public InvalidConfigurationException(string message) : base(message) {}
    }

    public class APIException : Exception
    {
        public readonly Meta Meta;

        public APIException(Meta meta) : base(
            $"code={meta.code} message={meta.message} description={meta.description}"
        )
        {
            Meta = meta; 
        }
    }
}
