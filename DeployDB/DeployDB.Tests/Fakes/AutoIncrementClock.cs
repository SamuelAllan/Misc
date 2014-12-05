using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB.Tests.Fakes
{
    public class AutoIncrementClock: Clock
    {
        private DateTime utcNow;
        private TimeSpan increment;

        public AutoIncrementClock()
            :this(DateTime.UtcNow)
        {
        }

        public AutoIncrementClock(DateTime initialUtc)
            : this(initialUtc, new TimeSpan(0, 0, 1))
        {
        }

        public AutoIncrementClock(DateTime initialUtc, TimeSpan increment)
        {
            this.utcNow = new DateTime(initialUtc.Ticks, DateTimeKind.Utc);
            this.increment = increment;
        }

        public DateTime UtcNow
        {
            get
            {
                var now = utcNow;
                utcNow = utcNow.Add(increment);
                return now;
            }
        }
    }
}
