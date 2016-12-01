using System.Collections.Generic;
using System.Web.Mvc;
using Domain;
using Domain.Infrastructure;
using NSubstitute;
using NUnit.Framework;
using WebUI.Controllers;
using WebUI.Infrastructure;
using WebUI.Services;
using WebUI.ViewModels.Admin;

namespace WebUI.UnitTests
{
    [TestFixture]
    public class AdminControllerTests : SessionControllerTests
    {
        private LogService FakeLogService { get; set; }

        public IUnitOfWorkFactory FakeUnitOfWorkFactory { get; set; }

        public IAdminCurrentSessionService FakeAdminCurrentSessionOfExpertsService { get; set; }

        private AdminController CreateControllerUnderTest()
        {
            FakeUnitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
            FakeAdminCurrentSessionOfExpertsService = Substitute.For<IAdminCurrentSessionService>();
            FakeCurrentSessionOfExpertsService = FakeAdminCurrentSessionOfExpertsService;
            FakeLogService = Substitute.For<LogService>();
            return new AdminController(FakeUnitOfWorkFactory, FakeAdminCurrentSessionOfExpertsService, FakeLogService);
        }

        #region Index

        [Test]
        public void Index_Anyway_RedirectToCurrentSession()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();

            var result = (RedirectToRouteResult)controllerUnderTest.Index();

            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        #endregion

        #region CurrentSession

        [Test]
        public void CurrentSession_SessionExists_DisplayCurrentSessionView()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession();
            
            var result = (ViewResult)controllerUnderTest.CurrentSession();
            
            Assert.That(result.ViewName, Is.EqualTo("CurrentSession"));
            Assert.IsInstanceOf<SessionViewModel>(result.Model);
        }

        [Test]
        public void CurrentSession_SessionDoesNotExist_DisplayNewSessionView()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetNullCurrentSession();
            
            var result = (ViewResult)controllerUnderTest.CurrentSession();
            
