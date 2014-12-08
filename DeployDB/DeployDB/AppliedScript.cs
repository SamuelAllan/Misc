using System;

namespace DeployDB
{
    public class AppliedScript
    {
        public AppliedScript(string name, DateTime deployTime, DateTime? rollbackTime)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Must provide name.", "name");
            Name = name;
            DeployTime = deployTime;
            RollbackTime = rollbackTime;
        }

        public string Name { get; private set; }
        public DateTime DeployTime { get; private set; }
        public DateTime? RollbackTime { get; private set; }

        public void Rollback(DateTime time)
        {
            if (RollbackTime != null)
                throw new InvalidOperationException("Already rolled back!");
            RollbackTime = time;
        }

        /// <summary>
        /// Not for public display, but rather to make debuging nicer.
        /// </summary>
        public override string ToString()
        {
            return string.Format("[{0} {1:yyyy-MM-dd hh:mm:ss {2:yyyy-MM-dd hh:mm:ss}]", Name, DeployTime, RollbackTime);
        }
    }
}
