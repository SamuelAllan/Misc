using System;
using System.Collections.Generic;

namespace DeployDB
{
    public class DeployExecutor: Executor
    {
        private readonly ScriptStore scriptStore;
        private readonly SchemaHistory schemaHistory;
        private readonly Database database;
        private readonly Clock clock;
        private readonly Feedback feedback;

        public DeployExecutor(ScriptStore scriptStore, SchemaHistory schemaHistory, Database database, Clock clock, Feedback feedback)
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

        public void Execute(IEnumerable<string> plan)
        {
            feedback.WriteLine("Beginning deployment.");
            foreach (string name in plan)
            {
                feedback.WriteLine("Deploying {0}...", name);
                DeployScript(name);
                RecordSchemaHistory(name);
            }
            feedback.WriteLine("Done.");
        }

        private void DeployScript(string name)
        {
            try
            {
                Script script = scriptStore[name];
                database.ApplyScript(script.Deploy);
            }
            catch
            {
                feedback.WriteLine("Failed to deploy {0}.", name);
                throw;
            }
        }

        private void RecordSchemaHistory(string name)
        {
            try
            {
                var appliedScript = new AppliedScript(name, clock.UtcNow, null);
                schemaHistory.SaveAppliedScript(appliedScript);
            }
            catch
            {
                feedback.WriteLine("Deployed {0} successfully, but failed to record schema history for it.", name);
                throw;
            }
        }
    }
}
