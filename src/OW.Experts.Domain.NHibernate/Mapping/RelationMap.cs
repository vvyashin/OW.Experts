using FluentNHibernate.Mapping;

namespace OW.Experts.Domain.NHibernate.Mapping
{
    public class RelationMap : DomainObjectMapping<Relation>
    {
        public RelationMap()
        {
            Map(x => x.IsChosen)
                .Not.Nullable();
            Map(x => x.OfferType)
                .Nullable();
            References(x => x.Expert)
                .Not.Nullable()
                .Cascade.None()
                .LazyLoad(Laziness.Proxy);
            References(x => x.Source)
                .Not.Nullable()
                .Cascade.None()
                .LazyLoad(Laziness.Proxy);
            References(x => x.Destination)
                .Not.Nullable()
                .Cascade.None()
                .LazyLoad(Laziness.Proxy);
            HasManyToMany(x => x.Types)
                .Cascade.All()
                .LazyLoad()
                .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}
