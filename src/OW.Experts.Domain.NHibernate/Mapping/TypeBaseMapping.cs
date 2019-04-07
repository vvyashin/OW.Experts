namespace OW.Experts.Domain.NHibernate.Mapping
{
    public abstract class TypeBaseMapping<T> : DomainObjectMapping<T>
        where T : TypeBase
    {
        protected TypeBaseMapping()
        {
            Map(x => x.Name)
                .Not.Nullable()
                .Unique();
        }
    }
}
