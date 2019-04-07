using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OW.Experts.WebUI.Infrastructure;

namespace OW.Experts.WebUI.CompositionRoot.IdentityEFAuth
{
    public class IdentityDbInitializer : DropCreateDatabaseIfModelChanges<AppIdentityDbContext>
    {
        protected override void Seed(AppIdentityDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            var expertRole = new IdentityRole(RoleNames.Expert);
            roleManager.Create(expertRole);

            var adminRole = new IdentityRole(RoleNames.Admin);
            roleManager.Create(adminRole);
            
            base.Seed(context);
        }
    }
}