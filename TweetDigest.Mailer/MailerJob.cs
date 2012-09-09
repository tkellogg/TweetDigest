using System;
using Quartz;

namespace TweetDigest.Mailer
{
    public class MailerJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Console.WriteLine("MailerJob");
        }
    }
}
