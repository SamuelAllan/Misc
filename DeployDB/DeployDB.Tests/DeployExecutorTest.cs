using System;
using System.Collections.Generic;
using System.Linq;

using DeployDB.Tests.Fakes;

using NUnit.Framework;

namespace DeployDB.Tests
{
    [TestFixture]
    public class DeployExecutorTest
    {
        private static readonly DateTime now = new DateTime(2014, 12, 4, 15, 29, 33, 123);
        private FakeScriptStore scriptStore;
        private FakeSchemaHistory schemaHistory;
        private FakeDatabase database;
        private AutoIncrementClock clock;
        private FakeFeedback feedback;
        private DeployExecutor deployer;

        [SetUp]
        public void Setup()
        {
            scriptStore = new FakeScriptStore();
            schemaHistory = new FakeSchemaHistory();
            database = new FakeDatabase();
            clock = new AutoIncrementClock(now);
            feedback = new FakeFeedback();
            deployer = new DeployExecutor(scriptStore, schemaHistory, database, clock, feedback);
        }

        [Test]
        public void DeploysAllScriptsPlannedInOrder()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            string[] plan = new[] { "001", "002", "003" };

            deployer.Execute(plan);

            CollectionAssert.AreEqual(new[] { "AAA", "CCC", "EEE" }, database.AppliedScripts);
        }

        [Test]
        public void DeploysOnlyScriptsPlanned()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            string[] plan = new[] { "001", "003" };

            deployer.Execute(plan);

            CollectionAssert.AreEqual(new[] { "AAA", "EEE" }, database.AppliedScripts);
        }

        [Test]
        public void DeployOrderComesFromPlan()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            string[] plan = new[] { "003", "001", "002" };

            deployer.Execute(plan);

            CollectionAssert.AreEqual(new[] { "EEE", "AAA", "CCC" }, database.AppliedScripts);
        }

        [Test]
        public void IfAScriptFailsDoesNotDeployFurtherScripts()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            database.FailOn("CCC");

            string[] plan = new[] { "001", "002", "003" };

            ExecutePlanAndSuckUpErrors(plan);

            CollectionAssert.AreEqual(new[] { "AAA", "CCC" }, database.AppliedScripts);
        }

        private void ExecutePlanAndSuckUpErrors(IEnumerable<string> plan)
        {
            try
            {
                deployer.Execute(plan);
            }
            catch
            {
            }
        }

        [Test]
        public void IfFailToRecordAScriptsApplicationDoNotDeployFurtherScripts()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            schemaHistory.FailSaving("002");

            string[] plan = new[] { "001", "002", "003" };

            ExecutePlanAndSuckUpErrors(plan);

            CollectionAssert.AreEqual(new[] { "AAA", "CCC" }, database.AppliedScripts);
        }

        [Test]
        public void RecordsDeployedScripts()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));

            string[] plan = new[] { "001", "002" };

            deployer.Execute(plan);

            var one = schemaHistory.Saved[0];
            Assert.AreEqual("001", one.Name, "one.Name");
            Assert.AreEqual(now, one.DeployTime, "one.DeployTime");
            Assert.IsNull(one.RollbackTime, "one.RollbackTime");

            var two = schemaHistory.Saved[1];
            Assert.AreEqual("002", two.Name, "two.Name");
            Assert.AreEqual(now.AddSeconds(1), two.DeployTime, "two.DeployTime");
            Assert.IsNull(two.RollbackTime, "two.RollbackTime");
        }

        [Test]
        public void IfAScriptFailsDoNotRecordThatScriptAsApplied()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            database.FailOn("CCC");

            string[] plan = new[] { "001", "002", "003" };

            ExecutePlanAndSuckUpErrors(plan);

            CollectionAssert.AreEquivalent(new[] { "001" }, schemaHistory.Saved.Select(x => x.Name));
        }

        [Test]
        public void FeedbackBeforeAttemptingToDeployEachScript()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));

            string[] plan = new[] { "001", "002" };

            deployer.Execute(plan);

            string output = feedback.Output;
            string expected = @"Beginning deployment.
Deploying 001...
Deploying 002...
Done.
";
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void IfAScriptFailsToDeployThereShouldBeFeedbackToIndicateThis()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            database.FailOn("CCC");

            string[] plan = new[] { "001", "002", "003" };

            ExecutePlanAndSuckUpErrors(plan);

            string output = feedback.Output;
            string expected = @"Beginning deployment.
Deploying 001...
Deploying 002...
Failed to deploy 002.
"; // Details of the exception should be output where the exception is finally caught.
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void IfAScriptFailsToRecordDeploymentThereShouldBeFeedbackToIndicateThis()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            schemaHistory.FailSaving("002");

            string[] plan = new[] { "001", "002", "003" };

            ExecutePlanAndSuckUpErrors(plan);

            string output = feedback.Output;
            string expected = @"Beginning deployment.
Deploying 001...
Deploying 002...
Deployed 002 successfully, but failed to record schema history for it.
"; // Details of the exception should be output where the exception is finally caught.
            Assert.AreEqual(expected, output);
        }
    }
}
