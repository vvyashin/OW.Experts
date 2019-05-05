using ExpressMapper;
using OW.Experts.Domain;
using OW.Experts.WebUI.ViewModels.Admin;

namespace OW.Experts.WebUI.Infrastructure.AutoConverter.Rules
{
    public class SessionConvertRegister : IConvertRegister
    {
        public void Register()
        {
            Mapper.Register<SessionOfExperts, SessionViewModel>();
        }
    }
}