namespace BrickLink.Client
{
    using System;

    public class ClientException : Exception
    {
        public ClientException(string message) : base(message) {}
    }
    
    public class InvalidConfigurationException : ClientException
    {
        public InvalidConfigurationException(string message) : base(message) {}
    }
}
