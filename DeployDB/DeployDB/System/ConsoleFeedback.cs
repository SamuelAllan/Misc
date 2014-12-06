using System;

namespace DeployDB.System
{
    public class ConsoleFeedback: Feedback
    {
        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
    }
}
