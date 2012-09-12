using System.Net;
using ActionMailer.Net.Standalone;

namespace TweetDigest.Mailer
{
    public interface IMassMailController
    {
        RazorEmailResult WeeklyFavorite(FavoriteTweetViewModel viewModel);
    }

    public class MassMailController : RazorMailerBase, IMassMailController
    {
        public MassMailController()
        {
            MailSender = new SendGridMailSender(new NetworkCredential(Config.SendGrid.User, Config.SendGrid.Password));
        }

        public RazorEmailResult WeeklyFavorite(FavoriteTweetViewModel viewModel)
        {
            From = "no-reply@tweetdigest.apphb.com";
            To.Add(viewModel.User.Email);
            Subject = "Your favorite tweets!";
            return Email("FavoriteTweet", viewModel);
        }

        public override string ViewPath
        {
            get { return "Mail"; }
        }
    }
}
