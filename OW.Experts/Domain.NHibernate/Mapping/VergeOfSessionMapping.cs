namespace Domain.NHibernate.Mapping
{
    public class VergeOfSessionMapping : DomainObjectMapping<VergeOfSession>
    {
        public VergeOfSessionMapping()
        {
            Map(x => x.Weight)
                .Not.Nullable();
            References(x => x.Verge)
                .Not.Nullable()
                .Cascade.None();
            References(x => x.SessionOfExperts)
                .Not.Nullable()
                .Cascade.None();
        }
    }
}
