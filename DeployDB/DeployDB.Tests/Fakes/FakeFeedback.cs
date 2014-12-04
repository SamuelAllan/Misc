using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeployDB.Tests.Fakes
{
    public class FakeFeedback: Feedback
    {
        private readonly StringBuilder feedback = new StringBuilder();
        
        public void WriteLine()
        {
            feedback.AppendLine();
        }

        public void WriteLine(string line)
        {
            feedback.AppendLine(line);
        }

        public void WriteLine(string format, params object[] args)
        {
            feedback.AppendFormat(format, args);
            feedback.AppendLine();
        }

        public string Output
        {
            get { return feedback.ToString(); }
        }
    }
}
