using System.Web.Mvc;
using OW.Experts.WebUI.Infrastructure;

namespace OW.Experts.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private ICurrentUser _currentUser;

        public ICurrentUser CurrentAuthorizedUser
        {
            get => _currentUser ?? (_currentUser = new HttpContextCurrentUser());
            set
            {
                if (_currentUser == null) _currentUser = value;
            }
        }

        public ActionResult Index()
        {
            if (CurrentAuthorizedUser.IsAdmin)
                return RedirectToAction("Index", "Admin");
            if (CurrentAuthorizedUser.IsExpert)
                return RedirectToAction("Index", "Expert");
            return RedirectToAction("Register", "Account");
        }
    }
}