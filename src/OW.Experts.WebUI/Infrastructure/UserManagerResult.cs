using System.Collections.Generic;
using System.Security.Claims;

namespace OW.Experts.WebUI.Infrastructure
{
    public class UserManagerResult
    {
        public static UserManagerResult Fail(string error)
        {
            return Fail(new string[] {error});
        }

        public static UserManagerResult Fail(IEnumerable<string> errors)
        {
            return new UserManagerResult
            {
                Succeeded = false,
                Errors = errors
            };
        }

        public static UserManagerResult Success(ClaimsIdentity claim)
        {
            return new UserManagerResult
            {
                Succeeded = true,
                Claim = claim
            };
        }

        public bool Succeeded { get; private set; }
        public IEnumerable<string> Errors { get; private set; }
        public ClaimsIdentity Claim { get; private set; } = null;
    }
}