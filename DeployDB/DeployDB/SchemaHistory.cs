using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB
{
    public interface SchemaHistory
    {
        void EnsureHistoryDeployed();
        IEnumerable<AppliedScript> GetAppliedScripts();
        void SaveAppliedScript(AppliedScript appliedScript);
    }
}
