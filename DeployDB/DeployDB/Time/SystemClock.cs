using System;

namespace Utilities.Time
{
    public class SystemClock: Clock
    {
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }
    }
}
