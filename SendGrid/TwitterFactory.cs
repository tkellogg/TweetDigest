using System;
using Culminator.Models;
using TweetSharp;

namespace Culminator
{
    public interface ITwitterFactory
    {
        TwitterService Create();
        Uri GetAuthorizationUri();
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
    }
}