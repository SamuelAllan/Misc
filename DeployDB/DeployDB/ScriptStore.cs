using System.Collections.Generic;

namespace DeployDB
{
    public interface ScriptStore
    {
        Script this[string name] { get; }
        IEnumerable<Script> Scripts { get; }
    }
}
