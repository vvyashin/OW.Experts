using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OW.Experts.WebUI.CompositionRoot.IdentityEFAuth;
using OW.Experts.WebUI.Infrastructure.Binders;
using OW.Experts.WebUI.ViewModels.Admin;

namespace OW.Experts.WebUI.CompositionRoot
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutofacConfig.RegisterDependency();
            Database.SetInitializer(new IdentityDbInitializer());
            ModelBinders.Binders.Add(typeof(NodeCandidateListViewModel), new NodeCandidatesBinder());
            ConverterConfig.RegisterAll();
        }
    }
}