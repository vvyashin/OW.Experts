using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebUI.Infrastructure;
using WebUI.ViewModels.Account;

[assembly: InternalsVisibleTo("WebUI.UnitTests")]

namespace WebUI.Controllers
{
    public class AccountController : Controller
    {
        private IUserManager _userManager;
        private IAuthenticationManager _authenticationManager;

        internal IAuthenticationManager AuthenticationManager
        {
            get
            {
                return _authenticationManager ??
                       (_authenticationManager = HttpContext.GetOwinContext().Authentication);
            }
            set { _authenticationManager = value; }
        }

        internal IUserManager UserManager
        {
            get
            {
                return _userManager ??
                       (_userManager = HttpContext.GetOwinContext().GetUserManager<IUserManager>());
            }
            set { _userManager = value; }
        }

        private void Authentication(ClaimsIdentity claim)
        {
            AuthenticationManager.SignOut();
            AuthenticationManager.SignIn(new AuthenticationProperties
            {
                IsPersistent = true
            }, claim);
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
                Account account = new Account {UserName = model.Login, Role = RoleNames.Expert};
                UserManagerResult result =
                    await UserManager.CreateAccountAndGetClaimAsync(account, model.Password);

                if (result.Succeeded) {
                    Authentication(result.Claim);
                    return RedirectToAction("Index", "Home");
                }
                else {
                    foreach (string error in result.Errors) {
                        ModelState.AddModelError("", error);
                    }
                    return View("Register", model);
                }
            }
            else {
                return View("Register", model);
            }
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
                UserManagerResult result =
                    await UserManager.FindAccountAndGetClaimAsync(model.Login, model.Password);
                if (result.Succeeded) {
                    Authentication(result.Claim);
                    return RedirectToAction("Index", "Home");
                }
                else {
                    foreach (string error in result.Errors) {
                        ModelState.AddModelError("", error);
                    }
                    return View("Login", model);
                }
            }
            else {
                return View("Login", model);
            }
        }

        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}