using NSubstitute;
using NUnit.Framework;
using OW.Experts.Domain;

namespace OW.Experts.WebUI.UnitTests.Base
{
    public class SessionControllerTests
    {
        protected const string SessionExistErrorMessage = "Текущая сессия уже существует";
        protected const string SessionNotExistErrorMessage = "Текущей сессии не существует";
        protected const string NotAvailableOnThisPhaseErrorMessage = "На данном этапе тестирования недоступно";

        protected const string NodeCandidateIsNotExistErrorMessage =
            "Одного или нескольких выбранных потенциальных узлов не существует";

        [OneTimeSetUp]
        public void SubstituteConverter()
        {
            SubstituteAutoConverter.Substitute();
        }

        [OneTimeTearDown]
        public void ResetConverter()
        {
            SubstituteAutoConverter.Reset();
        }

        protected ICurrentSessionService FakeCurrentSessionOfExpertsService { get; set; }

        protected SessionOfExperts SetFakeCurrentSession()
        {
            var session = Substitute.For<SessionOfExperts>();

            SetFakeCurrentSession(session);

            return session;
        }

        private void SetFakeCurrentSession(SessionOfExperts session)
        {
            FakeCurrentSessionOfExpertsService.DoesCurrentSessionExist.Returns(true);
            FakeCurrentSessionOfExpertsService.CurrentSession.Returns(session);
        }

        protected SessionOfExperts SetFakeCurrentSession(SessionPhase sessionPhase)
        {
            var session = Substitute.For<SessionOfExperts>();
            session.CurrentPhase.Returns(sessionPhase);

            SetFakeCurrentSession(session);

            return session;
        }

        protected void SetNullCurrentSession()
        {
            FakeCurrentSessionOfExpertsService.CurrentSession.Returns((SessionOfExperts)null);
            FakeCurrentSessionOfExpertsService.DoesCurrentSessionExist.Returns(false);
        }
    }
}