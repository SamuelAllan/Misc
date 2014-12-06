using System;
using System.Collections.Generic;

namespace DeployDB.Tests.Fakes
{
    public class FakeDatabase: Database
    {
        private List<string> appliedScripts = new List<string>();
        private HashSet<string> failOn = new HashSet<string>();

        public void ApplyScript(string script)
        {
            appliedScripts.Add(script);

            if (failOn.Contains(script))
                throw new Exception("I Die!");
        }

        public List<string> AppliedScripts
        {
            get { return new List<string>(appliedScripts); }
        }

        public void FailOn(string script)
        {
            failOn.Add(script);
        }
    }
}
