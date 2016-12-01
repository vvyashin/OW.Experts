using Domain.Infrastructure;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;

namespace Test.Infrastructure
{
    public static class TimeContextAssertionExtensions
    {
        /// <summary>
        /// Asserts that a date time is now
        /// </summary>
        /// <remarks>TimeContext.Current should be instance of FakeTimeContext</remarks>
        public static AndConstraint<DateTimeAssertions> BeNow(this DateTimeAssertions dateTimeAssertions, string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                .ForCondition(dateTimeAssertions.Subject == TimeContext.Current.Now)
                .BecauseOf(because, becauseArgs)
                .FailWith("Expected now date time{reason}, but found {0}.", dateTimeAssertions.Subject);

            return new AndConstraint<DateTimeAssertions>(dateTimeAssertions);
        }
    }
}
