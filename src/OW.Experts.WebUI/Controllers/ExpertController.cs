using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using JetBrains.Annotations;
using OW.Experts.Domain;
using OW.Experts.Domain.Infrastructure.Repository;
using OW.Experts.WebUI.Constants;
using OW.Experts.WebUI.Infrastructure;
using OW.Experts.WebUI.Infrastructure.AutoConverter;
using OW.Experts.WebUI.Services;
using OW.Experts.WebUI.ViewModels;
using OW.Experts.WebUI.ViewModels.Admin;
using OW.Experts.WebUI.ViewModels.Expert;

namespace OW.Experts.WebUI.Controllers
{
    [Authorize(Roles = RoleNames.Expert)]
    public class ExpertController : BaseSessionController
    {
        [NotNull]
        private readonly IExpertCurrentSessionService _currentSessionOfExpertsService;

        private readonly IGetNotionTypesQuery<NotionTypeViewModel> _notionTypeQuery;
        private ICurrentUser _currentUser;

        public ExpertController(
            [NotNull] IUnitOfWorkFactory unitOfWorkFactory,
            [NotNull] IExpertCurrentSessionService currentSessionOfExpertsService,
            [NotNull] IGetNotionTypesQuery<NotionTypeViewModel> notionTypeQuery,
            [NotNull] LogService logService)
            : base(unitOfWorkFactory, currentSessionOfExpertsService, logService)
        {
            if (currentSessionOfExpertsService == null) throw new ArgumentNullException(nameof(currentSessionOfExpertsService));
            if (notionTypeQuery == null) throw new ArgumentNullException(nameof(notionTypeQuery));
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            _currentSessionOfExpertsService = currentSessionOfExpertsService;
            _notionTypeQuery = notionTypeQuery;
        }

