using System.Collections.Generic;

namespace DeployDB
{
    public interface SchemaHistory
    {
        void EnsureHistoryDeployed();
        IEnumerable<AppliedScript> GetAppliedScripts();
        void SaveAppliedScript(AppliedScript appliedScript);
    }
}
