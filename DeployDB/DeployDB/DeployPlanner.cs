using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeployDB
{
    public class DeployPlanner
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

        public IEnumerable<string> MakePlan()
        {
            var deployedScripts = new HashSet<string>(schemaHistory.GetAppliedScripts()
                .Where(x => x.RollbackTime == null)
                .Select(x => x.Name));

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
            }

            CheckForUnknownDeployedScripts(deployedScripts);
            return plan;
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
    }
}
