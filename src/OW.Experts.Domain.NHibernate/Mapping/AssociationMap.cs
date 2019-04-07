using FluentNHibernate.Mapping;

namespace Domain.NHibernate.Mapping
{
    class AssociationMap : DomainObjectMapping<Association>
    {
        public AssociationMap()
        {
            Map(x => x.Notion)
                .Not.Nullable();
            Map(x => x.OfferType)
                .Nullable();
            References(x => x.Expert)
                .Not.Nullable()
                .Cascade.None()
                .LazyLoad(Laziness.Proxy);
            References(x => x.Type)
                .Cascade.None()
                .LazyLoad(Laziness.Proxy);
        }
    }
}
