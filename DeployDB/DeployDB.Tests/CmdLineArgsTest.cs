using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace DeployDB.Tests
{
    [TestFixture]
    public class CmdLineArgsTest
    {
        [Test]
        public void Parse_BaselineDeploy()
        {
            string[] args = new[] { "deploy", "--destination", "001", "--db", "SqlServer", "db connection string", "--scripts", @"C:\Temp\scripts" };
            CmdLineArgs result = CmdLineArgs.Parse(args);

            Assert.IsNotNull(result, "result");
            Assert.AreEqual("deploy", result.Mode, "Mode");
            Assert.AreEqual("001", result.Destination, "Destination");
            Assert.AreEqual("SqlServer", result.DbType, "DbType");
            Assert.AreEqual("db connection string", result.DbConnectionString, "DbConnectionString");
            Assert.AreEqual(@"C:\Temp\scripts", result.ScriptLocation, "ScriptLocation");
        }

        [Test]
        public void Parse_DestinationNotSpecified()
        {
            string[] args = new[] { "deploy", "--db", "SqlServer", "db connection string", "--scripts", @"C:\Temp\scripts" };
            CmdLineArgs result = CmdLineArgs.Parse(args);

            Assert.IsNotNull(result, "result");
            Assert.AreEqual("deploy", result.Mode, "Mode");
            Assert.IsNull(result.Destination, "Destination");
            Assert.AreEqual("SqlServer", result.DbType, "DbType");
            Assert.AreEqual("db connection string", result.DbConnectionString, "DbConnectionString");
            Assert.AreEqual(@"C:\Temp\scripts", result.ScriptLocation, "ScriptLocation");
        }

        [Test]
        public void Parse_ArgsInADifferentOrder()
        {
            string[] args = new[] { "deploy", "--scripts", @"C:\Temp\scripts", "--db", "SqlServer", "db connection string", "--destination", "001", };
            CmdLineArgs result = CmdLineArgs.Parse(args);

            Assert.IsNotNull(result, "result");
            Assert.AreEqual("deploy", result.Mode, "Mode");
            Assert.AreEqual("001", result.Destination, "Destination");
            Assert.AreEqual("SqlServer", result.DbType, "DbType");
            Assert.AreEqual("db connection string", result.DbConnectionString, "DbConnectionString");
            Assert.AreEqual(@"C:\Temp\scripts", result.ScriptLocation, "ScriptLocation");
        }

        [Test]
        public void Parse_ArgNamesNotCaseSensitiveDeploy()
        {
            string[] args = new[] { "DEploY", "--DestINAtion", "aBcDEf", "--Db", "sQLseRVEr", "db connection string", "--SCRipTS", @"C:\Temp\scripts" };
            CmdLineArgs result = CmdLineArgs.Parse(args);

            Assert.IsNotNull(result, "result");
            Assert.AreEqual("DEploY", result.Mode, "Mode");
            Assert.AreEqual("aBcDEf", result.Destination, "Destination");
            Assert.AreEqual("sQLseRVEr", result.DbType, "DbType");
            Assert.AreEqual("db connection string", result.DbConnectionString, "DbConnectionString");
            Assert.AreEqual(@"C:\Temp\scripts", result.ScriptLocation, "ScriptLocation");
        }

        [Test, ExpectedException]
        public void Parse_DbNotSpecified()
        {
            string[] args = new[] { "deploy", "--destination", "001", "--scripts", @"C:\Temp\scripts" };
            CmdLineArgs.Parse(args);
        }

        [Test, ExpectedException]
        public void Parse_ScriptsNotSpecified()
        {
            string[] args = new[] { "deploy", "--destination", "001", "--db", "SqlServer", "db connection string" };
            CmdLineArgs.Parse(args);
        }

        [Test, ExpectedException]
        public void Parse_MissingDestinationArgument()
        {
            string[] args = new[] { "deploy", "--destination", "--db", "SqlServer", "db connection string", "--scripts", @"C:\Temp\scripts" };
            CmdLineArgs.Parse(args);
        }

        [Test, ExpectedException]
        public void Parse_MissingDbTypeArgument()
        {
            string[] args = new[] { "deploy", "--destination", "001", "--db", "db connection string", "--scripts", @"C:\Temp\scripts" };
            CmdLineArgs.Parse(args);
        }

        [Test, ExpectedException]
        public void Parse_MissingDbConnectionStringArgument()
        {
            string[] args = new[] { "deploy", "--destination", "001", "--db", "SqlServer", "--scripts", @"C:\Temp\scripts" };
            CmdLineArgs.Parse(args);
        }

        [Test, ExpectedException]
        public void Parse_MissingScriptLocationArgument()
        {
            string[] args = new[] { "deploy", "--destination", "001", "--db", "SqlServer", "db connection string", "--scripts" };
            CmdLineArgs.Parse(args);
        }

        [Test, ExpectedException]
        public void Parse_DoubleDestination()
        {
            string[] args = new[] { "deploy", "--destination", "001", "--db", "SqlServer", "db connection string", "--scripts", @"C:\Temp\scripts", "--destination", "002" };
            CmdLineArgs.Parse(args);
        }

        [Test, ExpectedException]
        public void Parse_DoubleDb()
        {
            string[] args = new[] { "deploy", "--destination", "001", "--db", "SqlServer", "db connection string", "--scripts", @"C:\Temp\scripts", "--db", "SqlServer2", "db connection string 2" };
            CmdLineArgs.Parse(args);
        }

        [Test, ExpectedException]
        public void Parse_DoubleScripts()
        {
            string[] args = new[] { "deploy", "--destination", "001", "--db", "SqlServer", "db connection string", "--scripts", @"C:\Temp\scripts", "--scripts", @"C:\Temp\scripts2" };
            CmdLineArgs.Parse(args);
        }
    }
}