            Assert.AreEqual("NewSession", result.ViewName);
        }

        #endregion

        #region NewSession

        [Test]
        public void NewSession_SessionExists_SaveErrorAndRedirectToCurrentSession()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            var session = SetFakeCurrentSession();
            session.BaseNotion.Returns("baseNotion");
            
            var result = (RedirectToRouteResult)controllerUnderTest.NewSession(new NewSessionViewModel());
            
            FakeAdminCurrentSessionOfExpertsService.DidNotReceive().StartNewSession(Arg.Is("baseNotion"));
            Assert.AreEqual(SessionExistErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void NewSession_SessionDoesNotExistAndModelIsValid_StartNewSessionAndRedirectToCurrentSession()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            var sessionViewModel = new NewSessionViewModel() { BaseNotion = "baseNotion" };
            
            var result = (RedirectToRouteResult)controllerUnderTest.NewSession(sessionViewModel);

            FakeAdminCurrentSessionOfExpertsService.Received().StartNewSession(Arg.Is("baseNotion"));
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void NewSession_SessionDoesNotExistAndModelIsNotValid_DisplayView()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            var sessionViewModel = new NewSessionViewModel() { BaseNotion = "testNotion" };
            controllerUnderTest.ModelState.AddModelError("test", "test error");
            
            var result = (ViewResult)controllerUnderTest.NewSession(sessionViewModel);
            
            Assert.AreEqual("NewSession", result.ViewName);
            Assert.IsInstanceOf<NewSessionViewModel>(result.Model);
        }

        #endregion

        #region NextPhase

        [Test]
        public void NextPhase_SessionDoesNotExist_SaveErrorAndRedirectToCurrentSession()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetNullCurrentSession();
            
            var result = (RedirectToRouteResult)controllerUnderTest.NextPhase();

            FakeAdminCurrentSessionOfExpertsService.DidNotReceive().NextPhase();
            Assert.AreEqual(SessionNotExistErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void NextPhase_SessionExists_GoToNextPhaseAndRedirectToCurrentSession()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession();
            
            var result = (RedirectToRouteResult)controllerUnderTest.NextPhase();
            
            FakeAdminCurrentSessionOfExpertsService.Received(1).NextPhase();
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        #endregion

        #region SelectNode

        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        public void SelectNode_SessionExistsAndIsNotInRelationPhase_SaveErrorAndRedirectToCurrentSession(
            SessionPhase currentPhase)
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);
            
            var result = (RedirectToRouteResult)controllerUnderTest.SelectNode();
            
            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void SelectNode_SessionDoesNotExist_SaveErrorAndRedirectToCurrentSession()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetNullCurrentSession();
            
            var result = (RedirectToRouteResult)controllerUnderTest.SelectNode();
            
            Assert.AreEqual(SessionNotExistErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void SelectNode_SessionExistsAndIsInRelationPhase_OutputNodeCandidate()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.SelectingNodes);
            
            var result = (ViewResult)controllerUnderTest.SelectNode();
            
            Assert.AreEqual("SelectNode", result.ViewName);
            Assert.IsInstanceOf<NodeCandidateListViewModel>(result.Model);
        }

        #endregion

        #region SelectNodePost

        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        public void SelectNodePost_SessionExistsAndIsNotInRelationPhase_SaveErrorAndRedirectToCurrentSession(
            SessionPhase currentPhase)
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);
            
            var result = (RedirectToRouteResult)controllerUnderTest.SelectNode(new NodeCandidateListViewModel());
            
            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void SelectNodePost_SessionDoesNotExist_SaveErrorAndRedirectToCurrentSession()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetNullCurrentSession();
            
            var result = (RedirectToRouteResult)controllerUnderTest.SelectNode(new NodeCandidateListViewModel());
            
            Assert.AreEqual(SessionNotExistErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void SelectNodePost_SessionExistsAndIsInRelationPhase_SaveSelectedNodesAndRedirectToCurrentSession()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.SelectingNodes);
            
            var result = (RedirectToRouteResult)controllerUnderTest.SelectNode(new NodeCandidateListViewModel()
            {
                NodeCandidates = new List<NodeCandidateViewModel>()
            });
            
            FakeAdminCurrentSessionOfExpertsService.Received(1).CreateSemanticNetworkFromNodeCandidates(
                Arg.Any<IReadOnlyCollection<NodeCandidate>>());
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        #endregion

        #region SemanticNetwork

        [Test]
        public void SemanticNetwork_SessionDoesNotExist_SaveErrorAndRedirectToCurrentSession()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetNullCurrentSession();
            
            var result = (RedirectToRouteResult)controllerUnderTest.SemanticNetwork();
            
            Assert.AreEqual(SessionNotExistErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void SemanticNetwork_SessionExists_DisplaySemanticNetworkView()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession();
            var semanticNetwork = new SemanticNetworkReadModel(new List<ConceptReadModel>());
            FakeAdminCurrentSessionOfExpertsService.GetSemanticNetwork()
                .Returns(semanticNetwork);
            
            var result = (ViewResult)controllerUnderTest.SemanticNetwork();
            
            Assert.AreEqual("SemanticNetwork", result.ViewName);
            Assert.AreEqual(semanticNetwork, result.Model);
        }

        #endregion

        #region GetVerges

        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void GetVerges_SessionExistsAndIsNotInRelationPhase_SaveErrorAndRedirectToCurrenSession(
            SessionPhase currentPhase)
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);
            
            var result = (RedirectToRouteResult)controllerUnderTest.SaveVerges();

            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void GetVerges_SessionDoesNotExist_ErrorMessageAndRedirectToCurrentSession()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetNullCurrentSession();
            
            var result = (RedirectToRouteResult)controllerUnderTest.SaveVerges();
            
            Assert.AreEqual(SessionNotExistErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void GetVerges_SessionExistAndIsInRelationPhase_DisplaySemanticNetworkView()
        {
            AdminController controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.SelectingAndSpecifyingRelations);
            
            var result = (RedirectToRouteResult)controllerUnderTest.SaveVerges();
            
            Assert.AreEqual("SemanticNetwork", result.RouteValues["action"]);
        }

        #endregion
    }
}