using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using OW.Experts.WebUI.CompositionRoot;
using OW.Experts.WebUI.CompositionRoot.IdentityEFAuth;
using OW.Experts.WebUI.Infrastructure;

[assembly: OwinStartup(typeof (IdentityConfig))]

namespace OW.Experts.WebUI.CompositionRoot
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<AppIdentityDbContext>(AppIdentityDbContext.Create);
            app.CreatePerOwinContext<IUserManager>(IdentityUserManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });
        }
    }
}