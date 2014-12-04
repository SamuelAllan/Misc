using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB.Tests.Fakes
{
    public class FakeSchemaHistory: SchemaHistory
    {
        private Dictionary<string, AppliedScript> appliedScripts = new Dictionary<string, AppliedScript>();
        private List<AppliedScript> saved = new List<AppliedScript>();

        public void EnsureHistoryDeployed()
        {
            HistoryDeployed = true;
        }

        public IEnumerable<AppliedScript> GetAppliedScripts()
        {
            return appliedScripts.Values;
        }

        public void SaveAppliedScript(AppliedScript appliedScript)
        {
            appliedScripts[appliedScript.Name] = appliedScript;
            saved.Add(appliedScript);
        }

        public bool HistoryDeployed { get; private set; }

        public List<AppliedScript> Saved
        {
            get { return new List<AppliedScript>(saved); }
        }
        
        public void AddInitial(AppliedScript appliedScript)
        {
            appliedScripts[appliedScript.Name] = appliedScript;
        }
    }
}
