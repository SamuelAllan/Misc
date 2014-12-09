using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeployDB.Disk;
using DeployDB.SqlServer;
using DeployDB.System;

namespace DeployDB
{
    public class Factory
    {
        private readonly Feedback feedback;
        private readonly Clock clock;
        private readonly ScriptStore scriptStore;
        private readonly SchemaHistory schemaHistory;
        private readonly Database database;
        private readonly Planner planner;
        private readonly Executor executor;

        public Factory(CmdLineArgs args)
        {
            if (args == null)
                throw new ArgumentNullException("args");
            
            feedback = new ConsoleFeedback();
            clock = new SystemClock();
            scriptStore = new DiskScriptStore(args.ScriptLocation);

            if (!string.Equals(args.DbType, "SqlServer", StringComparison.InvariantCultureIgnoreCase))
                throw new Exception("Don't recognise DB type: " + args.DbType);
            SqlServerDatabase sqlDatabase = new SqlServerDatabase(args.DbConnectionString);
            database = sqlDatabase;
            schemaHistory = sqlDatabase;

            if (string.Equals("deploy", args.Mode, StringComparison.InvariantCultureIgnoreCase))
            {
                planner = new DeployPlanner(scriptStore, schemaHistory);
                executor = new DeployExecutor(scriptStore, schemaHistory, database, clock, feedback);
            }
            else if (string.Equals("rollback", args.Mode, StringComparison.InvariantCultureIgnoreCase))
            {
                planner = new RollbackPlanner(scriptStore, schemaHistory);
                executor = new RollbackExecutor(scriptStore, schemaHistory, database, clock, feedback);
            }
            else
            {
                throw new Exception("Don't recognise mode: " + args.Mode);
            }
        }

        public Planner GetPlanner()
        {
            return planner;
        }

        public Executor GetExecutor()
        {
            return executor;
        }

        public SchemaHistory GetSchemaHistory()
        {
            return schemaHistory;
        }
    }
}
