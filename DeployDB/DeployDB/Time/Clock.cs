using System;

namespace Utilities.Time
{
    public interface Clock
    {
        DateTime UtcNow { get; }
    }
}
