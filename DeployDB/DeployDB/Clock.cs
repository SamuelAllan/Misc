using System;

namespace DeployDB
{
    public interface Clock
    {
        DateTime UtcNow { get; }
    }
}
