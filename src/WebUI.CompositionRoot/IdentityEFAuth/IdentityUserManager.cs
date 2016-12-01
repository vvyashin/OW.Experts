using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebUI.Infrastructure;

namespace WebUI.CompositionRoot.IdentityEFAuth
{
    public class IdentityUserManager : UserManager<AppIdentityUser>, IUserManager
    {
        public IdentityUserManager(IUserStore<AppIdentityUser> store) : base(store)
        {
        }

        public static IUserManager Create()
        {
            AppIdentityDbContext db = AppIdentityDbContext.Create();
            IdentityUserManager manager = new IdentityUserManager(new UserStore<AppIdentityUser>(db));
            return manager;
        }

        public async Task<UserManagerResult> CreateAccountAndGetClaimAsync(Account account, string password,
            string applicationCookie)
        {
            AppIdentityUser identityUser = new AppIdentityUser() {UserName = account.UserName};
            IdentityResult result = await CreateAsync(identityUser, password);
            if (result.Succeeded) {
                await AddToRoleAsync(identityUser.Id, RoleNames.Expert);
                ClaimsIdentity claim = await CreateIdentityAsync(identityUser, applicationCookie);
                return UserManagerResult.Success(claim);
            }
            else {
                return UserManagerResult.Fail(result.Errors);
            }
        }

        public async Task<UserManagerResult> FindAccountAndGetClaimAsync(string login, string password,
            string applicationCookie)
        {
            AppIdentityUser identityUser = await FindAsync(login, password);
            if (identityUser == null) {
                return UserManagerResult.Fail("Неверный логин или пароль.");
            }
            else {
                ClaimsIdentity claim = await CreateIdentityAsync(identityUser, applicationCookie);
                return UserManagerResult.Success(claim);
            }
        }
    }
}