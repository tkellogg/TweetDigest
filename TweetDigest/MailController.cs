using System;
using System.Linq;
using ActionMailer.Net.Mvc;
using TweetDigest.Models;

namespace TweetDigest
{
    public interface IMailController
    {
        EmailResult LoginEmail(User model);
    }

    public class MailController : MailerBase, IMailController
    {
        public EmailResult LoginEmail(User model)
        {
            Subject = "Welcome to TweetDigest";
            From = "no-reply@tweetdigest.apphb.com";
            To.Add(model.Email);
            return Email("LoginEmail", new LoginEmailViewModel
                {
                    TwitterHandle = model.TwitterHandles.FirstOrDefault() ?? model.Email,
                    ReturnUrl = "http://tweetdigest.apphb.com/login/" + Guid.NewGuid()
                });
        }
    }

    public class LoginEmailViewModel
    {
        public string Email { get; set; }
        public string TwitterHandle { get; set; }

        /// <summary>
        /// The link they click on to log back in. Contains a unique string like a Guid or whatever
        /// </summary>
        public string ReturnUrl { get; set; }
    }
}