using System.Configuration;

namespace Culminator
{
    public static class Config
    {
        private static string Get(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static class SendGrid
        {
            public static string User
            {
                get { return Get("SENDGRID_USER") ?? "kellogh"; }
            }

            public static string Password
            {
                get { return Get("SENDGRID_PASSWORD"); }
            }
        }

        public static string MongoUrl
        {
            get { return Get("MONGOHQ_URL") ?? "mongodb://localhost/test"; }
        }

        public static class Twitter
        {
            public static string ConsumerKey
            {
                get { return Get("TWITTER_CONSUMERKEY") ?? "Ff04h5mqNOPZqgd3vsaASA"; }
            }

            public static string ConsumerSecret
            {
                get { return Get("TWITTER_CONSUMERSECRET"); }
            }
        }
    }
}