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

        public string Mode { get; set; }
        public string Destination { get; set; }
        public string DbType { get; set; }
        public string DbConnectionString { get; set; }
        public string ScriptLocation { get; set; }

        public static bool TryParse(string[] args, out CmdLineArgs result)
        {
            string mode = args[0];
            string destination = FindDestination(args);

            result = null;
            return false;
        }

        private static string FindDestination(string[] args)
        {
            const string argName = "--destination";
            int location = Array.FindIndex
        }

        private int FindIndexOf(string[] args, string optionName)
        { }
    }
}
