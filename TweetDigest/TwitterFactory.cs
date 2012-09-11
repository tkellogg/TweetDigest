using System;
using TweetDigest.Models;
using TweetSharp;

namespace TweetDigest
{
    public interface ITwitterFactory
    {
        TwitterService Create();
        TwitterService Create(User user);
        Uri GetAuthorizationUri();
        TwitterUser CurrentUser { get; }
    }

    public class TwitterFactory : ITwitterFactory
    {
        private readonly IContext context;

        public TwitterFactory(IContext context)
        {
            this.context = context;
        }

        public TwitterService Create()
        {
            var user = context.User;
            return Create(user);
        }

        public TwitterService Create(User user)
        {
            var isAuthenticated = user != null && !string.IsNullOrEmpty(user.AuthData.Secret)
                && !string.IsNullOrEmpty(user.AuthData.Token);

            if (!isAuthenticated)
                return new TwitterService(Config.Twitter.ConsumerKey, Config.Twitter.ConsumerSecret);

            return new TwitterService(Config.Twitter.ConsumerKey, Config.Twitter.ConsumerSecret, 
                                      user.AuthData.Token, user.AuthData.Secret);
        }

        public Uri GetAuthorizationUri()
        {
            var service = Create();
            var requestToken = service.GetRequestToken();
            return service.GetAuthorizationUri(requestToken);
        }

        public TwitterUser CurrentUser
        {
            get
            {
                var service = Create();
                return service.GetUserProfile();
            }
        }
    }
}