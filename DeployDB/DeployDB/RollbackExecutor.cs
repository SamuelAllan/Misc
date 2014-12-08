using System;
using System.Collections.Generic;

namespace DeployDB
{
    public class RollbackExecutor: Executor
    {
        // TODO: Pull duplications with DeployExecutor up into a shared base class?
        private readonly ScriptStore scriptStore;
        private readonly SchemaHistory schemaHistory;
        private readonly Database database;
        private readonly Clock clock;
        private readonly Feedback feedback;

        public RollbackExecutor(ScriptStore scriptStore, SchemaHistory schemaHistory, Database database, Clock clock, Feedback feedback)
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
            feedback.WriteLine("Beginning rollback.");
            foreach (string name in plan)
            {
                feedback.WriteLine("Rolling back {0}...", name);
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
                database.ApplyScript(script.Rollback);
            }
            catch
            {
                feedback.WriteLine("Failed to roll back {0}.", name);
                throw;
            }
        }

        private void RecordSchemaHistory(string name)
        {
            try
            {
                var appliedScript = schemaHistory.GetDeployedScript(name);
                appliedScript.Rollback(clock.UtcNow);
                schemaHistory.SaveAppliedScript(appliedScript);
            }
            catch
            {
                feedback.WriteLine("Rolled back {0} successfully, but failed to record schema history for it.", name);
                throw;
            }
        }
    }
}
