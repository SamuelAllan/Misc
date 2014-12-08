using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB
{
    public interface Planner
    {
        IEnumerable<string> MakePlan(string destination);
    }
}
