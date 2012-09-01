using System;

namespace SendGrid
{
    public static class Config
    {
        public static class SendGrid
        {
            public static string User
            {
                get { return Environment.GetEnvironmentVariable("SENDGRID_USER") ?? "kellogh"; }
            }

            public static string Password
            {
                get { return Environment.GetEnvironmentVariable("SENDGRID_PASSWORD"); }
            }
        }

        public static string MongoUrl
        {
            get { return Environment.GetEnvironmentVariable("MONGOHQ_URL") ?? "mongodb://localhost/test"; }
        }

        public static class Twitter
        {
            public static string ConsumerKey
            {
                get { return Environment.GetEnvironmentVariable("TWITTER_CONSUMERKEY") ?? "Ff04h5mqNOPZqgd3vsaASA"; }
            }

            public static string ConsumerSecret
            {
                get { return Environment.GetEnvironmentVariable("TWITTER_CONSUMERSECRET"); }
            }
        }
    }
}