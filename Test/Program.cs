using System;
using System.Text;
using SendGrid;
using TweetSharp;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var buffer = new StringBuilder();
            GatherTweets(tweet => 
                buffer.AppendFormat(@"<p>
                    <a href=""{2}""><img src=""{3}""/>@{0}</a>: 
                        {1}
                </p>", tweet.User.ScreenName, tweet.TextAsHtml, tweet.User.Url,
                     tweet.User.ProfileImageUrl));
            var sendgrid = new SendGridHelpers();
            sendgrid.SendEmail(buffer.ToString());
        }

        private static void GatherTweets(Action<TwitterStatus> accumulator)
        {
            var twitter = new TwitterHelpers();
            foreach (var tweet in twitter.GetFavorites())
            {
                accumulator(tweet);
            }
        }
    }
}
