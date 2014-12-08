using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeployDB
{
    public class DeployPlanner : Planner
    {
        private readonly ScriptStore scriptStore;
        private readonly SchemaHistory schemaHistory;

        public DeployPlanner(ScriptStore scriptStore, SchemaHistory schemaHistory)
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
            var deployedScripts = new HashSet<string>(schemaHistory.GetAppliedScripts()
                .Where(x => x.RollbackTime == null)
                .Select(x => x.Name));

            CheckThatDestinationIsNotAlreadyDeployed(destination, deployedScripts);

            var scripts = scriptStore.Scripts
                .Select(x => x.Name)
                .OrderBy(x => x);

            List<string> plan = new List<string>();
            foreach (string script in scripts)
            {
                if (deployedScripts.Contains(script))
                    deployedScripts.Remove(script);
                else
                    plan.Add(script);

                if (script == destination)
                    break;
            }

            CheckForUnknownDeployedScripts(deployedScripts);
            CheckThatArrivedAtDestination(destination, plan);
            return plan;
        }

        private void CheckThatDestinationIsNotAlreadyDeployed(string destination, HashSet<string> deployedScripts)
        {
            if (destination == null)
                return;
            if (deployedScripts.Contains(destination))
            {
                string message = string.Format("Destination script is already deployed: {0}.", destination);
                throw new Exception(message);
            }
        }

        private static void CheckForUnknownDeployedScripts(HashSet<string> deployedScripts)
        {
            if (deployedScripts.Count > 0)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine("Cannot deploy - DB has scripts deployed to it that I don't know about:");
                foreach (string script in deployedScripts.OrderBy(x => x))
                    message.AppendLine("    " + script);
                throw new Exception(message.ToString());
            }
        }

        private void CheckThatArrivedAtDestination(string destination, List<string> plan)
        {
            if (destination == null)
                return;
            string lastPlanned = plan.LastOrDefault();
            if (lastPlanned == destination)
                return;

            string message = string.Format("Destination script does not exist: {0}.", destination);
            throw new Exception(message);
        }
    }
}
