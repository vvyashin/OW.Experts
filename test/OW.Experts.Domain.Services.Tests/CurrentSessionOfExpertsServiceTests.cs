using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using OW.Experts.Test.Infrastructure;

namespace OW.Experts.Domain.Services.Tests
{
    [TestFixture]
    public class CurrentSessionOfExpertsServiceTests
    {
        private const string SessionExistErrorMessage = "Текущая сессия уже существует";

        private const string SessionDoesNotExistErrorMessage = "Текущей сессии еще не существует";

        private const string SessionIsNotInPhaseErrorMessage = "На данном этапе тестирования недоступно";

        private ISessionOfExpertsRepository _fakeSessionOfExpertsRepository;

        private ExpertService _fakeExpertService;

        private SemanticNetworkService _fakeSemanticNetworkService;

        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void JoinSession_SessionExistsAndIsNotInAssociationPhase_ThrowInvalidOperationException(
            SessionPhase currentPhase)
        {
            var serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession(currentPhase);

            Assert.Throws(
                Is.TypeOf<InvalidOperationException>()
                    .And.Message.EqualTo(SessionIsNotInPhaseErrorMessage),
                () => serviceUnderTest.JoinSession("testExpert"));
        }

        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void Associations_SessionExistsAndIsNotInAssociationPhase_InvalidOperationException(
            SessionPhase currentPhase)
        {
            var serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession(currentPhase);

            Assert.Throws(
                Is.TypeOf<InvalidOperationException>()
                    .And.Message.EqualTo(SessionIsNotInPhaseErrorMessage),
                () => serviceUnderTest.Associations(new List<string>(), "testExpert"));
        }

        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void AssociationsTypes_SessionExistsAndIsNotInAssociationTypePhase_InvalidOperationException(
            SessionPhase currentPhase)
        {
            var serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession(currentPhase);

            Assert.Throws(
                Is.TypeOf<InvalidOperationException>()
                    .And.Message.EqualTo(SessionIsNotInPhaseErrorMessage),
                () => serviceUnderTest.AssociationsTypes(new List<AssociationDto>(), "testExpert"));
        }

        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void Relation_SessionExistsAndIsNotInRelationPhase_ThrowInvalidOperationException(
            SessionPhase currentPhase)
        {
            var serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession(currentPhase);

            Assert.Throws(
                Is.TypeOf<InvalidOperationException>()
                    .And.Message.EqualTo(SessionIsNotInPhaseErrorMessage),
                () => serviceUnderTest.Relations(new RelationTupleDto(), "testExpert"));
        }

        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        public void CreateSemanticNetworkFromNodeCandidates_SessionExistsAndIsNotInNodePhase_InvalidOperationException(
            SessionPhase currentPhase)
        {
            var serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession(currentPhase);

            Assert.Throws(
                Is.TypeOf<InvalidOperationException>()
                    .And.Message.EqualTo(SessionIsNotInPhaseErrorMessage),
                () => serviceUnderTest.CreateSemanticNetworkFromNodeCandidates(new List<NodeCandidate>()));
        }

        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void SaveRelationsAsVergesOfSemanticNetwork_SessionExistAndNotRelationPhase_InvalidOperationException(
            SessionPhase currentPhase)
        {
            var serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession(currentPhase);

            Assert.Throws(
                Is.TypeOf<InvalidOperationException>()
                    .And.Message.EqualTo(SessionIsNotInPhaseErrorMessage),
                () => serviceUnderTest.SaveRelationsAsVergesOfSemanticNetwork());
        }

        [Test]
        public void Associations_CurrentSessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(
                serviceUnderTest =>
                    serviceUnderTest.Associations(new List<string>(), "testExpert"));
        }

