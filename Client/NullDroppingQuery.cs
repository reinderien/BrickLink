namespace BrickLink.Client
{
    using System.Collections.Specialized;

    internal class NullDroppingQuery : NameValueCollection 
    {
        public void Add(string key, object? value)
        {
            if (value != null) base.Add(key, value.ToString());
        }

        public void Set(string key, object? value)
        {
            if (value != null) base.Set(key, value.ToString());
        }
    }
}
