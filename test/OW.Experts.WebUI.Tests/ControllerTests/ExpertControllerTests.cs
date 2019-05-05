using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NSubstitute;
using NUnit.Framework;
using OW.Experts.Domain;
using OW.Experts.Domain.Infrastructure.Repository;
using OW.Experts.WebUI.Constants;
using OW.Experts.WebUI.Controllers;
using OW.Experts.WebUI.Infrastructure;
using OW.Experts.WebUI.Services;
using OW.Experts.WebUI.UnitTests.Base;
using OW.Experts.WebUI.ViewModels;
using OW.Experts.WebUI.ViewModels.Admin;
using OW.Experts.WebUI.ViewModels.Expert;

namespace OW.Experts.WebUI.UnitTests.ControllerTests
{
    [TestFixture]
    public class ExpertControllerTests : SessionControllerTests
    {
        private LogService _fakeLogService;

        private IUnitOfWorkFactory _fakeUnitOfWorkFactory;

        private IExpertCurrentSessionService _fakeExpertCurrentSessionOfExpertsService;

        private IGetNotionTypesQuery<NotionTypeViewModel> _fakeTypeQuery;

        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void JoinExpert_SessionExistsAndIsNotInAssociationPhase_RedirectToExpertTest(SessionPhase currentPhase)
        {
            var cut = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);

            var result = (RedirectToRouteResult)cut.JoinSession();

            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void Association_SessionExistsAndIsNotInAssociationPhase_SaveErrorAndRedirectToExpertTest(
            SessionPhase currentPhase)
        {
            var cut = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);

            var result =
                (RedirectToRouteResult)cut.Association(new AllAssociationViewModel(), ViewConstants.SaveAction);

            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void AssociationType_SessionExistsAndIsNotInAssociationTypePhase_SaveErrorAndRedirectToExpertTest(
            SessionPhase currentPhase)
        {
            var cut = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);

            var result =
                (RedirectToRouteResult)cut.AssociationType(new AssociationTypeViewModel(), ViewConstants.SaveAction);

            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void Relation_SessionExistsAndIsNotInRelationPhase_SaveErrorAndRedirectToExpertTest(
            SessionPhase currentPhase)
        {
            var cut = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);

            var result = (RedirectToRouteResult)cut.Relation(new RelationViewModel());

            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void Association_ActionIsFinish_CallFinishCurrentPhase()
        {
            var cut = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.MakingAssociations);
            var stubUserName = "userName";
            cut.CurrentAuthorizedUser = CreateUser(stubUserName);

            cut.Association(new AllAssociationViewModel { Body = string.Empty }, ViewConstants.FinishAction);

            _fakeExpertCurrentSessionOfExpertsService.Received(1).FinishCurrentPhase(Arg.Is(stubUserName));
        }

