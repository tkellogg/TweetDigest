using Quartz;
using Quartz.Spi;
using StructureMap;

namespace TweetDigest.Mailer
{
    public class StructureMapJobFactory : IJobFactory
    {
        private readonly IContainer container;

        public StructureMapJobFactory(IContainer container)
        {
            this.container = container;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob) container.GetInstance(bundle.JobDetail.JobType);
        }
    }
}