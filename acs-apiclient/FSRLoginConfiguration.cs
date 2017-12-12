using AcsApi.Models;

namespace AcsApi
{
    public struct FSRLoginConfiguration
    {
        internal string BaseUrl { get; private set; }

        internal Consumer Consumer { get; private set; }
		
        internal string Username { get; set; }

        public FSRLoginConfiguration(string baseUrl, Consumer consumer)
        {
            BaseUrl = baseUrl;
            Consumer = consumer;
            Username = null;
        }
    }
}
