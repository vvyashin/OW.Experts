using System.Collections.Generic;
using System.Web.Mvc;
using NSubstitute;
using NUnit.Framework;
using OW.Experts.Domain;
using OW.Experts.Domain.Infrastructure.Repository;
using OW.Experts.WebUI.Controllers;
using OW.Experts.WebUI.Infrastructure;
using OW.Experts.WebUI.Services;
using OW.Experts.WebUI.UnitTests.Base;
using OW.Experts.WebUI.ViewModels.Admin;

namespace OW.Experts.WebUI.UnitTests.ControllerTests
{
    [TestFixture]
    public class AdminControllerTests : SessionControllerTests
    {
        private LogService _fakeLogService;

        private IUnitOfWorkFactory _fakeUnitOfWorkFactory;

        private IAdminCurrentSessionService _fakeAdminCurrentSessionOfExpertsService;

        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        public void SelectNode_SessionExistsAndIsNotInRelationPhase_SaveErrorAndRedirectToCurrentSession(
            SessionPhase currentPhase)
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);

            var result = (RedirectToRouteResult)controllerUnderTest.SelectNode();

            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        public void SelectNodePost_SessionExistsAndIsNotInRelationPhase_SaveErrorAndRedirectToCurrentSession(
            SessionPhase currentPhase)
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);

            var result = (RedirectToRouteResult)controllerUnderTest.SelectNode(new NodeCandidateListViewModel());

            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void GetVerges_SessionExistsAndIsNotInRelationPhase_SaveErrorAndRedirectToCurrenSession(
            SessionPhase currentPhase)
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);

            var result = (RedirectToRouteResult)controllerUnderTest.SaveVerges();

            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void CurrentSession_SessionDoesNotExist_DisplayNewSessionView()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetNullCurrentSession();

            var result = (ViewResult)controllerUnderTest.CurrentSession();

            Assert.AreEqual("NewSession", result.ViewName);
        }

        [Test]
        public void CurrentSession_SessionExists_DisplayCurrentSessionView()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession();

            var result = (ViewResult)controllerUnderTest.CurrentSession();

            Assert.That(result.ViewName, Is.EqualTo("CurrentSession"));
            Assert.IsInstanceOf<SessionViewModel>(result.Model);
        }

        [Test]
        public void GetVerges_SessionDoesNotExist_ErrorMessageAndRedirectToCurrentSession()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetNullCurrentSession();

            var result = (RedirectToRouteResult)controllerUnderTest.SaveVerges();

            Assert.AreEqual(SessionNotExistErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void GetVerges_SessionExistAndIsInRelationPhase_DisplaySemanticNetworkView()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.SelectingAndSpecifyingRelations);

            var result = (RedirectToRouteResult)controllerUnderTest.SaveVerges();

            Assert.AreEqual("SemanticNetwork", result.RouteValues["action"]);
        }

        [Test]
        public void Index_Anyway_RedirectToCurrentSession()
        {
            var controllerUnderTest = CreateControllerUnderTest();

            var result = (RedirectToRouteResult)controllerUnderTest.Index();

            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void NewSession_SessionDoesNotExistAndModelIsNotValid_DisplayView()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            var sessionViewModel = new NewSessionViewModel { BaseNotion = "testNotion" };
            controllerUnderTest.ModelState.AddModelError("test", "test error");

            var result = (ViewResult)controllerUnderTest.NewSession(sessionViewModel);

            Assert.AreEqual("NewSession", result.ViewName);
            Assert.IsInstanceOf<NewSessionViewModel>(result.Model);
        }

        [Test]
        public void NewSession_SessionDoesNotExistAndModelIsValid_StartNewSessionAndRedirectToCurrentSession()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            var sessionViewModel = new NewSessionViewModel { BaseNotion = "baseNotion" };

            var result = (RedirectToRouteResult)controllerUnderTest.NewSession(sessionViewModel);

            _fakeAdminCurrentSessionOfExpertsService.Received().StartNewSession(Arg.Is("baseNotion"));
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void NewSession_SessionExists_SaveErrorAndRedirectToCurrentSession()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            var session = SetFakeCurrentSession();
            session.BaseNotion.Returns("baseNotion");

            var result = (RedirectToRouteResult)controllerUnderTest.NewSession(new NewSessionViewModel());

            _fakeAdminCurrentSessionOfExpertsService.DidNotReceive().StartNewSession(Arg.Is("baseNotion"));
            Assert.AreEqual(SessionExistErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void NextPhase_SessionDoesNotExist_SaveErrorAndRedirectToCurrentSession()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetNullCurrentSession();

            var result = (RedirectToRouteResult)controllerUnderTest.NextPhase();

            _fakeAdminCurrentSessionOfExpertsService.DidNotReceive().NextPhase();
            Assert.AreEqual(SessionNotExistErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void NextPhase_SessionExists_GoToNextPhaseAndRedirectToCurrentSession()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession();

            var result = (RedirectToRouteResult)controllerUnderTest.NextPhase();

            _fakeAdminCurrentSessionOfExpertsService.Received(1).NextPhase();
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void SelectNode_SessionDoesNotExist_SaveErrorAndRedirectToCurrentSession()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetNullCurrentSession();

            var result = (RedirectToRouteResult)controllerUnderTest.SelectNode();

            Assert.AreEqual(SessionNotExistErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void SelectNode_SessionExistsAndIsInRelationPhase_OutputNodeCandidate()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.SelectingNodes);

            var result = (ViewResult)controllerUnderTest.SelectNode();

            Assert.AreEqual("SelectNode", result.ViewName);
            Assert.IsInstanceOf<NodeCandidateListViewModel>(result.Model);
        }

        [Test]
        public void SelectNodePost_SessionDoesNotExist_SaveErrorAndRedirectToCurrentSession()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetNullCurrentSession();

            var result = (RedirectToRouteResult)controllerUnderTest.SelectNode(new NodeCandidateListViewModel());

            Assert.AreEqual(SessionNotExistErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void SelectNodePost_SessionExistsAndIsInRelationPhase_SaveSelectedNodesAndRedirectToCurrentSession()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.SelectingNodes);

            var result = (RedirectToRouteResult)controllerUnderTest.SelectNode(
                new NodeCandidateListViewModel
                {
                    NodeCandidates = new List<NodeCandidateViewModel>()
                });

            _fakeAdminCurrentSessionOfExpertsService.Received(1).CreateSemanticNetworkFromNodeCandidates(
                Arg.Any<IReadOnlyCollection<NodeCandidate>>());
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void SemanticNetwork_SessionDoesNotExist_SaveErrorAndRedirectToCurrentSession()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetNullCurrentSession();

            var result = (RedirectToRouteResult)controllerUnderTest.SemanticNetwork();

            Assert.AreEqual(SessionNotExistErrorMessage, controllerUnderTest.TempData[DataConstants.Error]);
            Assert.AreEqual("CurrentSession", result.RouteValues["action"]);
        }

        [Test]
        public void SemanticNetwork_SessionExists_DisplaySemanticNetworkView()
        {
            var controllerUnderTest = CreateControllerUnderTest();
            SetFakeCurrentSession();
            var semanticNetwork = new SemanticNetworkReadModel(new List<ConceptReadModel>());
            _fakeAdminCurrentSessionOfExpertsService.GetSemanticNetwork()
                .Returns(semanticNetwork);

            var result = (ViewResult)controllerUnderTest.SemanticNetwork();

            Assert.AreEqual("SemanticNetwork", result.ViewName);
            Assert.AreEqual(semanticNetwork, result.Model);
        }

        private AdminController CreateControllerUnderTest()
        {
            _fakeUnitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
            _fakeAdminCurrentSessionOfExpertsService = Substitute.For<IAdminCurrentSessionService>();
            FakeCurrentSessionOfExpertsService = _fakeAdminCurrentSessionOfExpertsService;
            _fakeLogService = Substitute.For<LogService>();
            return new AdminController(_fakeUnitOfWorkFactory, _fakeAdminCurrentSessionOfExpertsService, _fakeLogService);
        }
    }
}