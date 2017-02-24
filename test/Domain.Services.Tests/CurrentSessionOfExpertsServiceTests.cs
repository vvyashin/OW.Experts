using System;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Test.Infrastructure;

namespace Domain.Services.Tests
{
    [TestFixture]
    public class CurrentSessionOfExpertsServiceTests
    {
        private ISessionOfExpertsRepository FakeSessionOfExpertsRepository { get; set; }
        private ExpertService FakeExpertService { get; set; }
        private SemanticNetworkService FakeSemanticNetworkService { get; set; }

        private const string SessionExistErrorMessage = "Текущая сессия уже существует";
        private const string SessionDoesNotExistErrorMessage = "Текущей сессии еще не существует";
        private const string SessionIsNotInPhaseErrorMessage = "На данном этапе тестирования недоступно";

        private CurrentSessionOfExpertsService CreateServiceUnderTest()
        {
            FakeSessionOfExpertsRepository = Substitute.For<ISessionOfExpertsRepository>();
            FakeSemanticNetworkService = Substitute.For<SemanticNetworkService>();
            FakeExpertService = Substitute.For<ExpertService>();
            
            return new CurrentSessionOfExpertsService(FakeSessionOfExpertsRepository, FakeExpertService, FakeSemanticNetworkService);
        }

        private void SetNullSession()
        {
            FakeSessionOfExpertsRepository.GetCurrent().Returns((SessionOfExperts)null);
        }

        private SessionOfExperts SetFakeSession(
            SessionPhase sessionPhase = SessionPhase.MakingAssociations)
        {
            var session = Substitute.For<SessionOfExperts>();
            session.CurrentPhase.Returns(sessionPhase);
            
            FakeSessionOfExpertsRepository.GetCurrent().Returns(session);

            return session;
        }

