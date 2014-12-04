using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB
{
    public interface ScriptStore
    {
        Script this[string name] { get; }
        IEnumerable<Script> Scripts { get; }
    }
}
