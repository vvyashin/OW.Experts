using Domain;
using ExpressMapper;
using WebUI.ViewModels.Admin;

namespace WebUI.Infrastructure.AutoConverter.Rules
{
    public class SessionConvertRegister : IConvertRegister
    {
        public void Register()
        {
            Mapper.Register<SessionOfExperts, SessionViewModel>();
        }
    }
}
