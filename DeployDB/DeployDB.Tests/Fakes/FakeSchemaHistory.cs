using System;
using System.Collections.Generic;

namespace DeployDB.Tests.Fakes
{
    public class FakeSchemaHistory: SchemaHistory
    {
        private Dictionary<string, AppliedScript> appliedScripts = new Dictionary<string, AppliedScript>();
        private List<AppliedScript> saved = new List<AppliedScript>();
        private HashSet<string> failSaving = new HashSet<string>();

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
            appliedScripts[appliedScript.Name] = appliedScript;
        }

        public void FailSaving(string name)
        {
            failSaving.Add(name);
        }
    }
}