        [Test]
        public void Association_SessionDoesNotExist_SaveErrorAndRedirectToExpertTest()
        {
            var cut = CreateControllerUnderTest();
            SetNullCurrentSession();

            var result =
                (RedirectToRouteResult)cut.Association(new AllAssociationViewModel(), ViewConstants.SaveAction);

            Assert.AreEqual(SessionNotExistErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void Association_SessionExistsAndIsInAssociationPhaseAndModelIsNotValid_DisplayAssocaitionView()
        {
            var cut = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.MakingAssociations);

            // set the ModelState.IsValid = false
            cut.ModelState.AddModelError("test", "test error");

            var result = (ViewResult)cut.Association(new AllAssociationViewModel(), ViewConstants.SaveAction);

            Assert.AreEqual("Association", result.ViewName);
        }

        [Test]
        public void
            Association_SessionExistsAndIsInAssociationPhaseAndModelIsValid_SaveAddedAssociationAndRedirectToExpertTest()
        {
            var cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();
            SetFakeCurrentSession(SessionPhase.MakingAssociations);

            var result =
                (RedirectToRouteResult)cut.Association(
                    new AllAssociationViewModel { Body = string.Empty },
                    ViewConstants.SaveAction);

            _fakeExpertCurrentSessionOfExpertsService.Received(1)
                .Associations(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<string>());
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void AssociationType_ActionIsFinish_CallFinishCurrentPhase()
        {
            var cut = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.SpecifyingAssociationsTypes);
            var stubUserName = "userName";
            cut.CurrentAuthorizedUser = CreateUser(stubUserName);

            cut.AssociationType(
                new AssociationTypeViewModel
                {
                    ExpertAssociations = new List<AssociationViewModel>()
                },
                ViewConstants.FinishAction);

            _fakeExpertCurrentSessionOfExpertsService.Received(1).FinishCurrentPhase(Arg.Is(stubUserName));
        }

        [Test]
        public void AssociationType_SessionDoesNotExist_SaveErrorAndRedirectToExpertTest()
        {
            var cut = CreateControllerUnderTest();
            SetNullCurrentSession();

            var result =
                (RedirectToRouteResult)cut.AssociationType(new AssociationTypeViewModel(), ViewConstants.SaveAction);

            Assert.AreEqual(SessionNotExistErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void AssociationType_SessionExistsAndIsInAssociationTypePhaseAndModelIsNotValid_ViewAssocaitionType()
        {
            var cut = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.SpecifyingAssociationsTypes);

            // set the ModelState.IsValid = false
            cut.ModelState.AddModelError("test", "test error");

            var result = (ViewResult)cut.AssociationType(new AssociationTypeViewModel(), ViewConstants.SaveAction);

            Assert.AreEqual("AssociationType", result.ViewName);
        }

        [Test]
        public void
            AssociationType_SessionExistsAndIsInAssociationTypePhaseAndModelIsValid_SaveAddedTypeAndRedirectToExpertTest()
        {
            var cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();
            SetFakeCurrentSession(SessionPhase.SpecifyingAssociationsTypes);

            var result = (RedirectToRouteResult)cut.AssociationType(
                new AssociationTypeViewModel
                {
                    ExpertAssociations = new List<AssociationViewModel>()
                },
                ViewConstants.SaveAction);

            _fakeExpertCurrentSessionOfExpertsService.Received(1)
                .AssociationsTypes(Arg.Any<IReadOnlyCollection<AssociationDto>>(), Arg.Any<string>());
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void ExpertTest_SessionDoesNotExist_DisplayNoSessionView()
        {
            var cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();
            SetNullCurrentSession();

            var result = (ViewResult)cut.ExpertTest();

            Assert.AreEqual("NoSession", result.ViewName);
        }

        [Test]
        public void ExpertTest_SessionExistsAndExpertDoesNotJoinSession_DisplayJoinSessionView()
        {
            var cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.MakingAssociations);
            _fakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(false);

            var result = (ViewResult)cut.ExpertTest();

            Assert.AreEqual("JoinSession", result.ViewName);
            Assert.IsInstanceOf<SessionViewModel>(result.Model);
        }

        [Test]
        public void
            ExpertTest_SessionExistsAndIsInAssociationPhaseAndExpertJoinsSessionAndExpertDoesNotFinishPhase_DisplayAssociationView()
        {
            var cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.MakingAssociations);

            _fakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            _fakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(false);

            var result = (ViewResult)cut.ExpertTest();

            Assert.AreEqual("Association", result.ViewName);
            Assert.IsInstanceOf<AllAssociationViewModel>(result.Model);
        }

        [Test]
        public void
            ExpertTest_SessionExistsAndIsInAssociationPhaseAndExpertJoinsSessionAndExpertFinishesPhase_DisplayWaitView()
        {
            var cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.MakingAssociations);

            _fakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            _fakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(true);

            var result = (ViewResult)cut.ExpertTest();

            Assert.AreEqual("Wait", result.ViewName);
        }

        [Test]
        public void
            ExpertTest_SessionExistsAndIsInAssociationTypePhaseAndExpertJoinsSessionAndExpertDoesNotFinishPhase_DisplayAssociationTypeView()
        {
            var cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.SpecifyingAssociationsTypes);

            _fakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            _fakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(false);

            var result = (ViewResult)cut.ExpertTest();

