using System;
using System.Collections.Generic;
using System.Linq;

namespace DeployDB
{
    public class RollbackPlanner: Planner
    {
        private readonly ScriptStore scriptStore;
        private readonly SchemaHistory schemaHistory;

        public RollbackPlanner(ScriptStore scriptStore, SchemaHistory schemaHistory)
        {
            if (scriptStore == null)
                throw new ArgumentNullException("scriptStore");
            if (schemaHistory == null)
                throw new ArgumentNullException("schemaHistory");

            this.scriptStore = scriptStore;
            this.schemaHistory = schemaHistory;
        }

        public IEnumerable<string> MakePlan(string destination)
        {
            HashSet<string> rollbackScripts = new HashSet<string>(scriptStore.Scripts
                .Where(x => x.Rollback != null)
                .Select(x => x.Name));

            var deployedScripts = schemaHistory.GetAppliedScripts()
                .Where(x => x.RollbackTime == null)
                .Select(x => x.Name)
                .OrderByDescending(x => x);

            CheckDestinationIsActuallyDeployed(destination, deployedScripts);

            List<string> plan = new List<string>();
            foreach(string deployedScript in deployedScripts)
            {
                if (deployedScript == destination)
                    break;

                if (rollbackScripts.Contains(deployedScript))
                    plan.Add(deployedScript);
                else
                    throw new Exception(string.Format("Don't know how to roll back deployed script: {0}.", deployedScript));
            }
            return plan;
        }

        private void CheckDestinationIsActuallyDeployed(string destination, IOrderedEnumerable<string> deployedScripts)
        {
            if (destination == null)
                return;
            if (!deployedScripts.Contains(destination))
                throw new Exception(string.Format("Destination is not actually deployed: {0}.", destination));
        }
    }
}
