using System;
using Domain;
using ExpressMapper;
using WebUI.ViewModels.Admin;

namespace WebUI.Infrastructure.AutoConverter.Rules
{
    public class NodeCandidateConvertRegister : IConvertRegister 
    {
        public void Register()
        {
            Mapper.Register<NodeCandidate, NodeCandidateViewModel>()
                .Ignore(x => x.ExpertPercent)
                .Member(x => x.TypeId, x => x.TypeId.ToString());

            Mapper.Register<NodeCandidateViewModel, NodeCandidate>()
                .Ignore(x => x.ExpertPercent)
                .Member(x => x.TypeId, x => Guid.Parse(x.TypeId));
        }
    }
}
