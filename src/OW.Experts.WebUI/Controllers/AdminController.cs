using System;
using System.Collections.Generic;
using System.Web.Mvc;
using JetBrains.Annotations;
using OW.Experts.Domain;
using OW.Experts.Domain.Infrastructure.Repository;
using OW.Experts.WebUI.Infrastructure;
using OW.Experts.WebUI.Infrastructure.AutoConverter;
using OW.Experts.WebUI.Infrastructure.Binders;
using OW.Experts.WebUI.Services;
using OW.Experts.WebUI.ViewModels.Admin;

namespace OW.Experts.WebUI.Controllers
{
    [Authorize(Roles = RoleNames.Admin)]
    public class AdminController : BaseSessionController
    {
        #region dependencies

        [NotNull]
        private readonly IAdminCurrentSessionService _currentSessionOfExpertsService;

        #endregion

        public AdminController([NotNull] IUnitOfWorkFactory unitOfWorkFactory, 
            [NotNull] IAdminCurrentSessionService currentSessionOfExpertsService, [NotNull] LogService logService)
            : base(unitOfWorkFactory, currentSessionOfExpertsService, logService)
        {
            if (currentSessionOfExpertsService == null)
                throw new ArgumentNullException(nameof(currentSessionOfExpertsService));
            if (logService == null) throw new ArgumentNullException(nameof(logService));
            
            _currentSessionOfExpertsService = currentSessionOfExpertsService;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return RedirectToAction("CurrentSession");
        }

        [HttpGet]
        public ActionResult CurrentSession()
        {
            using (UnitOfWorkFactory.Create()) {
                if (_currentSessionOfExpertsService.CurrentSession == null) {
                    // If current session is not exists
                    return View("NewSession");
                }
                else {
                    SessionViewModel sessionViewModel = _currentSessionOfExpertsService.
                        CurrentSession.ConvertTo<SessionViewModel>();
                    return View("CurrentSession", sessionViewModel);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewSession(NewSessionViewModel model)
        {
            return HandleHttpPostRequest(
                currentSessionShouldNotExist: true,
                tryExecute: () =>
                {
                    _currentSessionOfExpertsService.StartNewSession(model.BaseNotion);
                    return RedirectToAction("CurrentSession");
                },
                andIfErrorReturn: RedirectToAction("CurrentSession"),
                viewWithProcessedForm: View("NewSession", model)
                );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NextPhase()
        {
            return HandleHttpPostRequest(
                currentSessionShouldExist: true,
                tryExecute: () =>
                {
                    _currentSessionOfExpertsService.NextPhase();
                    return RedirectToAction("CurrentSession");
                },
                andIfErrorReturn: RedirectToAction("CurrentSession")
                );
        }

        [HttpGet]
        public ActionResult SelectNode()
        {
            return HandleHttpGetRequest(
                currentSessionShouldExist: true,
                currentSessionShouldOnPhase: SessionPhase.SelectingNodes,
                tryExecute: () =>
                {
                    var nodeCandidatesForView = _currentSessionOfExpertsService.GetAllNodeCandidates().
                        ConvertTo<List<NodeCandidateViewModel>>();

                    return View("SelectNode", new NodeCandidateListViewModel()
                    {
                        NodeCandidates = nodeCandidatesForView,
                    });
                },
                andIfFailReturn: RedirectToAction("CurrentSession")
                );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectNode(
            [ModelBinder(typeof (NodeCandidatesBinder))] NodeCandidateListViewModel nodeCandidateListViewModels)
        {
            return HandleHttpPostRequest(
                currentSessionShouldExist: true,
                currentSessionShouldOnPhase: SessionPhase.SelectingNodes,
                tryExecute: () =>
                {
                    _currentSessionOfExpertsService.CreateSemanticNetworkFromNodeCandidates(
                        nodeCandidateListViewModels.NodeCandidates.ConvertTo<List<NodeCandidate>>());

                    return RedirectToAction("CurrentSession");
                },
                andIfErrorReturn: RedirectToAction("CurrentSession"),
                viewWithProcessedForm: View("SelectNode", nodeCandidateListViewModels)
                );
        }

        [HttpGet]
        public ActionResult SemanticNetwork()
        {
            return HandleHttpGetRequest(
                currentSessionShouldExist: true,
                tryExecute: () =>
                {
                    SemanticNetworkReadModel semanticNetwork =
                        _currentSessionOfExpertsService.GetSemanticNetwork();
                    return View("SemanticNetwork", semanticNetwork);
                },
                andIfFailReturn: RedirectToAction("CurrentSession")
                );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveVerges()
        {
            return HandleHttpPostRequest(
                currentSessionShouldExist: true,
                currentSessionShouldOnPhase: SessionPhase.SelectingAndSpecifyingRelations,
                tryExecute: () =>
                {
                    _currentSessionOfExpertsService.SaveRelationsAsVergesOfSemanticNetwork();
                    return RedirectToAction("SemanticNetwork");
                },
                andIfErrorReturn: RedirectToAction("CurrentSession")
                );
        }
    }
}