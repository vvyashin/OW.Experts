using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using OW.Experts.WebUI.Infrastructure;

namespace OW.Experts.WebUI.CompositionRoot.IdentityEFAuth
{
    public class IdentityUserManager : UserManager<AppIdentityUser>, IUserManager
    {
        public IdentityUserManager(IUserStore<AppIdentityUser> store)
            : base(store)
        {
        }

        public static IUserManager Create()
        {
            var db = AppIdentityDbContext.Create();
            var manager = new IdentityUserManager(new UserStore<AppIdentityUser>(db));
            return manager;
        }

        public async Task<UserManagerResult> CreateAccountAndGetClaimAsync(
            Account account,
            string password,
            string applicationCookie)
        {
            var identityUser = new AppIdentityUser { UserName = account.UserName };
            var result = await CreateAsync(identityUser, password);
            if (result.Succeeded) {
                await AddToRoleAsync(identityUser.Id, RoleNames.Expert);
                var claim = await CreateIdentityAsync(identityUser, applicationCookie);
                return UserManagerResult.Success(claim);
            }

            return UserManagerResult.Fail(result.Errors);
        }

        public async Task<UserManagerResult> FindAccountAndGetClaimAsync(
            string login,
            string password,
            string applicationCookie)
        {
            var identityUser = await FindAsync(login, password);
            if (identityUser == null) return UserManagerResult.Fail("Неверный логин или пароль.");

            var claim = await CreateIdentityAsync(identityUser, applicationCookie);
            return UserManagerResult.Success(claim);
        }
    }
}