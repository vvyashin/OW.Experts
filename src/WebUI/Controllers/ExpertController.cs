using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Infrastructure;
using JetBrains.Annotations;
using WebUI.Infrastructure;
using WebUI.Infrastructure.AutoConverter;
using WebUI.Infrastructure.Binders;
using WebUI.Services;
using WebUI.ViewModels;
using WebUI.ViewModels.Admin;
using WebUI.ViewModels.Expert;

namespace WebUI.Controllers
{
    [Authorize(Roles = RoleNames.Expert)]
    public class ExpertController : BaseSessionController
    {
        private ICurrentUser _currentUser;

        public ICurrentUser CurrentAuthorizedUser
        {
            get { return _currentUser ?? (_currentUser = new HttpContextCurrentUser()); }
            set { if (_currentUser == null) _currentUser = value; }
        }

        #region dependencies

        [NotNull]
        private readonly IExpertCurrentSessionService _currentSessionOfExpertsService;

        private readonly IGetNotionTypesQuery<NotionTypeViewModel> _notionTypeQuery;

        // + LogService in base class

        #endregion

        public ExpertController([NotNull] IUnitOfWorkFactory unitOfWorkFactory, [NotNull] IExpertCurrentSessionService currentSessionOfExpertsService,
            [NotNull] IGetNotionTypesQuery<NotionTypeViewModel> notionTypeQuery, [NotNull] LogService logService)
            : base(unitOfWorkFactory, currentSessionOfExpertsService, logService)
        {
            if (currentSessionOfExpertsService == null)
                throw new ArgumentNullException(nameof(currentSessionOfExpertsService));
            if (notionTypeQuery == null) throw new ArgumentNullException(nameof(notionTypeQuery));
            if (logService == null) throw new ArgumentNullException(nameof(logService));
            
            _currentSessionOfExpertsService = currentSessionOfExpertsService;
            _notionTypeQuery = notionTypeQuery;
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
                CurrentAuthorizedUser.Name)) {
                return Wait();
            }

            var model = new AllAssociationViewModel();
            model.Body = String.Join(", ", _currentSessionOfExpertsService
                .GetAssociationsByExpertName(CurrentAuthorizedUser.Name).Select(x => x.Notion));
            // ReSharper disable once PossibleNullReferenceException
            model.BaseNotion = _currentSessionOfExpertsService.CurrentSession.BaseNotion;

            return View("Association", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Association(AllAssociationViewModel model)
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
                            .Where(x => !String.IsNullOrWhiteSpace(x))
                            .ToList(), CurrentAuthorizedUser.Name);
                    this.Success("Ассоциации успешно сохранены");
                    return RedirectToAction("ExpertTest");
                },
                viewWithProcessedForm: View("Association", model),
                andIfErrorReturn: RedirectToAction("ExpertTest", "Expert")
                );
        }

        private ActionResult AssociationType()
        {
            if (_currentSessionOfExpertsService.DoesExpertCompleteCurrentPhase(
                CurrentAuthorizedUser.Name)) {
                return Wait();
            }

            var associations = _currentSessionOfExpertsService
                .GetAssociationsByExpertName(CurrentAuthorizedUser.Name)
                .ConvertTo<List<AssociationViewModel>>();

            AssociationTypeViewModel model = new AssociationTypeViewModel()
            {
                ExpertAssociations = associations,
            };
            this.PopulateNotionTypes(_notionTypeQuery.Execute(
                new GetNotionTypesSpecification<NotionTypeViewModel>(x => new NotionTypeViewModel()
                {
                    Id = x.Id.ToString(),
                    Name = x.Name
                })));

            return View("AssociationType", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssociationType(AssociationTypeViewModel model)
        {
            return HandleHttpPostRequest(
                currentSessionShouldExist: true,
                currentSessionShouldOnPhase: SessionPhase.SpecifyingAssociationsTypes,
                tryExecute: () =>
                {
                    _currentSessionOfExpertsService.AssociationsTypes(
                        model.ExpertAssociations.ConvertTo<List<AssociationDto>>(), CurrentAuthorizedUser.Name);
                    this.Success("Типы ассоциаций успешно сохранены");
                    return RedirectToAction("ExpertTest");
                },
                viewWithProcessedForm: View("AssociationType", model),
                andIfErrorReturn: RedirectToAction("ExpertTest")
                );
        }

        private ActionResult Relation()
        {
            Tuple<Relation, Relation> relationPair = _currentSessionOfExpertsService.
                GetNextRelationByExpertName(CurrentAuthorizedUser.Name);

            if (relationPair == null) {
                return EndSession();
            }
            else {
                return View("Relation", relationPair.ConvertTo<RelationViewModel>());
            }
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
                        relationViewModel.ConvertTo<RelationTupleDto>(), CurrentAuthorizedUser.Name);

                    this.Success("Типы ассоциаций успешно сохранены");
                    return RedirectToAction("ExpertTest");
                },
                viewWithProcessedForm: View("Relation", relationViewModel),
                andIfErrorReturn: RedirectToAction("ExpertTest")
                );
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
                andIfErrorReturn: RedirectToAction("ExpertTest")
                );
        }

        [HttpGet]
        public ActionResult ExpertTest()
        {
            using (UnitOfWorkFactory.Create()) {
                if (_currentSessionOfExpertsService.CurrentSession == null) {
                    return NoSession();
                }
                else if (_currentSessionOfExpertsService.
                    DoesExpertJoinSession(CurrentAuthorizedUser.Name)) {
                    return CurrentPhase();
                }
                else {
                    var sessionModel = _currentSessionOfExpertsService.
                        CurrentSession.ConvertTo<SessionViewModel>();
                    return View("JoinSession", sessionModel);
                }
            }
        }
    }
}