namespace BrickLink.Client
{
    using System;
    
    public class ClientException : Exception
    {
        public ClientException(string message) : base(message) {}
    }
}
