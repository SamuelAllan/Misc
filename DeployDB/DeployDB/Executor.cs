using System.Collections.Generic;

namespace DeployDB
{
    public interface Executor
    {
        void Execute(IEnumerable<string> plan);
    }
}
