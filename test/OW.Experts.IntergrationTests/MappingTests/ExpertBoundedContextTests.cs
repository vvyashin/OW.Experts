using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using OW.Experts.Domain;

namespace OW.Experts.IntergrationTests.MappingTests
{
    [TestFixture]
    public class ExpertBoundedContextTests : DropCreateOnSetupTestFixture
    {
        [Test]
        public void GenerateRelationsAndSetTypes()
        {
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var sessionOfExperts = new SessionOfExperts("baseNotion");
                GetRepository<SessionOfExperts>().AddOrUpdate(sessionOfExperts);

                var notionType = new NotionType("type");
                GetRepository<NotionType>().AddOrUpdate(notionType);

                var expert = new Expert("expertName", sessionOfExperts);
                var nodes = new List<Node> { new Node("notion1", notionType), new Node("notion2", notionType) };
                foreach (var node in nodes) GetRepository<Node>().AddOrUpdate(node);
                expert.GenerateRelations(nodes);
                GetRepository<Expert>().AddOrUpdate(expert);

                unitOfWork.Commit();
            }

            RelationType type1, type2;
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var expert = LinqProvider.Query<Expert>().Single();

                type1 = new RelationType("type1");
                type2 = new RelationType("type2");
                var relationTypeRepo = GetRepository<RelationType>();
                relationTypeRepo.AddOrUpdate(type1);
                relationTypeRepo.AddOrUpdate(type2);

                expert.SetTypesForRelation(
                    expert.Relations.Single(x => x.Source.Notion == "notion1").Id,
                    new[] { type1, type2 },
                    "offer1");

                unitOfWork.Commit();
            }

            using (UnitOfWorkFactory.Create()) {
                var expert = LinqProvider.Query<Expert>().Single();
                var nodes = LinqProvider.Query<Node>().ToList();

                expert.Relations.Should().BeEquivalentTo(
                    new[]
                    {
                        new
                        {
                            Expert = expert, Source = nodes.Single(x => x.Notion == "notion1"),
                            Destination = nodes.Single(x => x.Notion == "notion2"), Types = new[] { type1, type2 },
                            OfferType = "offer1"
                        },
                        new
                        {
                            Expert = expert, Source = nodes.Single(x => x.Notion == "notion2"),
                            Destination = nodes.Single(x => x.Notion == "notion1"), Types = new RelationType[0],
                            OfferType = (string)null
                        }
                    },
                    opt => opt.ExcludingMissingMembers());
            }
        }

        [Test]
        public void NewExpert()
        {
            Expert expert;
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var sessionOfExperts = new SessionOfExperts("baseNotion");
                GetRepository<SessionOfExperts>().AddOrUpdate(sessionOfExperts);

                expert = new Expert("expertName", sessionOfExperts);
                GetRepository<Expert>().AddOrUpdate(expert);

                unitOfWork.Commit();
            }

            using (UnitOfWorkFactory.Create()) {
                var savedExpert = GetRepository<Expert>().GetById(expert.Id);

                savedExpert.Should().BeEquivalentTo(expert, opt => opt.ExcludingNestedObjects());
            }
        }

        [Test]
        public void ReplaceAssociation_InEmptyCollection()
        {
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var sessionOfExperts = new SessionOfExperts("baseNotion");
                GetRepository<SessionOfExperts>().AddOrUpdate(sessionOfExperts);

                var expert = new Expert("expertName", sessionOfExperts);
                GetRepository<Expert>().AddOrUpdate(expert);

                unitOfWork.Commit();
            }

            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var expert = LinqProvider.Query<Expert>().Single();
                expert.ReplaceAllAssociations(new[] { "notion1", "notion2" });
                GetRepository<Expert>().AddOrUpdate(expert);

                unitOfWork.Commit();
            }

            using (UnitOfWorkFactory.Create()) {
                var expert = LinqProvider.Query<Expert>().Single();

                expert.Associations.Should().BeEquivalentTo(
                    new[]
                    {
                        new { Expert = expert, Notion = "notion1" },
                        new { Expert = expert, Notion = "notion2" }
                    },
                    opt => opt.ExcludingMissingMembers());
            }
        }

        [Test]
        public void ReplaceAssociation_NotEmptyCollection()
        {
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var sessionOfExperts = new SessionOfExperts("baseNotion");
                GetRepository<SessionOfExperts>().AddOrUpdate(sessionOfExperts);

                var expert = new Expert("expertName", sessionOfExperts);
                expert.ReplaceAllAssociations(new[] { "notion1", "notion2" });
                GetRepository<Expert>().AddOrUpdate(expert);

                unitOfWork.Commit();
            }

            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var expert = LinqProvider.Query<Expert>().Single();
                expert.ReplaceAllAssociations(new[] { "notion3", "notion4" });
                GetRepository<Expert>().AddOrUpdate(expert);

                unitOfWork.Commit();
            }

            using (UnitOfWorkFactory.Create()) {
                var expert = LinqProvider.Query<Expert>().Single();

                expert.Associations.Should().BeEquivalentTo(
                    new[]
                    {
                        new { Expert = expert, Notion = "notion3" },
                        new { Expert = expert, Notion = "notion4" }
                    },
                    opt => opt.ExcludingMissingMembers());
            }
        }

        [Test]
        public void UpdateAssociationTypes()
        {
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var sessionOfExperts = new SessionOfExperts("baseNotion");
                GetRepository<SessionOfExperts>().AddOrUpdate(sessionOfExperts);

                var expert = new Expert("expertName", sessionOfExperts);
                expert.ReplaceAllAssociations(new[] { "notion1", "notion2" });

                GetRepository<Expert>().AddOrUpdate(expert);

                unitOfWork.Commit();
            }

            Guid associationIdForUpdate;
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var expert = LinqProvider.Query<Expert>().Single();
                associationIdForUpdate = LinqProvider.Query<Association>().First().Id;

                var type = new NotionType("NotionType");
                GetRepository<NotionType>().AddOrUpdate(type);

                expert.SetTypeForAssociation(associationIdForUpdate, type, "offer");
                GetRepository<Expert>().AddOrUpdate(expert);

                unitOfWork.Commit();
            }

            using (UnitOfWorkFactory.Create()) {
                var expert = LinqProvider.Query<Expert>().Single();
                var type = LinqProvider.Query<NotionType>().Single();

                expert.Associations.Single(x => x.Id == associationIdForUpdate).Should().BeEquivalentTo(
                    new { Expert = expert, Type = type, OfferType = "offer" },
                    opt => opt.ExcludingMissingMembers());
            }
        }

        [Test]
        public void UpdateLastCompletedPhase()
        {
            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var sessionOfExperts = new SessionOfExperts("baseNotion");
                GetRepository<SessionOfExperts>().AddOrUpdate(sessionOfExperts);

                var expert = new Expert("expertName", sessionOfExperts);
                GetRepository<Expert>().AddOrUpdate(expert);

                unitOfWork.Commit();
            }

            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                var expert = LinqProvider.Query<Expert>().Single();
                expert.LastCompletedPhase = SessionPhase.SpecifyingAssociationsTypes;

                unitOfWork.Commit();
            }

            using (UnitOfWorkFactory.Create()) {
                var expert = LinqProvider.Query<Expert>().Single();

                expert.LastCompletedPhase.Should().Be(SessionPhase.SpecifyingAssociationsTypes);
            }
        }
    }
}