using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Domain;
using Domain.Infrastructure;
using NSubstitute;
using NUnit.Framework;
using WebUI.Constants;
using WebUI.Controllers;
using WebUI.Infrastructure;
using WebUI.Services;
using WebUI.ViewModels;
using WebUI.ViewModels.Admin;
using WebUI.ViewModels.Expert;

namespace WebUI.UnitTests
{
    [TestFixture]
    public class ExpertControllerTests : SessionControllerTests
    {
        // private ICurrentSessionOfExpertsService FakeCurrentSessionOfExpertsService { get; set; }
        // on base class
        private LogService FakeLogService { get; set; }

        private ICurrentUser CreateUser(string userName)
        {
            ICurrentUser stubUser = Substitute.For<ICurrentUser>();
            stubUser.Name.Returns(userName);
            return stubUser;
        }

        public IUnitOfWorkFactory FakeUnitOfWorkFactory { get; set; }

        public IExpertCurrentSessionService FakeExpertCurrentSessionOfExpertsService { get; set; }

        public IGetNotionTypesQuery<NotionTypeViewModel> FakeTypeQuery { get; set; }

        private ExpertController CreateControllerUnderTest()
        {
            SubstituteAutoConverter.Substitute();
            FakeLogService = Substitute.For<LogService>();
            FakeTypeQuery = Substitute.For<IGetNotionTypesQuery<NotionTypeViewModel>>();
            FakeUnitOfWorkFactory = Substitute.For<IUnitOfWorkFactory>();
            FakeExpertCurrentSessionOfExpertsService = Substitute.For<IExpertCurrentSessionService>();
            FakeCurrentSessionOfExpertsService = FakeExpertCurrentSessionOfExpertsService;

            return new ExpertController(FakeUnitOfWorkFactory, FakeExpertCurrentSessionOfExpertsService,
                FakeTypeQuery, FakeLogService);
        }

        #region Index

        [Test]
        public void Index_Anyway_RedirectToExpertTest()
        {
            ExpertController cut = CreateControllerUnderTest();
            
            var result = (RedirectToRouteResult)cut.Index();
            
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        #endregion
        
        #region JoinExpert

        [Test]
        public void JoinExpert_SessionDoesNotExist_SaveErrorAndRedirectToExpertTest()
        {
            ExpertController cut = CreateControllerUnderTest();
            SetNullCurrentSession();
            
            var result = (RedirectToRouteResult)cut.JoinSession();
            Assert.AreEqual(SessionNotExistErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void JoinExpert_SessionExistsAndIsNotInAssociationPhase_RedirectToExpertTest(SessionPhase currentPhase)
        {
            ExpertController cut = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);
            
            var result = (RedirectToRouteResult)cut.JoinSession();
            
            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void JoinExpert_SessionExistsAndIsInAssociationPhase_JoinAndRedirectToExpertTest()
        {
            ExpertController cut = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.MakingAssociations);
            string stubUserName = "StubUserName";
            cut.CurrentAuthorizedUser = CreateUser(stubUserName);
            
            var result = (RedirectToRouteResult)cut.JoinSession();
            
            FakeExpertCurrentSessionOfExpertsService.Received(1).JoinSession(Arg.Is(stubUserName));
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }
        #endregion

        #region ExpertTest

        [Test]
        public void ExpertTest_SessionDoesNotExist_DisplayNoSessionView()
        {
            ExpertController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();
            SetNullCurrentSession();
            
            var result = (ViewResult)cut.ExpertTest();
            
            Assert.AreEqual("NoSession", result.ViewName);
        }

        [Test]
        public void ExpertTest_SessionExistsAndExpertDoesNotJoinSession_DisplayJoinSessionView()
        {
            ExpertController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.MakingAssociations);
            FakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(false);
            
            var result = (ViewResult)cut.ExpertTest();
            
            Assert.AreEqual("JoinSession", result.ViewName);
            Assert.IsInstanceOf<SessionViewModel>(result.Model);
        }

        [Test]
        public void
            ExpertTest_SessionExistsAndIsInAssociationPhaseAndExpertJoinsSessionAndExpertFinishesPhase_DisplayWaitView()
        {
            ExpertController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.MakingAssociations);

            FakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            FakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(true);
            
            var result = (ViewResult)cut.ExpertTest();
            
            Assert.AreEqual("Wait", result.ViewName);
        }

        [Test]
        public void ExpertTest_SessionExistsAndIsInAssociationPhaseAndExpertJoinsSessionAndExpertDoesNotFinishPhase_DisplayAssociationView()
        {
            ExpertController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.MakingAssociations);

            FakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            FakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(false);
            
            var result = (ViewResult)cut.ExpertTest();
            
            Assert.AreEqual("Association", result.ViewName);
            Assert.IsInstanceOf<AllAssociationViewModel>(result.Model);
        }

