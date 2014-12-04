using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB
{
    public interface Feedback
    {
        void WriteLine();
        void WriteLine(string line);
        void WriteLine(string format, params object[] args);
    }
}
