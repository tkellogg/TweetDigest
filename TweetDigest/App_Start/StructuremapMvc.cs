using System.Web.Mvc;
using StructureMap;
using TweetDigest.DependencyResolution;

[assembly: WebActivator.PreApplicationStartMethod(typeof(TweetDigest.App_Start.StructuremapMvc), "Start")]

namespace TweetDigest.App_Start {
    public static class StructuremapMvc {
        public static void Start() {
            var container = (IContainer) IoC.Initialize();
            DependencyResolver.SetResolver(new SmDependencyResolver(container));
        }
    }
}