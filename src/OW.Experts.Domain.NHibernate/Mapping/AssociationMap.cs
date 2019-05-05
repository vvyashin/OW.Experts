using FluentNHibernate.Mapping;

namespace OW.Experts.Domain.NHibernate.Mapping
{
    internal class AssociationMap : DomainObjectMapping<Association>
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