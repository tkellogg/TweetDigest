using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using MongoDB.Driver;
using StructureMap;
namespace SendGrid {
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
                        });
            return ObjectFactory.Container;
        }
    }
}