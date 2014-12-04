using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB.Disk
{
    public class DiskScriptStore: ScriptStore
    {
        private const string deployFileSearchPattern = "*.deploy.sql";
        private const int deployFileExtensionLength = 11;
        private const string rollbackFileSearchPattern = "*.rollback.sql";
        private const int rollbackFileExtensionLength = 13;

        private readonly Lazy<Dictionary<string, Script>> scripts;

        public DiskScriptStore(string dir)
        {
            if (dir == null)
                throw new ArgumentNullException("dir");
            scripts = new Lazy<Dictionary<string, Script>>(() => ReadScriptsOffDisk(dir).ToDictionary(x => x.Name));
        }

        private static IEnumerable<Script> ReadScriptsOffDisk(string dir)
        {
            if (!Directory.Exists(dir))
                yield break;

            Dictionary<string, string> rollbacks = GetAllRollbacks(dir);

            foreach (KeyValuePair<string, string> deploy in GetAllDeploys(dir))
            {
                if (rollbacks.ContainsKey(deploy.Key))
                {
                    yield return new Script(deploy.Key, deploy.Value, rollbacks[deploy.Key]);
                    rollbacks.Remove(deploy.Key);
                }
                else
                {
                    yield return new Script(deploy.Key, deploy.Value, null);
                }
            }

            if (rollbacks.Count > 0)
                throw new Exception("There are rollback scripts without matching deploy scripts!");
        }

        private static Dictionary<string, string> GetAllRollbacks(string dir)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (string file in Directory.EnumerateFiles(dir, rollbackFileSearchPattern, SearchOption.AllDirectories))
            {
                string name = GetNameFromRollbackFilePath(file);
                string script = File.ReadAllText(file);
                result.Add(name, script);
            }
            return result;
        }

        private static string GetNameFromRollbackFilePath(string file)
        {
            string name = Path.GetFileName(file);
            return name.Substring(0, name.Length - rollbackFileExtensionLength);
        }

        private static IEnumerable<KeyValuePair<string, string>> GetAllDeploys(string dir)
        {
            foreach (string file in Directory.EnumerateFiles(dir, deployFileSearchPattern, SearchOption.TopDirectoryOnly))
            {
                string name = GetNameFromDeployFilePath(file);
                string script = File.ReadAllText(file);
                yield return new KeyValuePair<string, string>(name, script);
            }
        }

        private static string GetNameFromDeployFilePath(string file)
        {
            string name = Path.GetFileName(file);
            return name.Substring(0, name.Length - deployFileExtensionLength);
        }


        public Script this[string name]
        {
            get { return scripts.Value[name]; }
        }


        public IEnumerable<Script> Scripts
        {
            get { return scripts.Value.Values; }
        }
    }
}
