using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeployDB.Tests.Fakes;

using NUnit.Framework;

namespace DeployDB.Tests
{
    [TestFixture]
    public class DeployerTest
    {
        private static readonly DateTime now = new DateTime(2014, 12, 4, 15, 29, 33, 123);
        private FakeScriptStore scriptStore;
        private FakeSchemaHistory schemaHistory;
        private FakeDatabase database;
        private FakeClock clock;
        private FakeFeedback feedback;
        private Deployer deployer;

        [SetUp]
        public void Setup()
        {
            scriptStore = new FakeScriptStore();
            schemaHistory = new FakeSchemaHistory();
            database = new FakeDatabase();
            clock = new FakeClock();
            feedback = new FakeFeedback();
            deployer = new Deployer(scriptStore, schemaHistory, database, clock, feedback);
        }

        [Test]
        public void NoScriptsToDeploy_NoSchemaHistory()
        {
            deployer.Deploy();
            Assert.IsTrue(schemaHistory.HistoryDeployed, "History table still gets deployed");
            Assert.AreEqual(0, database.AppliedScripts.Count, "No scripts should be applied to DB");
            Assert.AreEqual(0, schemaHistory.Saved.Count, "No records of script running should be saved.");
        }

        [Test]
        public void ScriptsToDeploy_NoSchemaHistory()
        {
            scriptStore.Add(new Script("001", "ABC", "DEF"));
            scriptStore.Add(new Script("002", "GHI", "JKL"));

            deployer.Deploy();
            Assert.IsTrue(schemaHistory.HistoryDeployed, "History table gets deployed");
            CollectionAssert.AreEqual(new [] { "ABC", "GHI" }, database.AppliedScripts, "Deploy scripts applied in alphabetical order");
            // TODO: Work up the motivation to actually write some unit tests for this.
        }
    }
}
