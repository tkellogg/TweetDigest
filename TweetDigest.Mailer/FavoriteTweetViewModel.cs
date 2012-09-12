using System.Collections.Generic;
using TweetDigest.Models;
using TweetSharp;

namespace TweetDigest.Mailer
{
    public class FavoriteTweetViewModel
    {
        public IEnumerable<TwitterStatus> Tweets { get; set; }
        public User User { get; set; }
    }
}
