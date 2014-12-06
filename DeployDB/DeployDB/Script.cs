using System;

namespace DeployDB
{
    public class Script
    {
        public Script(string name, string deploy, string rollback)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Must provide name.", "name");
            if (deploy == null)
                throw new ArgumentNullException("deploy");

            Name = name;
            Deploy = deploy;
            Rollback = rollback;
        }

        public string Name { get; private set; }
        public string Deploy { get; private set; }
        public string Rollback { get; private set; }

        public override string ToString()
        {
            string r = Rollback == null ? string.Empty : ",R";
            return string.Format("[{0} D{1}]", Name, r);
        }
    }
}
