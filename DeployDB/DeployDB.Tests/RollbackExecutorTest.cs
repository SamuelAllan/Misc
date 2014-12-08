using System;
using System.Collections.Generic;
using System.Linq;

using DeployDB.Tests.Fakes;

using NUnit.Framework;

namespace DeployDB.Tests
{
    [TestFixture]
    public class RollbackExecutorTest
    {
        private static readonly DateTime deployTime = new DateTime(2014, 12, 3, 15, 29, 33, 456);
        private static readonly DateTime now = new DateTime(2014, 12, 4, 15, 29, 33, 123);
        private FakeScriptStore scriptStore;
        private FakeSchemaHistory schemaHistory;
        private FakeDatabase database;
        private AutoIncrementClock clock;
        private FakeFeedback feedback;
        private RollbackExecutor rollbacker;

        [SetUp]
        public void Setup()
        {
            scriptStore = new FakeScriptStore();
            schemaHistory = new FakeSchemaHistory();
            database = new FakeDatabase();
            clock = new AutoIncrementClock(now);
            feedback = new FakeFeedback();
            rollbacker = new RollbackExecutor(scriptStore, schemaHistory, database, clock, feedback);
        }

        private void RecordPriorDeployment(string name)
        {
            var appliedScript = new AppliedScript(name, deployTime, null);
            schemaHistory.AddInitial(appliedScript);
        }

        [Test]
        public void RollsBackAllScriptsPlannedInOrder()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            RecordPriorDeployment("001");
            RecordPriorDeployment("002");
            RecordPriorDeployment("003");

            string[] plan = new[] { "003", "002", "001" };

            rollbacker.Execute(plan);

            CollectionAssert.AreEqual(new[] { "FFF", "DDD", "BBB" }, database.AppliedScripts);
        }

        [Test]
        public void RollsBackOnlyScriptsPlanned()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            RecordPriorDeployment("001");
            RecordPriorDeployment("002");
            RecordPriorDeployment("003");

            string[] plan = new[] { "003", "001" };

            rollbacker.Execute(plan);

            CollectionAssert.AreEqual(new[] { "FFF", "BBB" }, database.AppliedScripts);
        }

        [Test]
        public void RollbackOrderComesFromPlan()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            RecordPriorDeployment("001");
            RecordPriorDeployment("002");
            RecordPriorDeployment("003");

            string[] plan = new[] { "003", "001", "002" };

            rollbacker.Execute(plan);

            CollectionAssert.AreEqual(new[] { "FFF", "BBB", "DDD" }, database.AppliedScripts);
        }

        [Test]
        public void IfAScriptFailsDoesNotRollBackFurtherScripts()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            RecordPriorDeployment("001");
            RecordPriorDeployment("002");
            RecordPriorDeployment("003");

            database.FailOn("DDD");

            string[] plan = new[] { "003", "002", "001" };

            ExecutePlanAndSuckUpErrors(plan);

            CollectionAssert.AreEqual(new[] { "FFF", "DDD" }, database.AppliedScripts);
        }

        private void ExecutePlanAndSuckUpErrors(IEnumerable<string> plan)
        {
            try
            {
                rollbacker.Execute(plan);
            }
            catch
            {
            }
        }

        [Test]
        public void IfFailToRecordAScriptsApplicationDoNotRollBackFurtherScripts()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            RecordPriorDeployment("001");
            RecordPriorDeployment("002");
            RecordPriorDeployment("003");

            schemaHistory.FailSaving("002");

            string[] plan = new[] { "003", "002", "001" };

            ExecutePlanAndSuckUpErrors(plan);

            CollectionAssert.AreEqual(new[] { "FFF", "DDD" }, database.AppliedScripts);
        }

        [Test]
        public void RecordsRolledBackScripts()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));

            RecordPriorDeployment("001");
            RecordPriorDeployment("002");

            string[] plan = new[] { "002", "001" };

            rollbacker.Execute(plan);

            var one = schemaHistory.Saved[0];
            Assert.AreEqual("002", one.Name, "one.Name");
            Assert.AreEqual(deployTime, one.DeployTime, "one.DeployTime");
            Assert.AreEqual(now, one.RollbackTime, "one.RollbackTime");

            var two = schemaHistory.Saved[1];
            Assert.AreEqual("001", two.Name, "two.Name");
            Assert.AreEqual(deployTime, two.DeployTime, "two.DeployTime");
            Assert.AreEqual(now.AddSeconds(1), two.RollbackTime, "two.RollbackTime");
        }

        [Test]
        public void IfAScriptFailsDoNotRecordThatScriptAsRolledBack()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            RecordPriorDeployment("001");
            RecordPriorDeployment("002");
            RecordPriorDeployment("003");

            database.FailOn("DDD");

            string[] plan = new[] { "003", "002", "001" };

            ExecutePlanAndSuckUpErrors(plan);

            CollectionAssert.AreEquivalent(new[] { "003" }, schemaHistory.Saved.Select(x => x.Name));
        }

        [Test]
        public void FeedbackBeforeAttemptingToRollBackEachScript()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));

            RecordPriorDeployment("001");
            RecordPriorDeployment("002");

            string[] plan = new[] { "002", "001" };

            rollbacker.Execute(plan);

            string output = feedback.Output;
            string expected = @"Beginning rollback.
Rolling back 002...
Rolling back 001...
Done.
";
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void IfAScriptFailsToRollBackThereShouldBeFeedbackToIndicateThis()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            RecordPriorDeployment("001");
            RecordPriorDeployment("002");
            RecordPriorDeployment("003");

            database.FailOn("DDD");

            string[] plan = new[] { "003", "002", "001" };

            ExecutePlanAndSuckUpErrors(plan);

            string output = feedback.Output;
            string expected = @"Beginning rollback.
Rolling back 003...
Rolling back 002...
Failed to roll back 002.
"; // Details of the exception should be output where the exception is finally caught.
            Assert.AreEqual(expected, output);
        }

        [Test]
        public void IfAScriptFailsToRecordRollbackThereShouldBeFeedbackToIndicateThis()
        {
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            RecordPriorDeployment("001");
            RecordPriorDeployment("002");
            RecordPriorDeployment("003");

            schemaHistory.FailSaving("002");

            string[] plan = new[] { "003", "002", "001" };

            ExecutePlanAndSuckUpErrors(plan);

            string output = feedback.Output;
            string expected = @"Beginning rollback.
Rolling back 003...
Rolling back 002...
Rolled back 002 successfully, but failed to record schema history for it.
"; // Details of the exception should be output where the exception is finally caught.
            Assert.AreEqual(expected, output);
        }

        [Test, ExpectedException]
        public void IfThereIsNoRecordOfDeploymentForAScriptThrow()
        {
            // A properly constructed rollback plan will never have this problem.
            scriptStore.Add(new Script("001", "AAA", "BBB"));
            scriptStore.Add(new Script("002", "CCC", "DDD"));
            scriptStore.Add(new Script("003", "EEE", "FFF"));

            RecordPriorDeployment("001");
            RecordPriorDeployment("003");

            string[] plan = new[] { "003", "002", "001" };

            rollbacker.Execute(plan);
        }
    }
}
