using FluentNHibernate.Mapping;

namespace OW.Experts.Domain.NHibernate.Mapping
{
    public class VergeMap : DomainObjectMapping<Verge>
    {
        public VergeMap()
        {
            Map(x => x.Weight)
                .Not.Nullable();
            References(x => x.SourceNode)
                .Column("SourceNodeId")
                .Not.Nullable()
                .Cascade.None()
                .LazyLoad(Laziness.Proxy);
            References(x => x.DestinationNode)
                .Column("DestinationNodeId")
                .Not.Nullable()
                .Cascade.None()
                .LazyLoad(Laziness.Proxy);
            References(x => x.Type)
                .Not.Nullable()
                .Cascade.None()
                .LazyLoad(Laziness.Proxy);
            HasMany(x => x.SessionWeightSlices)
                .Cascade.AllDeleteOrphan().Inverse()
                .LazyLoad()
                .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}