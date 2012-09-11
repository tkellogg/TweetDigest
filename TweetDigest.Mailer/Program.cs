using System;
using System.Threading;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using TweetDigest.DependencyResolution;

namespace TweetDigest.Mailer
{
    static class Program
    {
        static void Main()
        {
            var factory = new StdSchedulerFactory();
            var sched = factory.GetScheduler();

            var container = IoC.Initialize();
            container.Configure(x => x.AddRegistry<WorkerRegistry>());
            sched.JobFactory = new StructureMapJobFactory(container);

            sched.Start();

            var detail = new JobDetailImpl("mailer", typeof (MailerJob));
            // New job at 6:30 am every day
//            sched.ScheduleJob(detail, new CronTriggerImpl("mailer", "mailers", "0 36 20 * * ?"));
            sched.ScheduleJob(detail, new SimpleTriggerImpl("mailer", "mailer"));

            if (Environment.UserInteractive) Console.WriteLine("Ctrl-C to stop...");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
