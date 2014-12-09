using System;

namespace DeployDB
{
    class Program
    {
        static int Main(string[] args)
        {
            Planner planner;
            Executor executor;
            string destination;
            try
            {
                CmdLineArgs cmdLineArgs = CmdLineArgs.Parse(args);
                Factory factory = new Factory(cmdLineArgs);
                planner = factory.GetPlanner();
                executor = factory.GetExecutor();
                destination = cmdLineArgs.Destination;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(CmdLineArgs.Help);
                return -1;
            }

            try
            {
                var plan = planner.MakePlan(destination);
                executor.Execute(plan);
                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("ERROR!");
                return -1;
            }
        }
    }
}
