using System.Web.Mvc;
using MongoDB.Driver;
using StructureMap;

namespace Culminator.App_Start
{
    public class LogErrorsAttribute : FilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            var db = ObjectFactory.GetInstance<MongoCollection<Models.Error>>();
            var error = new Models.Error(filterContext.Exception);
            db.Save(error);
        }
    }
}