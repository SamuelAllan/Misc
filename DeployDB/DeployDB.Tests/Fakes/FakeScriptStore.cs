using System.Collections.Generic;

namespace DeployDB.Tests.Fakes
{
    public class FakeScriptStore: ScriptStore
    {
        private Dictionary<string, Script> scripts = new Dictionary<string,Script>();

        public void Add(Script script)
        {
            scripts.Add(script.Name, script);
        }

        public Script this[string name]
        {
            get { return scripts[name]; }
        }

        public IEnumerable<Script> Scripts
        {
            get { return scripts.Values; }
        }
    }
}
