using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB
{
    public class Executor
    {
        private readonly ScriptStore scriptStore;
        private readonly SchemaHistory schemaHistory;
        private readonly Database database;
        private readonly Clock clock;
        private readonly Feedback feedback;

        public Executor(ScriptStore scriptStore, SchemaHistory schemaHistory, Database database, Clock clock, Feedback feedback)
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
    }
}
