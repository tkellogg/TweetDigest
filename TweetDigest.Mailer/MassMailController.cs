using ActionMailer.Net.Standalone;

namespace TweetDigest.Mailer
{
    public interface IMassMailController
    {
        RazorEmailResult WeeklyFavorite();
    }

    public class MassMailController : RazorMailerBase, IMassMailController
    {
        public RazorEmailResult WeeklyFavorite()
        {
            return Email("FavoriteTweet");
        }

        public override string ViewPath
        {
            get { return "Mail"; }
        }
    }
}