        private void ShouldThrowIfSessionExists(Action<CurrentSessionOfExpertsService> action)
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession();

            Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo(SessionExistErrorMessage),
                () => action(serviceUnderTest));
        }

        private void ShouldThrowIfSessionDoesNotExist(Action<CurrentSessionOfExpertsService> action)
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            SetNullSession();

            Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo(SessionDoesNotExistErrorMessage),
                () => action(serviceUnderTest));
        }
        
        #region StartNewSession

        [Test]
        public void StartNewSession_NotionIsNull_ThrowArgumentNullException()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.StartNewSession(null));
        }

        [Test]
        public void StartNewSession_CurrentSessionExists_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionExists(serviceUnderTest => serviceUnderTest.StartNewSession("TestNotion"));
        }

        [Test]
        public void StartNewSession_CurrentSessionDoesNotExist_AddSession()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            SetNullSession();
            
            serviceUnderTest.StartNewSession("TestNotion");
            
            FakeSessionOfExpertsRepository.AddOrUpdate(Arg.Is<SessionOfExperts>(
                s => s.BaseNotion == "TestNotion"));
        }
        
        #endregion

        #region NextPhase

        [Test]
        public void NextPhase_SessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.NextPhase());
        }
        
        [Test]
        public void NextPhase_SessionExists_NextPhase()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            
            serviceUnderTest.NextPhase();

            session.Received(1).NextPhaseOrFinish();
        }

        [Test]
        public void NextPhase_SessionExistsAndIsInSelectingNodePhase_CreateRelationBlanks()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = new SessionOfExperts("baseNotion");
            session.SetProperty(nameof(session.CurrentPhase), SessionPhase.SelectingNodes);
            FakeSessionOfExpertsRepository.GetCurrent().Returns(session);
            
            serviceUnderTest.NextPhase();
            
            FakeExpertService.Received(1).CreateRelations(
                Arg.Is(serviceUnderTest.CurrentSession), Arg.Any<IReadOnlyCollection<Node>>());
        }

        #endregion

        #region DoesExpertJoinSession

        [Test]
        public void DoesExpertJoinSession_NameIsNull_ThrowArgumentNullException()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.DoesExpertJoinSession(null));
        }

        [Test]
        public void DoesExpertJoinSession_SessionDoesNotExist_False()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            SetNullSession();
            
            var result = serviceUnderTest.DoesExpertJoinSession("testExpert");
            
            Assert.IsFalse(result);
        }

        [Test]
        public void DoesExpertJoinSession_SessionExistsAndExpertDoesNotJoinSession_False()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var sessionOfExperts = SetFakeSession();
            FakeExpertService.DoesExpertJoinSession(Arg.Is("testExpert"), Arg.Is(sessionOfExperts)).Returns(false);
            
            var result = serviceUnderTest.DoesExpertJoinSession("testExpert");
            
            Assert.IsFalse(result);
        }

        [Test]
        public void DoesExpertJoinSession_SessionExistsAndExpertJoinSession_True()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var sessionOfExperts = SetFakeSession();
            FakeExpertService.DoesExpertJoinSession(Arg.Is("testExpert"), Arg.Is(sessionOfExperts)).Returns(true);

            var result = serviceUnderTest.DoesExpertJoinSession("testExpert");
            
            Assert.IsTrue(result);
        }

        #endregion

        #region JoinSession

        [Test]
        public void JoinSession_NameIsNull_ThrowArgumentNullException()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.JoinSession(null));
        }

        [Test]
        public void JoinSession_SessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.JoinSession("testExpert"));
        }

        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void JoinSession_SessionExistsAndIsNotInAssociationPhase_ThrowInvalidOperationException(
            SessionPhase currentPhase)
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession(currentPhase);
            
            Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo(SessionIsNotInPhaseErrorMessage),
                () => serviceUnderTest.JoinSession("testExpert"));
        }

        [Test]
        public void JoinSession_SessionExistsAndIsInAssociationPhase_CallExpertServiceJoinSession()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            SessionOfExperts session = SetFakeSession(SessionPhase.MakingAssociations);
            string expertName = "testExpert";
            FakeExpertService.DoesExpertJoinSession(Arg.Is(expertName), Arg.Is(session));
            
            serviceUnderTest.JoinSession(expertName);
            
            FakeExpertService.Received(1).JoinSession(Arg.Is(expertName), Arg.Is(session));
        }

        #endregion

        #region Associations

        [Test]
        public void Associations_NameIsNull_ThrowArgumentNullException()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.Associations(new List<string>(), null));
        }

        [Test]
        public void Associations_NotionsIsNull_ThrowArgumentNullException()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.Associations(null, "name"));
        }

        [Test]
        public void Associations_CurrentSessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.Associations(new List<string>(), "testExpert"));
        }

        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void Associations_SessionExistsAndIsNotInAssociationPhase_InvalidOperationException(
            SessionPhase currentPhase)
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession(currentPhase);
            
            Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo(SessionIsNotInPhaseErrorMessage),
                () => serviceUnderTest.Associations(new List<string>(), "testExpert"));
        }

        [Test]
        public void Associations_SessionExistsAndIsInAssociationPhase_CallExpertServiceAssociations()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession(SessionPhase.MakingAssociations);
            string expertName = "expertTest";
            var notions = new List<string>();

            serviceUnderTest.Associations(notions, expertName);
            
            FakeExpertService.Received(1).Associations(Arg.Is(notions), Arg.Is(expertName), Arg.Is(session));
        }

        #endregion

        #region AssociationsTypes

        [Test]
        public void AssociationsTypes_NameIsNull_ThrowArgumentNullException()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.AssociationsTypes(new List<AssociationDto>(), null));
        }

        [Test]
        public void AssociationsTypes_DtoIsNull_ThrowArgumentNullException()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.AssociationsTypes(null, "name"));
        }

        [Test]
        public void AssociationsTypes_CurrentSessionDoesNotExist_NewInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.AssociationsTypes(new List<AssociationDto>(), "testExpert"));
        }

        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void AssociationsTypes_SessionExistsAndIsNotInAssociationTypePhase_InvalidOperationException(
            SessionPhase currentPhase)
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession(currentPhase);
            
            Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo(SessionIsNotInPhaseErrorMessage),
                () => serviceUnderTest.AssociationsTypes(new List<AssociationDto>(), "testExpert"));
        }

        [Test]
        public void AssociationsTypes_SessionExistsAndIsInAssociationPhase_CallExpertServiceAssociationsTypes()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession(SessionPhase.SpecifyingAssociationsTypes);
            string expertName = "expertTest";
            var associations = new List<AssociationDto>();

            serviceUnderTest.AssociationsTypes(associations, expertName);
            
            FakeExpertService.Received(1).AssociationsTypes(Arg.Is(associations), Arg.Is(expertName), Arg.Is(session));
        }

        #endregion

        #region Relation

        [Test]
        public void Relation_NameIsNull_ThrowArgumentNullException()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.Relations(new RelationTupleDto(), null));
        }

        [Test]
        public void Relation_DtoIsNull_ThrowArgumentNullException()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.Relations(null, "name"));
        }

        [Test]
        public void Relation_CurrentSessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(
                serviceUnderTest => serviceUnderTest.Relations(new RelationTupleDto(), "testExpert"));
        }

        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void Relation_SessionExistsAndIsNotInRelationPhase_ThrowInvalidOperationException(SessionPhase currentPhase)
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession(currentPhase);
            
            Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo(SessionIsNotInPhaseErrorMessage),
                () => serviceUnderTest.Relations(new RelationTupleDto(), "testExpert"));
        }

        [Test]
        public void Relation_SessionExistAndIsInAssociationPhase_CallExpertServiceRelation()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession(SessionPhase.SelectingAndSpecifyingRelations);
            string expertName = "expertTest";
            var relation = new RelationTupleDto();
            
            serviceUnderTest.Relations(relation, expertName);
            
            FakeExpertService.Received(1).RelationTypes(Arg.Is(relation), Arg.Is(expertName), Arg.Is(session));
        }

        #endregion

        #region GetAllNodeCandidates

        [Test]
        public void GetAllNodeCandidates_CurrentSessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.GetAllNodeCandidates());
        }

        [Test]
        public void GetAllNodeCandidates_SessionExists_GetFromExpertService()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var nodeCandidates = new List<NodeCandidate>();
            FakeExpertService.GetNodeCandidatesBySession(Arg.Is(session)).Returns(nodeCandidates);
            
            var result = serviceUnderTest.GetAllNodeCandidates();
            
            Assert.AreSame(nodeCandidates, result);
        }

        #endregion

        #region GetExpertCount

        [Test]
        public void GetExpertCount_CurrentSessionDoesNotExist_NewInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.GetExpertCount());
        }

        [Test]
        public void GetExpertCount_SessionExists_GetFromExpertService()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var count = 2;
            FakeSessionOfExpertsRepository.GetExpertCount(Arg.Is(session)).Returns(count);
            
            var result = serviceUnderTest.GetExpertCount();
            
            Assert.AreEqual(count, result);
        }

        #endregion

        #region DoesExpertCompleteCurrentPhase

        [Test]
        public void DoesExpertCompleteCurrentPhase_NameIsNull_ThrowArgumentNullException()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.DoesExpertCompleteCurrentPhase(null));
        }

        [Test]
        public void DoesExpertCompleteCurrentPhase_CurrentSessionDoesNotExist_False()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            SetNullSession();
            
            var result = serviceUnderTest.DoesExpertCompleteCurrentPhase("expertTest");
            
            Assert.IsFalse(result);
        }

        [Test]
        public void DoesExpertCompleteCurrentPhase_CurrentSessionExistsAndExpertDoesNotFinishPhase_False()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            string expertName = "expertTest";
            FakeExpertService.DoesExpertCompleteCurrentPhase(Arg.Is(expertName), Arg.Is(session)).Returns(false);
            
            var result = serviceUnderTest.DoesExpertCompleteCurrentPhase(expertName);
            
            Assert.IsFalse(result);
        }

        [Test]
        public void DoesExpertCompleteCurrentPhase_CurrentSessionExistsAndExpertDoesNotFinishPhase_True()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            string expertName = "expertTest";
            FakeExpertService.DoesExpertCompleteCurrentPhase(Arg.Is(expertName), Arg.Is(session)).Returns(true);
            
            var result = serviceUnderTest.DoesExpertCompleteCurrentPhase(expertName);
            
            Assert.IsTrue(result);
        }

        #endregion

        #region GetAssociationsByExpertName

        [Test]
        public void GetAssociationsByExpertName_NameIsNull_ThrowArgumentNullException()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.GetAssociationsByExpertName(null));
        }

        [Test]
        public void GetAssociationsByExpertName_CurrentSessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.GetAssociationsByExpertName("expertTest"));
        }

        [Test]
        public void GetAssociationsByExpertName_SessionExists_GetFromExpertService()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var expertName = "expertTest";

            var associations = new List<Association>();
            FakeExpertService.GetAssociationsByExpertNameAndSession(Arg.Is(expertName), Arg.Is(session))
                .Returns(associations);
            
            var result = serviceUnderTest.GetAssociationsByExpertName(expertName);
            
            Assert.AreSame(associations, result);
        }

        #endregion

        #region GetNextRelationByExpertName

        [Test]
        public void GetNextRelationByExpertName_NameIsNull_ThrowArgumentNullException()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            
            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.GetNextRelationByExpertName(null));
        }

        [Test]
        public void GetNextRelationByExpertName_CurrentSessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.GetNextRelationByExpertName("expertTest"));
        }

        [Test]
        public void GetNextRelationByExpertName_CurrentSessionExist_GetFromExpertService()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var expertName = "expertTest";

            var relationPair = new Tuple<Relation, Relation>(
                Substitute.For<Relation>(), Substitute.For<Relation>());
            FakeExpertService.GetNextRelationPairByExpertNameAndSession(Arg.Is(expertName), Arg.Is(session))
                .Returns(relationPair);
            
            var result = serviceUnderTest.GetNextRelationByExpertName(expertName);
            
            Assert.AreSame(relationPair, result);
        }

        #endregion

        #region CreateSemanticNetworkFromNodeCandidates

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_DtoIsNull_ThrowArgumentNullException()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();

            Assert.Throws(Is.TypeOf<ArgumentNullException>(),
                () => serviceUnderTest.CreateSemanticNetworkFromNodeCandidates(null));
        }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_CurrentSessionDoesNotExist_ThrowInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(
                serviceUnderTest => serviceUnderTest.CreateSemanticNetworkFromNodeCandidates(new List<NodeCandidate>()));
        }

        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        public void CreateSemanticNetworkFromNodeCandidates_SessionExistsAndIsNotInNodePhase_InvalidOperationException(
            SessionPhase currentPhase)
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession(currentPhase);
            
            Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo(SessionIsNotInPhaseErrorMessage),
                () => serviceUnderTest.CreateSemanticNetworkFromNodeCandidates(new List<NodeCandidate>()));
        }

        [Test]
        public void CreateSemanticNetworkFromNodeCandidates_SessionExistsAndIsInNodePhase_CallSemanticNetworkService()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession(SessionPhase.SelectingNodes);
            var nodeCandidates = new List<NodeCandidate>();

            serviceUnderTest.CreateSemanticNetworkFromNodeCandidates(nodeCandidates);
            
            FakeSemanticNetworkService.Received(1)
                .CreateSemanticNetworkFromNodeCandidates(Arg.Is(nodeCandidates), Arg.Is(session));
        }

        #endregion

        #region SaveRelationsAsVergesOfSemanticNetwork

        [Test]
        public void SaveRelationsAsVergesOfSemanticNetwork_CurrentSessionDoesNotExist_NewInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(
                serviceUnderTest => serviceUnderTest.SaveRelationsAsVergesOfSemanticNetwork());
        }

        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void SaveRelationsAsVergesOfSemanticNetwork_SessionExistAndNotRelationPhase_InvalidOperationException(
            SessionPhase currentPhase)
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            SetFakeSession(currentPhase);

            Assert.Throws(Is.TypeOf<InvalidOperationException>()
                .And.Message.EqualTo(SessionIsNotInPhaseErrorMessage),
                () => serviceUnderTest.SaveRelationsAsVergesOfSemanticNetwork());
        }

        [Test]
        public void SaveRelationsAsVergesOfSemanticNetwork_SessionExistAndRelationPhase_CallSemanticNetworkService()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession(SessionPhase.SelectingAndSpecifyingRelations);
            var groupedRelations = new List<GroupedRelation>();
            FakeExpertService.GetGroupedRelations(Arg.Is(session)).Returns(groupedRelations);
            
            serviceUnderTest.SaveRelationsAsVergesOfSemanticNetwork();
            
            FakeSemanticNetworkService.Received(1)
                .SaveRelationsAsVergesOfSemanticNetwork(Arg.Is(groupedRelations), Arg.Is(session));
        }

        #endregion

        #region GetSematicNetwork

        [Test]
        public void GetSematicNetwork_CurrentSessionDoesNotExist_NewInvalidOperationException()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTest => serviceUnderTest.GetSemanticNetwork());
        }

        [Test]
        public void GetSemanticNetwork_GetByCurrentSession()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            var semanticNetworkStub = new SemanticNetworkReadModel(new List<ConceptReadModel>());
            FakeSemanticNetworkService.GetSemanticNetworkBySession(Arg.Is(session))
                .Returns(semanticNetworkStub);

            var result = serviceUnderTest.GetSemanticNetwork();

            Assert.That(result, Is.EqualTo(semanticNetworkStub));
        }

        #endregion

        #region FinishCurrentPhase

        [Test]
        public void FinishCurrentPhase_CurrentSessionDoesNotExist_Throw()
        {
            ShouldThrowIfSessionDoesNotExist(serviceUnderTests => serviceUnderTests.FinishCurrentPhase("expertName"));
        }

        [Test]
        public void FinishCurrentPhase_CurrentSessionExists_CallExpertService()
        {
            CurrentSessionOfExpertsService serviceUnderTest = CreateServiceUnderTest();
            var session = SetFakeSession();
            string expertName = "expertTest";
            
            serviceUnderTest.FinishCurrentPhase(expertName);

            FakeExpertService.Received(1).FinishCurrentPhase(Arg.Is(expertName), Arg.Is(session));
        }

        #endregion
    }
}