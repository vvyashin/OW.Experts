namespace Domain.NHibernate.Mapping
{
    public class SessionOfExpertsMap : DomainObjectMapping<SessionOfExperts>
    {
        public SessionOfExpertsMap()
        {
            Map(x => x.BaseNotion)
                .Not.Nullable();
            Map(x => x.CurrentPhase)
                .Not.Nullable();
            Map(x => x.StartTime)
                .Not.Nullable();
        }
    }
}
