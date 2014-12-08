using System;
using System.Collections.Generic;
using System.Linq;

namespace DeployDB.Tests.Fakes
{
    public class FakeSchemaHistory: SchemaHistory
    {
        private List<AppliedScript> appliedScripts = new List<AppliedScript>();
        private List<AppliedScript> saved = new List<AppliedScript>();
        private HashSet<string> failSaving = new HashSet<string>();

        public void EnsureHistoryDeployed()
        {
            HistoryDeployed = true;
        }

        public IEnumerable<AppliedScript> GetAppliedScripts()
        {
            return appliedScripts;
        }

        public AppliedScript GetDeployedScript(string name)
        {
            return appliedScripts.SingleOrDefault(x => x.Name == name && x.RollbackTime == null);
        }

        public void SaveAppliedScript(AppliedScript appliedScript)
        {
            appliedScripts.Add(appliedScript);
            saved.Add(appliedScript);

            if (failSaving.Contains(appliedScript.Name))
                throw new Exception("I Cease to Live!");
        }

        public bool HistoryDeployed { get; private set; }

        public List<AppliedScript> Saved
        {
            get { return new List<AppliedScript>(saved); }
        }
        
        public void AddInitial(AppliedScript appliedScript)
        {
            appliedScripts.Add(appliedScript);
        }

        public void FailSaving(string name)
        {
            failSaving.Add(name);
        }
    }
}
