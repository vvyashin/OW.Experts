using System.Web.Mvc;
using OW.Experts.WebUI.Infrastructure;

namespace OW.Experts.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private ICurrentUser _currentUser;

        public ICurrentUser CurrentAuthorizedUser
        {
            get { return _currentUser ?? (_currentUser = new HttpContextCurrentUser()); }
            set { if (_currentUser == null) _currentUser = value; }
        }

        public ActionResult Index()
        {
            if (CurrentAuthorizedUser.IsAdmin) {
                return RedirectToAction("Index", "Admin");
            }
            else if (CurrentAuthorizedUser.IsExpert) {
                return RedirectToAction("Index", "Expert");
            }
            else {
                return RedirectToAction("Register", "Account");
            }
        }
    }
}