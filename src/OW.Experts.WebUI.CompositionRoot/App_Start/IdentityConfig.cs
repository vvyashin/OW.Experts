using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using OW.Experts.WebUI.CompositionRoot;
using OW.Experts.WebUI.CompositionRoot.IdentityEFAuth;
using Owin;

[assembly: OwinStartup(typeof(IdentityConfig))]

namespace OW.Experts.WebUI.CompositionRoot
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext(AppIdentityDbContext.Create);
            app.CreatePerOwinContext(IdentityUserManager.Create);

            app.UseCookieAuthentication(
                new CookieAuthenticationOptions
                {
                    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                    LoginPath = new PathString("/Account/Login")
                });
        }
    }
}