using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using OW.Experts.WebUI.Infrastructure;
using OW.Experts.WebUI.ViewModels.Account;

namespace OW.Experts.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private IAuthenticationManager _authenticationManager;
        private IUserManager _userManager;

        internal IAuthenticationManager AuthenticationManager
        {
            get =>
                _authenticationManager ??
                (_authenticationManager = HttpContext.GetOwinContext().Authentication);
            set => _authenticationManager = value;
        }

        internal IUserManager UserManager
        {
            get =>
                _userManager ??
                (_userManager = HttpContext.GetOwinContext().GetUserManager<IUserManager>());
            set => _userManager = value;
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid) {
                var account = new Account { UserName = model.Login, Role = RoleNames.Expert };
                var result =
                    await UserManager.CreateAccountAndGetClaimAsync(account, model.Password);

                if (result.Succeeded) {
                    Authentication(result.Claim);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
                return View("Register", model);
            }

            return View("Register", model);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid) {
                var result =
                    await UserManager.FindAccountAndGetClaimAsync(model.Login, model.Password);
                if (result.Succeeded) {
                    Authentication(result.Claim);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
                return View("Login", model);
            }

            return View("Login", model);
        }

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private void Authentication(ClaimsIdentity claim)
        {
            AuthenticationManager.SignOut();
            AuthenticationManager.SignIn(
                new AuthenticationProperties
                {
                    IsPersistent = true
                },
                claim);
        }
    }
}