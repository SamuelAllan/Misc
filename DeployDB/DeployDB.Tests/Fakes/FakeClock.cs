using System;

namespace DeployDB.Tests.Fakes
{
    public class FakeClock: Clock
    {
        private DateTime utcNow;

        public FakeClock()
            :this(DateTime.UtcNow)
        {
        }

        public FakeClock(DateTime utcNow)
        {
            this.utcNow = new DateTime(utcNow.Ticks, DateTimeKind.Utc);
        }

        public DateTime UtcNow
        {
            get { return utcNow; }
        }

        public void PassTime(int seconds)
        {
            utcNow = utcNow.AddSeconds(seconds);
        }

        public void SetNow(DateTime newUtcNow)
        {
            utcNow = newUtcNow;
        }
    }
}
