using System;
using OW.Experts.Domain.Infrastructure;

namespace OW.Experts.Test.Infrastructure
{
    public class FakeTimeContext : TimeContext
    {
        public FakeTimeContext()
        {
            Now = DateTime.MinValue.AddYears(1900).AddSeconds(21387);
            UtcNow = DateTime.MinValue.AddYears(1900).AddSeconds(21388);
            Today = DateTime.MinValue.AddYears(1900).AddSeconds(21389);
        }

        public override DateTime Now { get; }

        public override DateTime UtcNow { get; }

        public override DateTime Today { get; }
    }
}