            Assert.AreEqual("AssociationType", result.ViewName);
            Assert.IsInstanceOf<AssociationTypeViewModel>(result.Model);
        }

        [Test]
        public void
            ExpertTest_SessionExistsAndIsInAssociationTypePhaseAndExpertJoinsSessionAndExpertFinishesPhase_DisplayWaitView()
        {
            var cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.SpecifyingAssociationsTypes);

            _fakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            _fakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(true);

            var result = (ViewResult)cut.ExpertTest();

            Assert.AreEqual("Wait", result.ViewName);
        }

        [Test]
        public void
            ExpertTest_SessionExistsAndIsInRelationPhaseAndExpertJoinsSessionAndAllRelationsWereChosen_DisplayEndView()
        {
            var cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.SelectingAndSpecifyingRelations);

            _fakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            _fakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(true);
            _fakeExpertCurrentSessionOfExpertsService.GetNextRelationByExpertName(Arg.Any<string>())
                .Returns((Tuple<Relation, Relation>)null);

            var result = (ViewResult)cut.ExpertTest();

            Assert.AreEqual("EndSession", result.ViewName);
        }

        [Test]
        public void
            ExpertTest_SessionExistsAndIsInRelationPhaseAndExpertJoinsSessionAndNotAllRelationsWereChosen_DisplayRelationView()
        {
            var cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.SelectingAndSpecifyingRelations);

            _fakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            _fakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(true);
            _fakeExpertCurrentSessionOfExpertsService.GetNextRelationByExpertName(Arg.Any<string>())
                .Returns(new Tuple<Relation, Relation>(Substitute.For<Relation>(), Substitute.For<Relation>()));

            var result = (ViewResult)cut.ExpertTest();

            Assert.AreEqual("Relation", result.ViewName);
            Assert.IsInstanceOf<RelationViewModel>(result.Model);
        }

        [Test]
        public void ExpertTest_SessionExistsAndIsInSelectingNodesPhaseAndExpertJoinsSession_DisplayWaitView()
        {
            var cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.MakingAssociations);

            _fakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            _fakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(true);

            var result = (ViewResult)cut.ExpertTest();

            Assert.AreEqual("Wait", result.ViewName);
        }

        [Test]
        public void Index_Anyway_RedirectToExpertTest()
        {
            var cut = CreateControllerUnderTest();

            var result = (RedirectToRouteResult)cut.Index();

            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void JoinExpert_SessionDoesNotExist_SaveErrorAndRedirectToExpertTest()
        {
            var cut = CreateControllerUnderTest();
            SetNullCurrentSession();

            var result = (RedirectToRouteResult)cut.JoinSession();
            Assert.AreEqual(SessionNotExistErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void JoinExpert_SessionExistsAndIsInAssociationPhase_JoinAndRedirectToExpertTest()
        {
            var cut = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.MakingAssociations);
            var stubUserName = "StubUserName";
            cut.CurrentAuthorizedUser = CreateUser(stubUserName);

            var result = (RedirectToRouteResult)cut.JoinSession();

            _fakeExpertCurrentSessionOfExpertsService.Received(1).JoinSession(Arg.Is(stubUserName));
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void Relation_SessionDoesNotExist_SaveErrorAndRedirectToExpertTest()
        {
            var cut = CreateControllerUnderTest();
            SetNullCurrentSession();

            var result = (RedirectToRouteResult)cut.Relation(new RelationViewModel());

            Assert.AreEqual(SessionNotExistErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void Relation_SessionExistsAndIsInRelationPhaseAndModelIsNotValid_ViewRelation()
        {
            var cut = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.SelectingAndSpecifyingRelations);

            // set the ModelState.IsValid = false
            cut.ModelState.AddModelError("test", "test error");

            var result = (ViewResult)cut.Relation(new RelationViewModel());

            Assert.AreEqual("Relation", result.ViewName);
        }

        [Test]
        public void Relation_SessionExistsAndIsInRelationPhaseAndModelIsValid_SaveRelationAndRedirectToExpertTest()
        {
            var cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();
            SetFakeCurrentSession(SessionPhase.SelectingAndSpecifyingRelations);

            var result = (RedirectToRouteResult)cut.Relation(new RelationViewModel());

            _fakeExpertCurrentSessionOfExpertsService.Received(1)
                .Relations(Arg.Any<RelationTupleDto>(), Arg.Any<string>());
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        private ExpertController CreateControllerUnderTest()
        {
            SubstituteAutoConverter.Substitute();
            _fakeLogService = Substitute.For<LogService>();
            _fakeTypeQuery = Substitute.For<IGetNotionTypesQuery<NotionTypeViewModel>>();
            _fakeUnitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
            _fakeExpertCurrentSessionOfExpertsService = Substitute.For<IExpertCurrentSessionService>();
            FakeCurrentSessionOfExpertsService = _fakeExpertCurrentSessionOfExpertsService;

            return new ExpertController(
                _fakeUnitOfWorkFactory,
                _fakeExpertCurrentSessionOfExpertsService,
                _fakeTypeQuery,
                _fakeLogService);
        }

        private ICurrentUser CreateUser(string userName)
        {
            var stubUser = Substitute.For<ICurrentUser>();
            stubUser.Name.Returns(userName);
            return stubUser;
        }
    }
}