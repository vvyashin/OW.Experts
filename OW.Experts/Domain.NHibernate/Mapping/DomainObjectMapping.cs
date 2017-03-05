using Domain.Infrastructure;
using FluentNHibernate.Mapping;

namespace Domain.NHibernate.Mapping
{
    public abstract class DomainObjectMapping<T> : ClassMap<T> where T : DomainObject
    {
        protected DomainObjectMapping()
        {
            Id(x => x.Id)
                .Access.CamelCaseField(Prefix.Underscore)
                .GeneratedBy.GuidComb();
        } 
    }
}
