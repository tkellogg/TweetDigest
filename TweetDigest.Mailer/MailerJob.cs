using System;
using System.Linq;
using Quartz;
using TweetDigest.Models;

namespace TweetDigest.Mailer
{
    public class MailerJob : IJob
    {
        private readonly IUserRepository userRepository;
        private readonly ITweetRepository tweetRepository;
        private readonly IMassMailController mail;

        public MailerJob(IUserRepository userRepository, ITweetRepository tweetRepository, IMassMailController mail)
        {
            this.userRepository = userRepository;
            this.tweetRepository = tweetRepository;
            this.mail = mail;
            Console.WriteLine("MailerJob.ctor");
        }

        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("MailerJob.Execute");
            foreach(var user in userRepository.GetAll())
            {
                var tweets = tweetRepository.GetFavoritesForUser(user).Where(t => t.CreatedDate > user.EpochOfTweets);
                var vm = new FavoriteTweetViewModel
                    {
                        Tweets = tweets,
                        User = user
                    };

                mail.WeeklyFavorite(vm).Deliver();

                user.EpochOfTweets = DateTime.Now;
                userRepository.Save(user);
            }
        }
    }
}
