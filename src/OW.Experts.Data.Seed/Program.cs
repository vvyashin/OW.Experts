using OW.Experts.Domain;
using OW.Experts.Domain.NHibernate;

namespace OW.Experts.Data.Seed
{
    public static class Program
    {
        public static void Main()
        {
            var sessionFactory = OWDatabaseConfiguration.Configure("thread_static");
            var unitOfWorkFactory = new NHUnitOfWorkFactory(sessionFactory);

            using (var unitOfWork = unitOfWorkFactory.Create()) {
                var notionRepo = new NHRepository<NotionType>(sessionFactory);
                notionRepo.AddOrUpdate(new NotionType(Constants.NotionType));
                notionRepo.AddOrUpdate(new NotionType(Constants.ActionType));

                var relationRepo = new NHRepository<RelationType>(sessionFactory);
                relationRepo.AddOrUpdate(new RelationType(Constants.AssociationType));
                relationRepo.AddOrUpdate(new RelationType(Constants.TaxonomyType));
                relationRepo.AddOrUpdate(new RelationType(Constants.MeronomyType));

                unitOfWork.Commit();
            }
        }
    }
}