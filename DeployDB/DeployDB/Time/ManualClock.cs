using System;

namespace Utilities.Time
{
    /// <summary>
    /// A clock that is only manually adjusted. For testing purposes.
    /// </summary>
    public class ManualClock: Clock
    {
        private DateTime utcNow;

        public ManualClock()
        {
            utcNow = DateTime.UtcNow;
        }

        public ManualClock(DateTime utcNow)
        {
            this.utcNow = utcNow;
        }

        public DateTime UtcNow
        {
            get { return utcNow; }
        }

        public void Advance(TimeSpan delta)
        {
            utcNow = utcNow.Add(delta);
        }

        public void AdvanceSeconds(int seconds)
        {
            utcNow = utcNow.AddSeconds(seconds);
        }
    }
}
