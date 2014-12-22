using System.Collections.Generic;

namespace DeployDB
{
    public interface Planner
    {
        IEnumerable<string> MakePlan(string destination);
    }
}
