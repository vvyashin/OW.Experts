using System;
using ExpressMapper;
using OW.Experts.Domain;
using OW.Experts.WebUI.ViewModels.Expert;

namespace OW.Experts.WebUI.Infrastructure.AutoConverter.Rules
{
    public class RelationConvertRegister : IConvertRegister
    {
        public void Register()
        {
            Mapper.Register<Tuple<Relation, Relation>, RelationViewModel>()
                .Member(x => x.StraightRelationid, x => x.Item1.Id.ToString())
                .Member(x => x.ReverseRelationId, x => x.Item2.Id.ToString())
                .Member(x => x.FirstNodeType, x => x.Item1.Source.Type.Name)
                .Member(x => x.SecondNodeType, x => x.Item1.Destination.Type.Name)
                .Member(x => x.FirstNodeNotion, x => x.Item1.Source.Notion)
                .Member(x => x.SecondNodeNotion, x => x.Item1.Destination.Notion);

            Mapper.Register<RelationViewModel, RelationTupleDto>()
                .Member(x => x.StraightRelationId, x => Guid.Parse(x.StraightRelationid))
                .Member(x => x.ReverseRelationId, x => Guid.Parse(x.ReverseRelationId))
                .Member(x => x.DoesRelationExist, x => x.DoesRelationExist == "yes");
        }
    }
}
