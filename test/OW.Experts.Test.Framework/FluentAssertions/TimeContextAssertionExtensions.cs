using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using OW.Experts.Domain.Infrastructure;

namespace OW.Experts.Test.Infrastructure.FluentAssertions
{
    public static class TimeContextAssertionExtensions
    {
        /// <summary>
        /// Asserts that a date time is now.
        /// </summary>
        /// <remarks>TimeContext.Current should be instance of FakeTimeContext.</remarks>
        /// <param name="dateTimeAssertions">Fluent Assertions DateTimeAssertions.</param>
        /// <param name="because">
        /// A formatted phrase as is supported by <see cref="M:System.String.Format(System.String,System.Object[])" /> explaining why the assertion
        /// is needed. If the phrase does not start with the word <i>because</i>, it is prepended automatically.
        /// </param>
        /// <param name="becauseArgs">
        /// Zero or more objects to format using the placeholders in <see cref="!:because" />.
        /// </param>
        /// <returns>Fluent Assertions AndConstraint.</returns>
        public static AndConstraint<DateTimeAssertions> BeNow(
            this DateTimeAssertions dateTimeAssertions,
            string because = "",
            params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(dateTimeAssertions.Subject == TimeContext.Current.Now)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected now date time{reason}, but found {0}.", dateTimeAssertions.Subject);

            return new AndConstraint<DateTimeAssertions>(dateTimeAssertions);
        }
    }
}