        [Test]
        public void Associations_NameIsNull_ThrowArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.Associations(new List<string>(), null));
        }

        [Test]
        public void Associations_NotionsIsNull_ThrowArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.Associations(null, "name"));
        }

        [Test]
        public void Associations_SessionExistsAndIsInAssociationPhase_CallExpertServiceAssociations()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var expertName = "expertTest";
            var notions = new List<string>();

            serviceUnderTest.Associations(notions, expertName);

            _fakeExpertService.Received(1).Associations(Arg.Is(notions), Arg.Is(expertName), Arg.Is(session));
        }

        [Test]
        public void AssociationsTypes_CurrentSessionDoesNotExist_NewInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(
                serviceUnderTest =>
                    serviceUnderTest.AssociationsTypes(new List<AssociationDto>(), "testExpert"));
        }

        [Test]
        public void AssociationsTypes_DtoIsNull_ThrowArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.AssociationsTypes(null, "name"));
        }

        [Test]
        public void AssociationsTypes_NameIsNull_ThrowArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.AssociationsTypes(new List<AssociationDto>(), null));
        }

        [Test]
        public void AssociationsTypes_SessionExistsAndIsInAssociationPhase_CallExpertServiceAssociationsTypes()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession(SessionPhase.SpecifyingAssociationsTypes);
            var expertName = "expertTest";
            var associations = new List<AssociationDto>();

            serviceUnderTest.AssociationsTypes(associations, expertName);

            _fakeExpertService.Received(1).AssociationsTypes(Arg.Is(associations), Arg.Is(expertName), Arg.Is(session));
        }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_CurrentSessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(
                serviceUnderTest =>
                    serviceUnderTest.CreateSemanticNetworkFromNodeCandidates(new List<NodeCandidate>()));
        }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_DtoIsNull_ThrowArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.CreateSemanticNetworkFromNodeCandidates(null));
        }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_SessionExistsAndIsInNodePhase_CallSemanticNetworkService()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession(SessionPhase.SelectingNodes);
            var nodeCandidates = new List<NodeCandidate>();

            serviceUnderTest.CreateSemanticNetworkFromNodeCandidates(nodeCandidates);

            _fakeSemanticNetworkService.Received(1)
                .CreateSemanticNetworkFromNodeCandidates(Arg.Is(nodeCandidates), Arg.Is(session));
        }

        [Test]
        public void DoesExpertCompleteCurrentPhase_CurrentSessionDoesNotExist_False()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            SetNullSession();

            var result = serviceUnderTest.DoesExpertCompleteCurrentPhase("expertTest");

            Assert.IsFalse(result);
        }

        [Test]
        public void DoesExpertCompleteCurrentPhase_CurrentSessionExistsAndExpertDoesNotFinishPhase_False()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var expertName = "expertTest";
            _fakeExpertService.DoesExpertCompleteCurrentPhase(Arg.Is(expertName), Arg.Is(session)).Returns(false);

            var result = serviceUnderTest.DoesExpertCompleteCurrentPhase(expertName);

            Assert.IsFalse(result);
        }

        [Test]
        public void DoesExpertCompleteCurrentPhase_CurrentSessionExistsAndExpertDoesNotFinishPhase_True()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var expertName = "expertTest";
            _fakeExpertService.DoesExpertCompleteCurrentPhase(Arg.Is(expertName), Arg.Is(session)).Returns(true);

            var result = serviceUnderTest.DoesExpertCompleteCurrentPhase(expertName);

            Assert.IsTrue(result);
        }

        [Test]
        public void DoesExpertCompleteCurrentPhase_NameIsNull_ThrowArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.DoesExpertCompleteCurrentPhase(null));
        }

        [Test]
        public void DoesExpertJoinSession_NameIsNull_ThrowArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.DoesExpertJoinSession(null));
        }

        [Test]
        public void DoesExpertJoinSession_SessionDoesNotExist_False()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            SetNullSession();

            var result = serviceUnderTest.DoesExpertJoinSession("testExpert");

            Assert.IsFalse(result);
        }

        [Test]
        public void DoesExpertJoinSession_SessionExistsAndExpertDoesNotJoinSession_False()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var sessionOfExperts = SetFakeSession();
            _fakeExpertService.DoesExpertJoinSession(Arg.Is("testExpert"), Arg.Is(sessionOfExperts)).Returns(false);

            var result = serviceUnderTest.DoesExpertJoinSession("testExpert");

            Assert.IsFalse(result);
        }

        [Test]
        public void DoesExpertJoinSession_SessionExistsAndExpertJoinSession_True()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var sessionOfExperts = SetFakeSession();
            _fakeExpertService.DoesExpertJoinSession(Arg.Is("testExpert"), Arg.Is(sessionOfExperts)).Returns(true);

            var result = serviceUnderTest.DoesExpertJoinSession("testExpert");

            Assert.IsTrue(result);
        }

        [Test]
        public void FinishCurrentPhase_CurrentSessionDoesNotExist_Throw()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTests => serviceUnderTests.FinishCurrentPhase("expertName"));
        }

        [Test]
        public void FinishCurrentPhase_CurrentSessionExists_CallExpertService()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var expertName = "expertTest";

            serviceUnderTest.FinishCurrentPhase(expertName);

            _fakeExpertService.Received(1).FinishCurrentPhase(Arg.Is(expertName), Arg.Is(session));
        }

        [Test]
        public void GetAllNodeCandidates_CurrentSessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.GetAllNodeCandidates());
        }

        [Test]
        public void GetAllNodeCandidates_SessionExists_GetFromExpertService()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var nodeCandidates = new List<NodeCandidate>();
            _fakeExpertService.GetNodeCandidatesBySession(Arg.Is(session)).Returns(nodeCandidates);

            var result = serviceUnderTest.GetAllNodeCandidates();

            Assert.AreSame(nodeCandidates, result);
        }

        [Test]
        public void GetAssociationsByExpertName_CurrentSessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(
                serviceUnderTest =>
                    serviceUnderTest.GetAssociationsByExpertName("expertTest"));
        }

        [Test]
        public void GetAssociationsByExpertName_NameIsNull_ThrowArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.GetAssociationsByExpertName(null));
        }

        [Test]
        public void GetAssociationsByExpertName_SessionExists_GetFromExpertService()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var expertName = "expertTest";

            var associations = new List<Association>();
            _fakeExpertService.GetAssociationsByExpertNameAndSession(Arg.Is(expertName), Arg.Is(session))
                .Returns(associations);

            var result = serviceUnderTest.GetAssociationsByExpertName(expertName);

            Assert.AreSame(associations, result);
        }

        [Test]
        public void GetExpertCount_CurrentSessionDoesNotExist_NewInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.GetExpertCount());
        }

        [Test]
        public void GetExpertCount_SessionExists_GetFromExpertService()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var count = 2;
            _fakeSessionOfExpertsRepository.GetExpertCount(Arg.Is(session)).Returns(count);

            var result = serviceUnderTest.GetExpertCount();

            Assert.AreEqual(count, result);
        }

        [Test]
        public void GetNextRelationByExpertName_CurrentSessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(
                serviceUnderTest =>
                    serviceUnderTest.GetNextRelationByExpertName("expertTest"));
        }

        [Test]
        public void GetNextRelationByExpertName_CurrentSessionExist_GetFromExpertService()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var expertName = "expertTest";

            var relationPair = new Tuple<Relation, Relation>(
                Substitute.For<Relation>(),
                Substitute.For<Relation>());
            _fakeExpertService.GetNextRelationPairByExpertNameAndSession(Arg.Is(expertName), Arg.Is(session))
                .Returns(relationPair);

            var result = serviceUnderTest.GetNextRelationByExpertName(expertName);

            Assert.AreSame(relationPair, result);
        }

        [Test]
        public void GetNextRelationByExpertName_NameIsNull_ThrowArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.GetNextRelationByExpertName(null));
        }

        [Test]
        public void GetSemanticNetwork_GetByCurrentSession()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var semanticNetworkStub = new SemanticNetworkReadModel(new List<ConceptReadModel>());
            _fakeSemanticNetworkService.GetSemanticNetworkBySession(Arg.Is(session))
                .Returns(semanticNetworkStub);

            var result = serviceUnderTest.GetSemanticNetwork();

            Assert.That(result, Is.EqualTo(semanticNetworkStub));
        }

        [Test]
        public void GetSematicNetwork_CurrentSessionDoesNotExist_NewInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.GetSemanticNetwork());
        }

        [Test]
        public void JoinSession_NameIsNull_ThrowArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.JoinSession(null));
        }

        [Test]
        public void JoinSession_SessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.JoinSession("testExpert"));
        }

        [Test]
        public void JoinSession_SessionExistsAndIsInAssociationPhase_CallExpertServiceJoinSession()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var expertName = "testExpert";
            _fakeExpertService.DoesExpertJoinSession(Arg.Is(expertName), Arg.Is(session));

            serviceUnderTest.JoinSession(expertName);

            _fakeExpertService.Received(1).JoinSession(Arg.Is(expertName), Arg.Is(session));
        }

        [Test]
        public void NextPhase_SessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.NextPhase());
        }

        [Test]
        public void NextPhase_SessionExists_NextPhase()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();

            serviceUnderTest.NextPhase();

            session.Received(1).NextPhaseOrFinish();
        }

        [Test]
        public void NextPhase_SessionExistsAndIsInSelectingNodePhase_CreateRelationBlanks()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = new SessionOfExperts("baseNotion");
            session.SetProperty(nameof(session.CurrentPhase), SessionPhase.SelectingNodes);
            _fakeSessionOfExpertsRepository.GetCurrent().Returns(session);

            serviceUnderTest.NextPhase();

            _fakeExpertService.Received(1).CreateRelations(
                Arg.Is(serviceUnderTest.CurrentSession),
                Arg.Any<IReadOnlyCollection<Node>>());
        }

        [Test]
        public void Relation_CurrentSessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(
                serviceUnderTest => serviceUnderTest.Relations(new RelationTupleDto(), "testExpert"));
        }

        [Test]
        public void Relation_DtoIsNull_ThrowArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.Relations(null, "name"));
        }

        [Test]
        public void Relation_NameIsNull_ThrowArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.Relations(new RelationTupleDto(), null));
        }

        [Test]
        public void Relation_SessionExistAndIsInAssociationPhase_CallExpertServiceRelation()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession(SessionPhase.SelectingAndSpecifyingRelations);
            var expertName = "expertTest";
            var relation = new RelationTupleDto();

            serviceUnderTest.Relations(relation, expertName);

            _fakeExpertService.Received(1).RelationTypes(Arg.Is(relation), Arg.Is(expertName), Arg.Is(session));
        }

        [Test]
        public void SaveRelationsAsVergesOfSemanticNetwork_CurrentSessionDoesNotExist_NewInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(
                serviceUnderTest => serviceUnderTest.SaveRelationsAsVergesOfSemanticNetwork());
        }

        [Test]
        public void SaveRelationsAsVergesOfSemanticNetwork_SessionExistAndRelationPhase_CallSemanticNetworkService()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession(SessionPhase.SelectingAndSpecifyingRelations);
            var groupedRelations = new List<GroupedRelation>();
            _fakeExpertService.GetGroupedRelations(Arg.Is(session)).Returns(groupedRelations);

            serviceUnderTest.SaveRelationsAsVergesOfSemanticNetwork();

            _fakeSemanticNetworkService.Received(1)
                .SaveRelationsAsVergesOfSemanticNetwork(Arg.Is(groupedRelations), Arg.Is(session));
        }

        [Test]
        public void StartNewSession_CurrentSessionDoesNotExist_AddSession()
        {
            var serviceUnderTest = CreateServiceUnderTest();
            SetNullSession();

            serviceUnderTest.StartNewSession("TestNotion");

            _fakeSessionOfExpertsRepository.AddOrUpdate(
                Arg.Is<SessionOfExperts>(
                    s => s.BaseNotion == "TestNotion"));
        }

        [Test]
        public void StartNewSession_CurrentSessionExists_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionExists(serviceUnderTest => serviceUnderTest.StartNewSession("TestNotion"));
        }

        [Test]
        public void StartNewSession_NotionIsNull_ThrowArgumentNullException()
        {
            var serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(
                Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.StartNewSession(null));
        }

        private void SetNullSession()
        {
            _fakeSessionOfExpertsRepository.GetCurrent().Returns((SessionOfExperts)null);
        }

        private SessionOfExperts SetFakeSession(
            SessionPhase sessionPhase = SessionPhase.MakingAssociations)
        {
            var session = Substitute.For<SessionOfExperts>();
            session.CurrentPhase.Returns(sessionPhase);

            _fakeSessionOfExpertsRepository.GetCurrent().Returns(session);

            return session;
        }

        private void ShouldThrowIfSessionExists(Action<CurrentSessionOfExpertsService> action)
        {
            var serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession();

            Assert.Throws(
                Is.TypeOf<InvalidOperationException>()
                    .And.Message.EqualTo(SessionExistErrorMessage),
                () => action(serviceUnderTest));
        }

        private void ShouldThrowIfSessionDoesNotExist(Action<CurrentSessionOfExpertsService> action)
        {
            var serviceUnderTest = CreateServiceUnderTest();
            SetNullSession();

            Assert.Throws(
                Is.TypeOf<InvalidOperationException>()
                    .And.Message.EqualTo(SessionDoesNotExistErrorMessage),
                () => action(serviceUnderTest));
        }

        private CurrentSessionOfExpertsService CreateServiceUnderTest()
        {
            _fakeSessionOfExpertsRepository = Substitute.For<ISessionOfExpertsRepository>();
            _fakeSemanticNetworkService = Substitute.For<SemanticNetworkService>();
            _fakeExpertService = Substitute.For<ExpertService>();

            return new CurrentSessionOfExpertsService(
                _fakeSessionOfExpertsRepository,
                _fakeExpertService,
                _fakeSemanticNetworkService);
        }
    }
}