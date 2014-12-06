using System;

namespace DeployDB.System
{
    public class SystemClock: Clock
    {
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }
    }
}
