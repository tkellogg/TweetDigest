using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Net;
using ActionMailer.Net;
using MongoDB.Driver;
using StructureMap;

namespace TweetDigest.DependencyResolution {
    public static class IoC {
        public static IContainer Initialize() {
            var pluralizer = PluralizationService.CreateService(CultureInfo.CurrentCulture);
            ObjectFactory.Initialize(x =>
                        {
                            x.Scan(scan =>
                                    {
                                        scan.TheCallingAssembly();
                                        scan.WithDefaultConventions();
                                    });
                            x.For<MongoDatabase>().Use(() => MongoDatabase.Create(Config.MongoUrl));
                            x.For(typeof(MongoCollection<>)).Use(context =>
                                {
                                    var db = context.GetInstance<MongoDatabase>();
                                    var type = context.BuildStack.Current.RequestedType;
                                    var modelType = type.GetGenericArguments()[0];
                                    var collectionName = pluralizer.Pluralize(modelType.Name);
                                    return db.GetCollection(modelType, collectionName);
                                });
                            x.For<IMailSender>().Use<SendGridMailSender>();
                            x.For<NetworkCredential>().Use(new NetworkCredential(Config.SendGrid.User,
                                                                                 Config.SendGrid.Password));
                        });
            return ObjectFactory.Container;
        }
    }
}