using StructureMap.Configuration.DSL;

namespace TweetDigest.Mailer
{
    class WorkerRegistry : Registry
    {
        public WorkerRegistry()
        {
            Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.WithDefaultConventions();
                });
        }
    }
}
