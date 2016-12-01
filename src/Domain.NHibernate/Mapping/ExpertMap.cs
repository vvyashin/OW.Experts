using FluentNHibernate.Mapping;

namespace Domain.NHibernate.Mapping
{
    public class ExpertMap : DomainObjectMapping<Expert>
    {
        ExpertMap()
        {
            Map(x => x.LastCompletedPhase)
                .Nullable();
            Map(x => x.Name)
                .Not.Nullable();
            References(x => x.SessionOfExperts)
                .Not.Nullable()
                .Cascade.None()
                .LazyLoad(Laziness.Proxy);
            HasMany(x => x.Associations)
                .Cascade.AllDeleteOrphan().Inverse()
                .LazyLoad()
                .Access.CamelCaseField(Prefix.Underscore);
            HasMany(x => x.Relations)
                .Cascade.AllDeleteOrphan().Inverse()
                .LazyLoad()
                .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}
