using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace OW.Experts.WebUI.Infrastructure
{
    public interface IUserManager : IDisposable
    {
        Task<UserManagerResult> CreateAccountAndGetClaimAsync(Account account, string password,
            string applicationCookie = DefaultAuthenticationTypes.ApplicationCookie);

        Task<UserManagerResult> FindAccountAndGetClaimAsync(string login, string password,
            string applicationCookie = DefaultAuthenticationTypes.ApplicationCookie);
    }
}