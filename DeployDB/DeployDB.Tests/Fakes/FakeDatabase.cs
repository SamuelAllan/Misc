using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB.Tests.Fakes
{
    public class FakeDatabase: Database
    {
        private List<string> appliedScripts = new List<string>();

        public void ApplyScript(string script)
        {
            appliedScripts.Add(script);
        }

        public List<string> AppliedScripts
        {
            get { return new List<string>(appliedScripts); }
        }
    }
}
