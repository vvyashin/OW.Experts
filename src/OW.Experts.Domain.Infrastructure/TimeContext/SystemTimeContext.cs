using System;

namespace OW.Experts.Domain.Infrastructure
{
    public class SystemTimeContext : TimeContext
    {
        public override DateTime Now => DateTime.Now;
        public override DateTime UtcNow => DateTime.UtcNow;
        public override DateTime Today => DateTime.Today;
    }
}
