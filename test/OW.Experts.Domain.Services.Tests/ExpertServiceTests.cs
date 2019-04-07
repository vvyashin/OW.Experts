using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using OW.Experts.Test.Infrastructure;

namespace OW.Experts.Domain.Services.Tests
{
    [TestFixture]
    public class ExpertServiceTests
    {
        public IExpertRepository FakeExpertRepository { get; set; }
        public IAssociationRepository FakeAssociationRepository { get; set; }
        public IRelationRepository FakeRelationRepository { get; set; }
        public ITypeRepository<NotionType> FakeNotionTypeRepository { get; set; }
        public IRelationTypeRepository FakeRelationTypeRepository { get; set; }

        private ExpertService CreateServiceUnderTest()
        {
            FakeExpertRepository = Substitute.For<IExpertRepository>();
            FakeAssociationRepository = Substitute.For<IAssociationRepository>();
            FakeRelationRepository = Substitute.For<IRelationRepository>();
            FakeNotionTypeRepository = Substitute.For<ITypeRepository<NotionType>>();
            FakeRelationTypeRepository = Substitute.For<IRelationTypeRepository>();

            return new ExpertService(FakeExpertRepository, FakeNotionTypeRepository, 
                FakeRelationTypeRepository, FakeAssociationRepository, FakeRelationRepository);
        }

        private SessionOfExperts CreateSession()
        {
            var session = Substitute.For<SessionOfExperts>();
            
            return session;
        }

        private SessionOfExperts CreateSession(SessionPhase phase)
        {
            var session = Substitute.For<SessionOfExperts>();
            session.CurrentPhase.Returns(phase);

            return session;
        }

        private void FromExpertRepositoryReturnNullExpert()
        {
            FakeExpertRepository.GetExpertByNameAndSession(
                Arg.Any<GetExpertByNameAndSessionSpecification>())
                .Returns((Expert) null);
        }

        private Expert FromExpertRepositoryReturnFakeExpert(string expertName, SessionOfExperts session)
        {
            var expert = Substitute.For<Expert>();
            FakeExpertRepository.GetExpertByNameAndSession(
                Arg.Is<GetExpertByNameAndSessionSpecification>(
                    x => x.ExpertName == expertName && x.SessionOfExperts == session))
                    .Returns(expert);
            
            return expert;
        }

        private NotionType FromNotionTypeRepositoryReturnFakeType(Guid notionTypeId)
        {
            var notionType = Substitute.For<NotionType>();
            FakeNotionTypeRepository.GetById(Arg.Is(notionTypeId)).Returns(notionType);

            return notionType;
        }

        #region JoinSession