        public ICurrentUser CurrentAuthorizedUser
        {
            get => _currentUser ?? (_currentUser = new HttpContextCurrentUser());
            set
            {
                if (_currentUser == null) _currentUser = value;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Association(AllAssociationViewModel model, string action)
        {
            // ReSharper disable once PossibleNullReferenceException
            return HandleHttpPostRequest(
                currentSessionShouldExist: true,
                currentSessionShouldOnPhase: SessionPhase.MakingAssociations,
                tryExecute: () =>
                {
                    _currentSessionOfExpertsService.Associations(
                        model.Body.Split(',', ';')
                            .Select(x => x.Trim())
                            .Distinct()
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .ToList(),
                        CurrentAuthorizedUser.Name);
                    FinishPhaseIfActionIsFinish(action);
                    this.Success("Ассоциации успешно сохранены");
                    return RedirectToAction("ExpertTest");
                },
                viewWithProcessedForm: View("Association", model),
                andIfErrorReturn: RedirectToAction("ExpertTest", "Expert"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssociationType(AssociationTypeViewModel model, string action)
        {
            return HandleHttpPostRequest(
                currentSessionShouldExist: true,
                currentSessionShouldOnPhase: SessionPhase.SpecifyingAssociationsTypes,
                tryExecute: () =>
                {
                    _currentSessionOfExpertsService.AssociationsTypes(
                        model.ExpertAssociations.ConvertTo<List<AssociationDto>>(),
                        CurrentAuthorizedUser.Name);
                    FinishPhaseIfActionIsFinish(action);
                    this.Success("Типы ассоциаций успешно сохранены");
                    return RedirectToAction("ExpertTest");
                },
                viewWithProcessedForm: View("AssociationType", model),
                andIfErrorReturn: RedirectToAction("ExpertTest"));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Relation(RelationViewModel relationViewModel)
        {
            return HandleHttpPostRequest(
                currentSessionShouldExist: true,
                currentSessionShouldOnPhase: SessionPhase.SelectingAndSpecifyingRelations,
                tryExecute: () =>
                {
                    _currentSessionOfExpertsService.Relations(
                        relationViewModel.ConvertTo<RelationTupleDto>(),
                        CurrentAuthorizedUser.Name);

                    this.Success("Типы ассоциаций успешно сохранены");
                    return RedirectToAction("ExpertTest");
                },
                viewWithProcessedForm: View("Relation", relationViewModel),
                andIfErrorReturn: RedirectToAction("ExpertTest"));
        }

        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("ExpertTest");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult JoinSession()
        {
            return HandleHttpPostRequest(
                currentSessionShouldExist: true,
                currentSessionShouldOnPhase: SessionPhase.MakingAssociations,
                tryExecute: () =>
                {
                    _currentSessionOfExpertsService.JoinSession(CurrentAuthorizedUser.Name);
                    return RedirectToAction("ExpertTest");
                },
                andIfErrorReturn: RedirectToAction("ExpertTest"));
        }

        [HttpGet]
        public ActionResult ExpertTest()
        {
            using (UnitOfWorkFactory.Create()) {
                if (_currentSessionOfExpertsService.CurrentSession == null) return NoSession();

                if (_currentSessionOfExpertsService.DoesExpertJoinSession(CurrentAuthorizedUser.Name))
                    return CurrentPhase();

                var sessionModel = _currentSessionOfExpertsService.CurrentSession.ConvertTo<SessionViewModel>();
                return View("JoinSession", sessionModel);
            }
        }

        private ActionResult CurrentPhase()
        {
            // ReSharper disable once PossibleNullReferenceException
            switch (_currentSessionOfExpertsService.CurrentSession.CurrentPhase) {
                case SessionPhase.MakingAssociations:
                    return Association();
                case SessionPhase.SpecifyingAssociationsTypes:
                    return AssociationType();
                case SessionPhase.SelectingNodes:
                    return Wait();
                case SessionPhase.SelectingAndSpecifyingRelations:
                    return Relation();
                default:
                    return NoSession();
            }
        }

        private ActionResult Association()
        {
            if (_currentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(
                CurrentAuthorizedUser.Name))
                return Wait();

            var model = new AllAssociationViewModel();
            model.Body = string.Join(
                ", ",
                _currentSessionOfExpertsService
                    .GetAssociationsByExpertName(CurrentAuthorizedUser.Name).Select(x => x.Notion));

            // ReSharper disable once PossibleNullReferenceException
            model.BaseNotion = _currentSessionOfExpertsService.CurrentSession.BaseNotion;

            return View("Association", model);
        }

        private void FinishPhaseIfActionIsFinish(string action)
        {
            if (action == ViewConstants.FinishAction)
                _currentSessionOfExpertsService.FinishCurrentPhase(CurrentAuthorizedUser.Name);
        }

        private ActionResult AssociationType()
        {
            if (_currentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(
                CurrentAuthorizedUser.Name))
                return Wait();

            var associations = _currentSessionOfExpertsService
                .GetAssociationsByExpertName(CurrentAuthorizedUser.Name)
                .ConvertTo<List<AssociationViewModel>>();

            var model = new AssociationTypeViewModel
            {
                ExpertAssociations = associations
            };
            this.PopulateNotionTypes(
                _notionTypeQuery.Execute(
                    new GetNotionTypesSpecification<NotionTypeViewModel>(
                        x => new NotionTypeViewModel
                        {
                            Id = x.Id.ToString(),
                            Name = x.Name
                        })));

            return View("AssociationType", model);
        }

        private ActionResult Relation()
        {
            var relationPair = _currentSessionOfExpertsService.GetNextRelationByExpertName(CurrentAuthorizedUser.Name);

            if (relationPair == null)
                return EndSession();
            return View("Relation", relationPair.ConvertTo<RelationViewModel>());
        }

        private ActionResult Wait()
        {
            return View("Wait");
        }

        private ActionResult EndSession()
        {
            return View("EndSession");
        }

        private ActionResult NoSession()
        {
            return View("NoSession");
        }
    }
}