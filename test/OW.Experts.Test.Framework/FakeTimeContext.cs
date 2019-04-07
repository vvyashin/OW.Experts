using System;
using OW.Experts.Domain.Infrastructure;

namespace OW.Experts.Test.Infrastructure
{
    public class FakeTimeContext : TimeContext
    {
        private DateTime _now;
        private DateTime _utcNow;
        private DateTime _today;

        public FakeTimeContext()
        {
            _now = DateTime.MinValue.AddYears(1900).AddSeconds(21387);
            _utcNow = DateTime.MinValue.AddYears(1900).AddSeconds(21388);
            _today = DateTime.MinValue.AddYears(1900).AddSeconds(21389);
        }

        public override DateTime Now => _now;
        public override DateTime UtcNow => _utcNow;
        public override DateTime Today => _today;
    }
}
