using System;

namespace OW.Experts.Domain.Infrastructure
{
    public abstract class TimeContext
    {
        private static TimeContext _current;

        public static TimeContext Current
        {
            get => _current ?? (_current = new SystemTimeContext());
            set => _current = value;
        }

        public abstract DateTime Now { get; }

        public abstract DateTime UtcNow { get; }

        public abstract DateTime Today { get; }

        public static void Reset()
        {
            _current = null;
        }
    }
}