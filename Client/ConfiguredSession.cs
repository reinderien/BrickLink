namespace BrickLink.Client
{
    using System.Configuration;
    
    public record ConfiguredSession : Session
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
    
        public ConfiguredSession() : base(
            consumerKey: ConsumerKey,
            tokenValue: TokenValue,
            consumerSecret: ConsumerSecret,
            tokenSecret: TokenSecret
        ) {}
    }
}
