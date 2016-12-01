using System;
using System.Linq.Expressions;
using Domain.Infrastructure;
using JetBrains.Annotations;

namespace Domain
{
    public class SessionOfExperts : DomainObject
    {
        #region rules

        public static Expression<Func<SessionOfExperts, bool>> IsEnded =>
            s => s.CurrentPhase == SessionPhase.Ended;

        #endregion

        /// <summary>
        /// Ctor only for mapping from repository
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        protected SessionOfExperts() { }

        public SessionOfExperts([NotNull] string baseNotion)
        {
            if (String.IsNullOrWhiteSpace(baseNotion))
                throw new ArgumentException("Notion should not be empty string", nameof(baseNotion));

            BaseNotion = baseNotion;
            StartTime = TimeContext.Current.Now;
            CurrentPhase = SessionPhase.MakingAssociations;
        }

        [NotNull]
        public virtual string BaseNotion { get; }

        public virtual DateTime StartTime { get; }

        public virtual SessionPhase CurrentPhase { get; protected set; }

        public virtual void NextPhaseOrFinish()
        {
            if (CurrentPhase < SessionPhase.SelectingAndSpecifyingRelations) {
                CurrentPhase++;
            }
            else {
                Finish();
            }
        }

        public virtual void Finish()
        {
            CurrentPhase = SessionPhase.Ended;
        }

        public override string ToString()
        {
            return $"{BaseNotion} ({StartTime})";
        }
    }
}