        [Test]
        public void JoinSession_NameIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.JoinSession(null, Substitute.For<SessionOfExperts>()));
        }

        [Test]
        public void JoinSession_SessionIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.JoinSession("testExpert", null));
        }
        
        [Test]
        public void JoinSession_ExpertRepositorySave()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = Substitute.For<SessionOfExperts>();
            
            serviceUnderTest.JoinSession("testExpert", session);
            
            FakeExpertRepository.Received(1).AddOrUpdate(Arg.Is<Expert>(
                e => e.Name == "testExpert" && e.SessionOfExperts == session));
        }

        #endregion

        #region Associations

        [Test]
        public void Associations_NameIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            var exception = (ArgumentNullException) Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.Associations(new List<string>(), null,
                    CreateSession()));
            
            Assert.AreEqual("expertName", exception.ParamName);
        }

        [Test]
        public void Associations_NotionsIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();

            var exception = (ArgumentNullException) Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.Associations(null, "testExpert", CreateSession()));
            
            Assert.AreEqual("notions", exception.ParamName);
        }

        [Test]
        public void Associations_SessionIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            var exception = (ArgumentNullException) Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.Associations(new List<string>(), "testExpert", null));
            
            Assert.AreEqual("sessionOfExperts", exception.ParamName);
        }

        [Test]
        public void Associations_ExpertDoesNotExist_InvalidOperationException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            FromExpertRepositoryReturnNullExpert();
            
            Assert.Throws<InvalidOperationException>(
                () => serviceUnderTest.Associations(new List<string>(), "expertName", CreateSession()));
        }

        [Test]
        public void Associations_ReplaceAllAssociationsOfExpert()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);
            var notions = new[] {"notion1", "notion2"};

            serviceUnderTest.Associations(notions, "expertName", session);

            expert.Received(1).ReplaceAllAssociations(Arg.Is(notions));
        }

        [Test]
        public void Associations_ExpertShouldBeSaved()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);

            serviceUnderTest.Associations(new List<string>(), "expertName", session);

            FakeExpertRepository.Received(1).AddOrUpdate(Arg.Is(expert));
        }

        #endregion

        #region AssociationsTypes

        [Test]
        public void AssociationsTypes_NameIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            var exception = (ArgumentNullException) Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.AssociationsTypes(new List<AssociationDto>(), null,
                    CreateSession()));
            
            Assert.AreEqual("expertName", exception.ParamName);
        }

        [Test]
        public void AssociationsTypes_AssociationsIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.AssociationsTypes(null, "testExpert", CreateSession()));
        }

        [Test]
        public void AssociationsTypes_SessionIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.AssociationsTypes(new List<AssociationDto>(), "testExpert", null));
        }

        [Test]
        public void AssociationsTypes_ExpertDoesNotExist_InvalidOperationException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            FromExpertRepositoryReturnNullExpert();
            
            Assert.Throws<InvalidOperationException>(
                () => serviceUnderTest.AssociationsTypes(new List<AssociationDto>(), "testExpert", CreateSession()));
        }

        [Test]
        public void AssociationsTypes_SetTypeForAssociationOfExpert()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);
            var notionTypeId = GeneratedGuids.Third;
            var notionType = FromNotionTypeRepositoryReturnFakeType(notionTypeId);
            var associations = new[]
            {
                new AssociationDto() {Id = GeneratedGuids.First, OfferType = "offer1", TypeId = notionTypeId},
                new AssociationDto() {Id = GeneratedGuids.Second, OfferType = "offer1", TypeId = notionTypeId}
            };

            serviceUnderTest.AssociationsTypes(associations, "expertName", session);

            expert.Received(1).SetTypeForAssociation(Arg.Is(associations[0].Id), 
                Arg.Is(notionType), Arg.Is(associations[0].OfferType));
            expert.Received(1).SetTypeForAssociation(Arg.Is(associations[1].Id),
                Arg.Is(notionType), Arg.Is(associations[1].OfferType));
        }

        [Test]
        public void AssociationTypes_ExpertShouldBeSaved()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);

            serviceUnderTest.AssociationsTypes(new List<AssociationDto>(), "expertName", session);

            FakeExpertRepository.Received(1).AddOrUpdate(Arg.Is(expert));
        }

        #endregion

        #region RelationTypes

        [Test]
        public void RelationTypes_NameIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.RelationTypes(new RelationTupleDto(), null, CreateSession()));
        }

        [Test]
        public void RelationTypes_RelationTupleIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.RelationTypes(null, "testExpert", CreateSession()));
        }

        [Test]
        public void RelationTypes_SessionIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.RelationTypes(new RelationTupleDto(), "testExpert", null));
        }

        [Test]
        public void RelationTypes_ExpertDoesNotExist_InvalidOperationException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            FromExpertRepositoryReturnNullExpert();
            
            Assert.Throws<InvalidOperationException>(
                () => serviceUnderTest.RelationTypes(new RelationTupleDto(), "testExpert", CreateSession()));
        }

        [Test]
        public void RelationTypes_IfRelationDoesNotExist_SetTypesWithEmptyCollectionOfTypes()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);
            var relationTuple = new RelationTupleDto()
            {
                StraightRelationId = GeneratedGuids.First,
                ReverseRelationId = GeneratedGuids.Second,
                DoesRelationExist = false
            };

            serviceUnderTest.RelationTypes(relationTuple, "expertName", session);

            expert.Received(1)
                .SetTypesForRelation(Arg.Is(relationTuple.StraightRelationId), 
                    Arg.Is<IReadOnlyCollection<RelationType>>(x => x.Count == 0), Arg.Is((string) null));
            expert.Received(1)
                .SetTypesForRelation(Arg.Is(relationTuple.ReverseRelationId),
                    Arg.Is<IReadOnlyCollection<RelationType>>(x => x.Count == 0), Arg.Is((string) null));
        }

        [Test]
        public void RelationTypes_IfRelationExists_SetTypesOfTheGeneralType()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);
            var generalType = Substitute.For<RelationType>();
            FakeRelationTypeRepository.GetGeneralType().Returns(generalType);
            var relationTuple = new RelationTupleDto()
            {
                StraightRelationId = GeneratedGuids.First,
                ReverseRelationId = GeneratedGuids.Second,
                DoesRelationExist = true
            };

            serviceUnderTest.RelationTypes(relationTuple, "expertName", session);

            expert.Received(1)
                .SetTypesForRelation(Arg.Is(relationTuple.StraightRelationId), 
                    Arg.Is<IReadOnlyCollection<RelationType>>(x => x.Contains(generalType)), Arg.Is((string)null));
            expert.Received(1)
                .SetTypesForRelation(Arg.Is(relationTuple.ReverseRelationId), 
                    Arg.Is<IReadOnlyCollection<RelationType>>(x => x.Contains(generalType)), Arg.Is((string)null));
        }

        [Test]
        public void RelationTypes_IfRelationExistsAndStraightIsTaxonym_SetTypesOfTheTaxonymType()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);
            var taxonomyType = Substitute.For<RelationType>();
            FakeRelationTypeRepository.GetTaxonomyType().Returns(taxonomyType);
            var relationTuple = new RelationTupleDto()
            {
                StraightRelationId = GeneratedGuids.First,
                DoesRelationExist = true,
                IsStraightTaxonym = true
            };

            serviceUnderTest.RelationTypes(relationTuple, "expertName", session);

            expert.Received(1)
                .SetTypesForRelation(Arg.Is(relationTuple.StraightRelationId),
                    Arg.Is<IReadOnlyCollection<RelationType>>(x => x.Contains(taxonomyType)), Arg.Is((string)null));
        }

        [Test]
        public void RelationTypes_IfRelationExistsAndReverseIsTaxonym_SetTypesOfTheTaxonymType()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);
            var taxonomyType = Substitute.For<RelationType>();
            FakeRelationTypeRepository.GetTaxonomyType().Returns(taxonomyType);
            var relationTuple = new RelationTupleDto()
            {
                ReverseRelationId = GeneratedGuids.First,
                DoesRelationExist = true,
                IsReverseTaxonym = true
            };

            serviceUnderTest.RelationTypes(relationTuple, "expertName", session);

            expert.Received(1)
                .SetTypesForRelation(Arg.Is(relationTuple.ReverseRelationId),
                    Arg.Is<IReadOnlyCollection<RelationType>>(x => x.Contains(taxonomyType)), Arg.Is((string)null));
        }

        [Test]
        public void RelationTypes_IfRelationExistsAndStraightIsMeronym_SetTypesOfTheMeronymType()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);
            var meronomyType = Substitute.For<RelationType>();
            FakeRelationTypeRepository.GetMeronomyType().Returns(meronomyType);
            var relationTuple = new RelationTupleDto()
            {
                StraightRelationId = GeneratedGuids.First,
                DoesRelationExist = true,
                IsStraightMeronym = true
            };

            serviceUnderTest.RelationTypes(relationTuple, "expertName", session);

            expert.Received(1)
                .SetTypesForRelation(Arg.Is(relationTuple.StraightRelationId),
                    Arg.Is<IReadOnlyCollection<RelationType>>(x => x.Contains(meronomyType)), Arg.Is((string)null));
        }

        [Test]
        public void RelationTypes_IfRelationExistsAndReverseIsMeronym_SetTypesOfTheMeronymType()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);
            var meronomyType = Substitute.For<RelationType>();
            FakeRelationTypeRepository.GetMeronomyType().Returns(meronomyType);
            var relationTuple = new RelationTupleDto()
            {
                ReverseRelationId = GeneratedGuids.First,
                DoesRelationExist = true,
                IsReverseMeronym = true
            };

            serviceUnderTest.RelationTypes(relationTuple, "expertName", session);

            expert.Received(1)
                .SetTypesForRelation(Arg.Is(relationTuple.ReverseRelationId),
                    Arg.Is<IReadOnlyCollection<RelationType>>(x => x.Contains(meronomyType)), Arg.Is((string)null));
        }

        [Test]
        public void RelationTypes_ExpertShouldBeSaved()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);

            serviceUnderTest.RelationTypes(new RelationTupleDto(), "expertName", session);

            FakeExpertRepository.Received(1).AddOrUpdate(Arg.Is(expert));
        }

        #endregion

        #region DoesExpertJoinSession

        [Test]
        public void DoesExpertJoinSession_NameIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.DoesExpertJoinSession(null, CreateSession()));
        }

        [Test]
        public void DoesExpertJoinSession_SessionIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.DoesExpertJoinSession("testExpert", null));
        }

        [Test]
        public void DoesExpertJoinSession_ExpertDoesNotExist_False()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            FromExpertRepositoryReturnNullExpert();
            
            var result = serviceUnderTest.DoesExpertJoinSession("testExpert", CreateSession());
            
            Assert.IsFalse(result);
        }

        [Test]
        public void DoesExpertJoinSession_ExpertExists_True()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            FromExpertRepositoryReturnFakeExpert("expertName", session);
            
            var result = serviceUnderTest.DoesExpertJoinSession("expertName", session);
            
            Assert.IsTrue(result);
        }

        #endregion

        #region DoesExpertCompleteCurrentPhase

        [Test]
        public void DoesExpertCompleteCurrentPhase_NameIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.DoesExpertCompleteCurrentPhase(null, CreateSession()));
        }

        [Test]
        public void DoesExpertCompleteCurrentPhase_SessionIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.DoesExpertCompleteCurrentPhase("testExpert", null));
        }

        [Test]
        public void DoesExpertCompleteCurrentPhase_ExpertDoesNotExist_False()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            FromExpertRepositoryReturnNullExpert();
            
            var result = serviceUnderTest.DoesExpertCompleteCurrentPhase("expertName", CreateSession());
            
            Assert.IsFalse(result);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void DoesExpertCompleteCurrentPhase_ReturnIsPhaseCompleted(bool isPhaseCompleted)
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession(SessionPhase.SelectingAndSpecifyingRelations);
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);
            expert.IsPhaseCompleted.Returns(isPhaseCompleted);
            
            var actualResult = serviceUnderTest.DoesExpertCompleteCurrentPhase("expertName", session);
            
            Assert.That(actualResult, Is.EqualTo(isPhaseCompleted));
        }

        #endregion

        #region GetAssociationsByExpertNameAndSession

        [Test]
        public void GetAssociationsByExpertNameAndSession_NameIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.GetAssociationsByExpertNameAndSession(null, CreateSession()));
        }

        [Test]
        public void GetAssociationsByExpertNameAndSession_NullSession_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.GetAssociationsByExpertNameAndSession("testExpert", null));
        }

        [Test]
        public void GetAssociationsByExpertNameAndSession_ExpertDoesNotExist_InvalidOperationException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            FromExpertRepositoryReturnNullExpert();
            var expertName = "testExpert";
            var sessionOfExperts = CreateSession();
            
            Assert.Throws<InvalidOperationException>(
                () => serviceUnderTest.GetAssociationsByExpertNameAndSession(expertName, sessionOfExperts));
        }

        [Test]
        public void GetAssociationsByExpertNameAndSession_ExpertExists_GetFromExpert()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);
            var associations = new List<Association>() {Substitute.For<Association>()};
            expert.Associations.Returns(associations);

            var actualAssociations = serviceUnderTest.GetAssociationsByExpertNameAndSession("expertName", session);

            Assert.That(actualAssociations, Is.EqualTo(associations));
        }

        #endregion

        #region GetNextRelationPairByExpertNameAndSession

        [Test]
        public void GetNextRelationPairByExpertNameAndSession_NameIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.GetNextRelationPairByExpertNameAndSession(null, CreateSession()));
        }

        [Test]
        public void GetNextRelationPairByExpertNameAndSession_SessionIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.GetNextRelationPairByExpertNameAndSession("testExpert", null));
        }

        [Test]
        public void GetNextRelationPairByExpertNameAndSession_ExpertDoesNotExist_InvalidOperationException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            FromExpertRepositoryReturnNullExpert();
            
            Assert.Throws<InvalidOperationException>(
                () => serviceUnderTest.GetNextRelationPairByExpertNameAndSession("expertName", CreateSession()));
        }

        [Test]
        public void GetNextRelationPairByExpertNameAndSession_ExpertExists_GetFromExpert()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert("expertName", session);
            var relationTuple = new Tuple<Relation, Relation>(Substitute.For<Relation>(), Substitute.For<Relation>());
            expert.GetNextRelationPair().Returns(relationTuple);

            var actualRelationTuple = serviceUnderTest.GetNextRelationPairByExpertNameAndSession("expertName", session);

            Assert.That(actualRelationTuple, Is.EqualTo(relationTuple));
        }

        #endregion

        #region GetNodeCandidatesBySession

        [Test]
        public void GetNodeCandidatesBySession_SessionIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.GetNodeCandidatesBySession(null));
        }

        [Test]
        public void GetNodeCandidatesBySession_SessionIsNotNull_ReturnFromRepository()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var sessionOfExperts = CreateSession();
            var nodeCandidateDtoList = new List<NodeCandidate>();
            FakeAssociationRepository.GetNodeCandidatesBySession(Arg.Is(sessionOfExperts))
                .Returns(nodeCandidateDtoList);
            
            var result = serviceUnderTest.GetNodeCandidatesBySession(sessionOfExperts);
            
            Assert.AreSame(nodeCandidateDtoList, result);
        }

        #endregion

        #region CreateRelations

        [Test]
        public void CreateRelations_SessionIsNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            var exception = (ArgumentNullException) Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.CreateRelations(null, new List<Node>()));
            
            Assert.AreEqual("sessionOfExperts", exception.ParamName);
        }

        [Test]
        public void CreateRelations_NodesAreNull_ArgumentNullException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws<ArgumentNullException>(
                () => serviceUnderTest.CreateRelations(CreateSession(), null));
        }


        [Test]
        public void CreateRelations_CreateForEachExpertsFromAllNodesExceptSessionNotion()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            session.BaseNotion.Returns("notionOfSession");
            var notionOfSessionNode = Substitute.For<Node>();
            notionOfSessionNode.Notion.Returns("notionOfSession");
            var nodes = new List<Node>() { notionOfSessionNode, Substitute.For<Node>(), Substitute.For<Node>() };
            var experts = new List<Expert>() { Substitute.For<Expert>(), Substitute.For<Expert>() };
            FakeExpertRepository.GetExpertsBySession(
                Arg.Is<GetExpertsBySessionSpecification>(s => s.SessionOfExperts == session)).Returns(experts);
            
            serviceUnderTest.CreateRelations(session, nodes);

            foreach (var expert in experts) {
                expert.Received(1).GenerateRelations(Arg.Is<IReadOnlyCollection<Node>>(nd =>
                    nd.Contains(nodes[1]) && nd.Contains(nodes[2])));
            }
        }

        [Test]
        public void CreateRelations_AllExpertsShouldBeSaved()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            var session = CreateSession();
            var experts = new List<Expert>() { Substitute.For<Expert>(), Substitute.For<Expert>() };
            FakeExpertRepository.GetExpertsBySession(
                Arg.Is<GetExpertsBySessionSpecification>(x => x.SessionOfExperts == session))
                .Returns(experts);
            var nodes = new List<Node>() { Substitute.For<Node>(), Substitute.For<Node>() };

            serviceUnderTest.CreateRelations(session, nodes);

            foreach (var expert in experts) {
                FakeExpertRepository.Received(1).AddOrUpdate(Arg.Is(expert));
            }
        }

        #endregion

        #region FinishCurrentPhase

        [Test]
        public void FinishCurrentPhase_ExpertDoesNotExist_InvalidOperationException()
        {
            ExpertService serviceUnderTest = CreateServiceUnderTest();
            FromExpertRepositoryReturnNullExpert();

            Assert.Throws<InvalidOperationException>(
                () => serviceUnderTest.FinishCurrentPhase("expertName", CreateSession()));
        }

        [Test]
        public void FinishCurrentPhase_FinishCurrentPhaseForExpert()
        {
            ExpertService serviceUnserTests = CreateServiceUnderTest();
            var expertName = "expertName";
            var session = CreateSession();
            var expert = FromExpertRepositoryReturnFakeExpert(expertName, session);

            serviceUnserTests.FinishCurrentPhase(expertName, session);

            expert.Received(1).FinishCurrentPhase();
        }
        
        #endregion
    }
}