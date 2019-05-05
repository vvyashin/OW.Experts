using Microsoft.AspNet.Identity.EntityFramework;

namespace OW.Experts.WebUI.CompositionRoot.IdentityEFAuth
{
    public class AppIdentityDbContext : IdentityDbContext<AppIdentityUser>
    {
        public AppIdentityDbContext()
            : base("IdentityDb")
        {
        }

        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }
}