        [Test]
        public void ExpertTest_SessionExistsAndIsInAssociationTypePhaseAndExpertJoinsSessionAndExpertFinishesPhase_DisplayWaitView()
        {
            ExpertController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.SpecifyingAssociationsTypes);

            FakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            FakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(true);

            var result = (ViewResult)cut.ExpertTest();

            Assert.AreEqual("Wait", result.ViewName);
        }

        [Test]
        public void ExpertTest_SessionExistsAndIsInAssociationTypePhaseAndExpertJoinsSessionAndExpertDoesNotFinishPhase_DisplayAssociationTypeView()
        {
            ExpertController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.SpecifyingAssociationsTypes);

            FakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            FakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(false);
            
            var result = (ViewResult)cut.ExpertTest();
            
            Assert.AreEqual("AssociationType", result.ViewName);
            Assert.IsInstanceOf<AssociationTypeViewModel>(result.Model);
        }

        [Test]
        public void ExpertTest_SessionExistsAndIsInSelectingNodesPhaseAndExpertJoinsSession_DisplayWaitView()
        {
            ExpertController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.MakingAssociations);

            FakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            FakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(true);
            
            var result = (ViewResult)cut.ExpertTest();
            
            Assert.AreEqual("Wait", result.ViewName);
        }

        [Test]
        public void
            ExpertTest_SessionExistsAndIsInRelationPhaseAndExpertJoinsSessionAndAllRelationsWereChosen_DisplayEndView()
        {
            ExpertController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.SelectingAndSpecifyingRelations);

            FakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            FakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(true);
            FakeExpertCurrentSessionOfExpertsService.GetNextRelationByExpertName(Arg.Any<string>())
                .Returns((Tuple<Relation, Relation>)null);
            
            var result = (ViewResult)cut.ExpertTest();
            
            Assert.AreEqual("EndSession", result.ViewName);
        }

        [Test]
        public void ExpertTest_SessionExistsAndIsInRelationPhaseAndExpertJoinsSessionAndNotAllRelationsWereChosen_DisplayRelationView()
        {
            ExpertController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();

            SetFakeCurrentSession(SessionPhase.SelectingAndSpecifyingRelations);

            FakeExpertCurrentSessionOfExpertsService.DoesExpertJoinSession(Arg.Any<string>()).Returns(true);
            FakeExpertCurrentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(Arg.Any<string>()).Returns(true);
            FakeExpertCurrentSessionOfExpertsService.GetNextRelationByExpertName(Arg.Any<string>())
                .Returns(new Tuple<Relation, Relation>(Substitute.For<Relation>(), Substitute.For<Relation>()));

            var result = (ViewResult)cut.ExpertTest();
            
            Assert.AreEqual("Relation", result.ViewName);
            Assert.IsInstanceOf<RelationViewModel>(result.Model);
        }

        #endregion

        #region AssociationHttpPost

        [Test]
        public void Association_SessionDoesNotExist_SaveErrorAndRedirectToExpertTest()
        {
            ExpertController cut = CreateControllerUnderTest();
            SetNullCurrentSession();
            
            var result = (RedirectToRouteResult)cut.Association(new AllAssociationViewModel(), ViewConstants.SaveAction);
            
            Assert.AreEqual(SessionNotExistErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void Association_SessionExistsAndIsNotInAssociationPhase_SaveErrorAndRedirectToExpertTest(
            SessionPhase currentPhase)
        {
            ExpertController cut = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);
            
            var result = (RedirectToRouteResult)cut.Association(new AllAssociationViewModel(), ViewConstants.SaveAction);
            
            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void Association_SessionExistsAndIsInAssociationPhaseAndModelIsNotValid_DisplayAssocaitionView()
        {
            ExpertController cut = CreateControllerUnderTest();
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
            ExpertController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();
            SetFakeCurrentSession(SessionPhase.MakingAssociations);
            
            var result = (RedirectToRouteResult)cut.Association(new AllAssociationViewModel() {Body = ""}, ViewConstants.SaveAction);

            FakeExpertCurrentSessionOfExpertsService.Received(1)
                .Associations(Arg.Any<IReadOnlyCollection<string>>(), Arg.Any<string>());
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void Association_ActionIsFinish_CallFinishCurrentPhase()
        {
            ExpertController cut = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.MakingAssociations);
            var stubUserName = "userName";
            cut.CurrentAuthorizedUser = CreateUser(stubUserName);

            cut.Association(new AllAssociationViewModel() { Body = "" }, ViewConstants.FinishAction);

            FakeExpertCurrentSessionOfExpertsService.Received(1).FinishCurrentPhase(Arg.Is(stubUserName));
        }

        #endregion

        #region AssociationTypeHttpPost

        [Test]
        public void AssociationType_SessionDoesNotExist_SaveErrorAndRedirectToExpertTest()
        {
            ExpertController cut = CreateControllerUnderTest();
            SetNullCurrentSession();
            
            var result = (RedirectToRouteResult)cut.AssociationType(new AssociationTypeViewModel(), ViewConstants.SaveAction);
            
            Assert.AreEqual(SessionNotExistErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [TestCase(SessionPhase.SelectingAndSpecifyingRelations)]
        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void AssociationType_SessionExistsAndIsNotInAssociationTypePhase_SaveErrorAndRedirectToExpertTest(
            SessionPhase currentPhase)
        {
            ExpertController cut = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);
            
            var result = (RedirectToRouteResult)cut.AssociationType(new AssociationTypeViewModel(), ViewConstants.SaveAction);
            
            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void AssociationType_SessionExistsAndIsInAssociationTypePhaseAndModelIsNotValid_ViewAssocaitionType()
        {
            ExpertController cut = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.SpecifyingAssociationsTypes);
            // set the ModelState.IsValid = false
            cut.ModelState.AddModelError("test", "test error");
            
            var result = (ViewResult)cut.AssociationType(new AssociationTypeViewModel(), ViewConstants.SaveAction);
            
            Assert.AreEqual("AssociationType", result.ViewName);
        }

        [Test]
        public void AssociationType_SessionExistsAndIsInAssociationTypePhaseAndModelIsValid_SaveAddedTypeAndRedirectToExpertTest()
        {
            ExpertController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();
            SetFakeCurrentSession(SessionPhase.SpecifyingAssociationsTypes);

            var result = (RedirectToRouteResult)cut.AssociationType(new AssociationTypeViewModel()
            {
                ExpertAssociations = new List<AssociationViewModel>()
            }, 
            ViewConstants.SaveAction);

            FakeExpertCurrentSessionOfExpertsService.Received(1)
                .AssociationsTypes(Arg.Any<IReadOnlyCollection<AssociationDto>>(), Arg.Any<string>());
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void AssociationType_ActionIsFinish_CallFinishCurrentPhase()
        {
            ExpertController cut = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.SpecifyingAssociationsTypes);
            var stubUserName = "userName";
            cut.CurrentAuthorizedUser = CreateUser(stubUserName);

            cut.AssociationType(new AssociationTypeViewModel()
            {
                ExpertAssociations = new List<AssociationViewModel>()
            }, 
            ViewConstants.FinishAction);

            FakeExpertCurrentSessionOfExpertsService.Received(1).FinishCurrentPhase(Arg.Is(stubUserName));
        }

        #endregion

        #region RelationHttpPost

        [Test]
        public void Relation_SessionDoesNotExist_SaveErrorAndRedirectToExpertTest()
        {
            ExpertController cut = CreateControllerUnderTest();
            SetNullCurrentSession();
            
            var result = (RedirectToRouteResult)cut.Relation(new RelationViewModel());
            
            Assert.AreEqual(SessionNotExistErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [TestCase(SessionPhase.MakingAssociations)]
        [TestCase(SessionPhase.SpecifyingAssociationsTypes)]
        [TestCase(SessionPhase.SelectingNodes)]
        public void Relation_SessionExistsAndIsNotInRelationPhase_SaveErrorAndRedirectToExpertTest(
            SessionPhase currentPhase)
        {
            ExpertController cut = CreateControllerUnderTest();
            SetFakeCurrentSession(currentPhase);

            var result = (RedirectToRouteResult)cut.Relation(new RelationViewModel());
            
            Assert.AreEqual(NotAvailableOnThisPhaseErrorMessage, cut.TempData[DataConstants.Error]);
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        [Test]
        public void Relation_SessionExistsAndIsInRelationPhaseAndModelIsNotValid_ViewRelation()
        {
            ExpertController cut = CreateControllerUnderTest();
            SetFakeCurrentSession(SessionPhase.SelectingAndSpecifyingRelations);
            // set the ModelState.IsValid = false
            cut.ModelState.AddModelError("test", "test error");
            
            var result = (ViewResult)cut.Relation(new RelationViewModel());
            
            Assert.AreEqual("Relation", result.ViewName);
        }

        [Test]
        public void Relation_SessionExistsAndIsInRelationPhaseAndModelIsValid_SaveRelationAndRedirectToExpertTest()
        {
            ExpertController cut = CreateControllerUnderTest();
            cut.CurrentAuthorizedUser = Substitute.For<ICurrentUser>();
            SetFakeCurrentSession(SessionPhase.SelectingAndSpecifyingRelations);
            
            var result = (RedirectToRouteResult)cut.Relation(new RelationViewModel());
            
            FakeExpertCurrentSessionOfExpertsService.Received(1).Relations(Arg.Any<RelationTupleDto>(), Arg.Any<string>());
            Assert.AreEqual("ExpertTest", result.RouteValues["action"]);
        }

        #endregion
    }
}