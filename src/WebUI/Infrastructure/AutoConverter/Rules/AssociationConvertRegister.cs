using System;
using Domain;
using ExpressMapper;
using WebUI.ViewModels.Expert;

namespace WebUI.Infrastructure.AutoConverter.Rules
{
    public class AssociationConvertRegister : IConvertRegister
    {
        public void Register()
        {
            Mapper.Register<Association, AssociationViewModel>()
                .Member(x => x.Id, x => x.Id.ToString())
                .Member(x => x.TypeId, x => x.Type.Id.ToString());

            Mapper.Register<AssociationViewModel, AssociationDto>()
                .Member(x => x.Id, x => Guid.Parse(x.Id))
                .Member(x => x.TypeId, x => Guid.Parse(x.TypeId));
        }
    }
}
