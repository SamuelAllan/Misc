using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB
{
    public class AppliedScript
    {
        public string Name { get; set; }
        public DateTime DeployTime { get; set; }
        public DateTime? RollbackTime { get; set; }
    }
}
