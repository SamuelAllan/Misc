using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB
{
    public class Deployer
    {
        private readonly ScriptStore scriptStore;
        private readonly SchemaHistory schemaHistory;
        private readonly Database database;
        private readonly Clock clock;
        private readonly Feedback feedback;

        public Deployer(ScriptStore scriptStore, SchemaHistory schemaHistory, Database database, Clock clock, Feedback feedback)
        {
            if (scriptStore == null)
                throw new ArgumentNullException("scriptStore");
            if (schemaHistory == null)
                throw new ArgumentNullException("schemaHistory");
            if (database == null)
                throw new ArgumentNullException("database");
            if (clock == null)
                throw new ArgumentNullException("clock");
            if (feedback == null)
                throw new ArgumentNullException("feedback");

            this.scriptStore = scriptStore;
            this.schemaHistory = schemaHistory;
            this.database = database;
            this.clock = clock;
            this.feedback = feedback;
        }

        public void Deploy()
        {
            List<string> plan = MakePlan();

            plan.Add(null);
        }

        private List<string> MakePlan()
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
                if (!deployedScripts.Contains(script))
                    plan.Add(script);
            }
            return plan;
        }
    }
}
