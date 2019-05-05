using System;
using System.Web.Mvc;
using JetBrains.Annotations;
using OW.Experts.Domain;
using OW.Experts.Domain.Infrastructure.Repository;
using OW.Experts.WebUI.Infrastructure;
using OW.Experts.WebUI.Services;

namespace OW.Experts.WebUI.Controllers
{
    public class BaseSessionController : Controller
    {
        private const string SessionAlreadyExistsErrorMessage = "Текущая сессия уже существует";
        private const string SessionDoesNotExistsErrorMessage = "Текущей сессии не существует";
        private const string DoesNotAvailableInThisPhase = "На данном этапе тестирования недоступно";
        private const string InnerErrorMessage = "Произошла внутренняя ошибка приложения.Обратитесь к администратору";

        [NotNull]
        private readonly ICurrentSessionService _currentSessionService;

        [NotNull]
        private readonly LogService _logService;

        protected BaseSessionController(
            [NotNull] IUnitOfWorkFactory unitOfWorkFactory,
            [NotNull] ICurrentSessionService currentSessionService,
            [NotNull] LogService logService)
        {
            if (unitOfWorkFactory == null) throw new ArgumentNullException(nameof(unitOfWorkFactory));
            if (currentSessionService == null) throw new ArgumentNullException(nameof(currentSessionService));
            if (logService == null) throw new ArgumentNullException(nameof(logService));

            UnitOfWorkFactory = unitOfWorkFactory;
            _currentSessionService = currentSessionService;
            _logService = logService;
        }

        protected IUnitOfWorkFactory UnitOfWorkFactory { get; }

        /// <summary>
        /// Handles Http request without form (Get and Post with empty form). If Success return appropriate view.
        /// If fail return ActionResult from parameters.
        /// </summary>
        /// <param name="tryExecute">Code for getting data and returning view.</param>
        /// <param name="andIfFailReturn">If error return this.</param>
        /// <param name="currentSessionShouldNotExist">If true condition (currentSessionShouldNotExist) is verified.</param>
        /// <param name="currentSessionShouldExist">If true the existence of the session is verified.</param>
        /// <param name="currentSessionShouldOnPhase">If not null the phase of session is verified.</param>
        /// <returns>ActionResult of mvc controller.</returns>
        protected ActionResult HandleHttpGetRequest(
            [NotNull] Func<ActionResult> tryExecute,
            [NotNull] ActionResult andIfFailReturn,
            bool currentSessionShouldNotExist = false,
            bool currentSessionShouldExist = false,
            SessionPhase? currentSessionShouldOnPhase = null)
        {
            if (tryExecute == null) throw new ArgumentNullException(nameof(tryExecute));
            if (andIfFailReturn == null) throw new ArgumentNullException(nameof(andIfFailReturn));

            using (UnitOfWorkFactory.Create()) {
                if (currentSessionShouldNotExist && _currentSessionService.DoesCurrentSessionExist) {
                    this.Fail(SessionAlreadyExistsErrorMessage);
                    return andIfFailReturn;
                }

                if (currentSessionShouldExist && _currentSessionService.DoesCurrentSessionExist == false) {
                    this.Fail(SessionDoesNotExistsErrorMessage);
                    return andIfFailReturn;
                }

                if (currentSessionShouldOnPhase != null &&
                    _currentSessionService.CurrentSession?.CurrentPhase != currentSessionShouldOnPhase.Value) {
                    this.Fail(DoesNotAvailableInThisPhase);
                    return andIfFailReturn;
                }

                try {
                    return tryExecute();
                }
                catch (Exception ex) {
                    _logService.Log(ex);
                    this.Fail(InnerErrorMessage);
                    return andIfFailReturn;
                }
            }
        }

        /// <summary>
        /// Handles Http request with not empty form. If Success return result of function @tryExecute.
        /// If fail return processedView with errors. If global errors that don't depend on the filling form
        /// (like 'Session does not exist') return ActionResult @andIfErrorReturn.
        /// </summary>
        /// <param name="tryExecute">Code for getting data and returning view.</param>
        /// <param name="andIfErrorReturn">If global error return this.</param>
        /// <param name="viewWithProcessedForm">If fail on filling form return this.</param>
        /// <param name="currentSessionShouldNotExist">If true condition (currentSessionShouldNotExist) is verified.</param>
        /// <param name="currentSessionShouldExist">If true the existence of the session is verified.</param>
        /// <param name="currentSessionShouldOnPhase">If not null the phase of session is verified.</param>
        /// <returns>ActionResult of mvc controller.</returns>
        protected ActionResult HandleHttpPostRequest(
            [NotNull] Func<ActionResult> tryExecute,
            [NotNull] ActionResult andIfErrorReturn,
            ViewResult viewWithProcessedForm = null,
            bool currentSessionShouldNotExist = false,
            bool currentSessionShouldExist = false,
            SessionPhase? currentSessionShouldOnPhase = null)
        {
            if (tryExecute == null) throw new ArgumentNullException(nameof(tryExecute));
            if (andIfErrorReturn == null) throw new ArgumentNullException(nameof(andIfErrorReturn));

            using (var unitOfWork = UnitOfWorkFactory.Create()) {
                if (currentSessionShouldNotExist && _currentSessionService.DoesCurrentSessionExist) {
                    this.Fail(SessionAlreadyExistsErrorMessage);
                    return andIfErrorReturn;
                }

                if (currentSessionShouldExist && _currentSessionService.DoesCurrentSessionExist == false) {
                    this.Fail(SessionDoesNotExistsErrorMessage);
                    return andIfErrorReturn;
                }

                if (currentSessionShouldOnPhase != null &&
                    _currentSessionService.CurrentSession?.CurrentPhase != currentSessionShouldOnPhase.Value) {
                    this.Fail(DoesNotAvailableInThisPhase);
                    return andIfErrorReturn;
                }

                try {
                    if (viewWithProcessedForm == null || ModelState.IsValid) {
                        var result = tryExecute();
                        unitOfWork.Commit();

                        return result;
                    }

                    return viewWithProcessedForm;
                }
                catch (Exception ex) {
                    _logService.Log(ex);
                    this.Fail(InnerErrorMessage);
                    return andIfErrorReturn;
                }
            }
        }
    }
}