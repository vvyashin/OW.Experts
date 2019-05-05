using FluentNHibernate.Mapping;

namespace OW.Experts.Domain.NHibernate.Mapping
{
    public class NodeMap : DomainObjectMapping<Node>
    {
        public NodeMap()
        {
            Map(x => x.Notion)
                .Not.Nullable();
            References(x => x.Type)
                .Not.Nullable()
                .Cascade.None()
                .LazyLoad(Laziness.Proxy);
            HasManyToMany(x => x.SessionsOfExperts)
                .Cascade.All()
                .LazyLoad()
                .Access.CamelCaseField(Prefix.Underscore);
            HasMany(x => x.IngoingVerges)
                .KeyColumn("DestinationNodeId")
                .Cascade.None()
                .LazyLoad()
                .Access.CamelCaseField(Prefix.Underscore)
                .Inverse();
            HasMany(x => x.OutgoingVerges)
                .KeyColumn("SourceNodeId")
                .Cascade.None()
                .LazyLoad()
                .Access.CamelCaseField(Prefix.Underscore)
                .Inverse();
        }
    }
}