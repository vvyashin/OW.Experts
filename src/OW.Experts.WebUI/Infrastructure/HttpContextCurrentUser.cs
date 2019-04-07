using System.Web;

namespace OW.Experts.WebUI.Infrastructure
{
    public class HttpContextCurrentUser : ICurrentUser
    {
        public string Name => HttpContext.Current.User.Identity.Name;
        public bool IsAdmin => HttpContext.Current.User.IsInRole(RoleNames.Admin);
        public bool IsExpert => HttpContext.Current.User.IsInRole(RoleNames.Expert);
    }
}