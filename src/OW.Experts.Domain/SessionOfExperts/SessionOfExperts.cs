using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using OW.Experts.Domain.Infrastructure;

namespace OW.Experts.Domain
{
    public class SessionOfExperts : DomainObject
    {
        public SessionOfExperts([NotNull] string baseNotion)
        {
            if (string.IsNullOrWhiteSpace(baseNotion))
                throw new ArgumentException("Notion should not be empty string", nameof(baseNotion));

            BaseNotion = baseNotion;
            StartTime = TimeContext.Current.Now;
            CurrentPhase = SessionPhase.MakingAssociations;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionOfExperts"/> class.
        /// </summary>
        // ReSharper disable once NotNullMemberIsNotInitialized
        protected SessionOfExperts()
        {
        }

        public static Expression<Func<SessionOfExperts, bool>> IsEnded =>
            s => s.CurrentPhase == SessionPhase.Ended;

        [NotNull]
        public virtual string BaseNotion { get; }

        public virtual DateTime StartTime { get; }

        public virtual SessionPhase CurrentPhase { get; protected set; }

        public virtual void NextPhaseOrFinish()
        {
            if (CurrentPhase < SessionPhase.SelectingAndSpecifyingRelations)
                CurrentPhase++;
            else
                Finish();
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