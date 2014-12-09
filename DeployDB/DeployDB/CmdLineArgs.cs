using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB
{
    public class CmdLineArgs
    {
        public const string Help = @"DeployDB has two modes: deploy and rollback.
    DeployDB deploy <args>
    DeployDB rollback <args>

    --destination <script>
        Optional, if not specified will deploy/rollback all.
        <script> = desired head script

    --db <dbType> <connectionString>
        Required
        <dbType> in { SqlServer }
        <connectionString> = connection string for DB

    --scripts <location>
        Required
        <location> = path to script folder

";

        private const string destinationArg = "--destination";
        private const string dbArg = "--db";
        private const string scriptsArg = "--scripts";

        public string Mode { get; set; }
        public string Destination { get; set; }
        public string DbType { get; set; }
        public string DbConnectionString { get; set; }
        public string ScriptLocation { get; set; }

        public static CmdLineArgs Parse(string[] args)
        {
            string mode = args[0];
            string destination = FindDestination(args);
            string dbType = FindDbType(args);
            string dbConnectionString = FindDbConnectionString(args);
            string scriptLocation = FindScriptLocation(args);
            return new CmdLineArgs
                {
                    Mode = mode,
                    Destination = destination,
                    DbType = dbType,
                    DbConnectionString = dbConnectionString,
                    ScriptLocation = scriptLocation,
                };
        }

        private static string FindDestination(string[] args)
        {
            int location = FindIndexOf(args, destinationArg);
            if (location < 0)
                return null;
            int destinationIndex = location + 1;
            if (destinationIndex >= args.Length)
                throw new Exception("Did not include destination.");
            string destination = args[destinationIndex];
            if (destination.StartsWith("--"))
                throw new Exception("Did not include destination.");
            return destination;
        }

        private static int FindIndexOf(string[] args, string argName)
        {
            int result = -1;
            for (int i = 0; i < args.Length; i++)
            {
                if (string.Equals(args[i], argName, StringComparison.InvariantCultureIgnoreCase))
                    if (result < 0)
                        result = i;
                    else
                        throw new Exception("Passed multiple of: " + argName);
            }
            return result;
        }

        private static string FindDbType(string[] args)
        {
            int location = FindIndexOf(args, dbArg);
            if (location < 0)
                throw new Exception("Did not provide DB args.");
            int dbTypeIndex = location + 1;
            if (dbTypeIndex >= args.Length)
                throw new Exception("Did not include DB type.");
            string dbType = args[dbTypeIndex];
            if (dbType.StartsWith("--"))
                throw new Exception("Did not include DB type.");
            return dbType;
        }

        private static string FindDbConnectionString(string[] args)
        {
            int location = FindIndexOf(args, dbArg);
            if (location < 0)
                throw new Exception("Did not provide DB args.");
            int dbConnectionStringIndex = location + 2;
            if (dbConnectionStringIndex >= args.Length)
                throw new Exception("Did not include DB connection string.");
            string dbConnectionString = args[dbConnectionStringIndex];
            if (dbConnectionString.StartsWith("--"))
                throw new Exception("Did not include DB connection string.");
            return dbConnectionString;
        }

        private static string FindScriptLocation(string[] args)
        {
            int location = FindIndexOf(args, scriptsArg);
            if (location < 0)
                throw new Exception("Did not provide scripts arg.");
            int scriptsLocationIndex = location + 1;
            if (scriptsLocationIndex >= args.Length)
                throw new Exception("Did not include scripts location.");
            string scriptsLocation = args[scriptsLocationIndex];
            if (scriptsLocation.StartsWith("--"))
                throw new Exception("Did not include DB scripts location.");
            return scriptsLocation;
        }
    }
}
