using System.Collections.Generic;
using TweetSharp;

namespace TweetDigest.Models
{
    public interface ITweetRepository
    {
        IEnumerable<TwitterStatus> GetFavoritesForUser(User user);
    }

    public class TweetRepository : ITweetRepository
    {
        private readonly ITwitterFactory twitterFactory;

        public TweetRepository(ITwitterFactory twitterFactory)
        {
            this.twitterFactory = twitterFactory;
        }

        public IEnumerable<TwitterStatus> GetFavoritesForUser(User user)
        {
            var twitterService = twitterFactory.Create(user);
            return twitterService.ListFavoriteTweets();
        }
    }
}