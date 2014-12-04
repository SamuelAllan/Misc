using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            this.utcNow = utcNow;
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
