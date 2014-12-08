using System.Collections.Generic;

namespace DeployDB
{
    public interface SchemaHistory
    {
        void EnsureHistoryDeployed();
        IEnumerable<AppliedScript> GetAppliedScripts();
        AppliedScript GetDeployedScript(string name);
        void SaveAppliedScript(AppliedScript appliedScript);
    }
}
