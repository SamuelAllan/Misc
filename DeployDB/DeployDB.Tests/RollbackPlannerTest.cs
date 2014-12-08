using DeployDB.Tests.Fakes;

using NUnit.Framework;

namespace DeployDB.Tests
{
    [TestFixture]
    public class RollbackPlannerTest
    {
        private FakeScriptStore scriptStore;
        private FakeSchemaHistory schemaHistory;
        private RollbackPlanner planner;
        private Clock clock;

        [SetUp]
        public void Setup()
        {
            scriptStore = new FakeScriptStore();
            schemaHistory = new FakeSchemaHistory();
            planner = new RollbackPlanner(scriptStore, schemaHistory);
            clock = new AutoIncrementClock();
        }

        private void AddScriptToStore(string name)
        {
            scriptStore.Add(new Script(name, "ABC", "DEF"));
        }

        private void AddScriptWithoutRollbackToStore(string name)
        {
            scriptStore.Add(new Script(name, "ABC", null));
        }

        private void AddScriptToHistory(string name)
        {
            var appliedScript = new AppliedScript(name, clock.UtcNow, null);
            schemaHistory.AddInitial(appliedScript);
        }

        private void AddRolledBackScriptToHistory(string name)
        {
            var appliedScript = new AppliedScript(name, clock.UtcNow, clock.UtcNow);
            schemaHistory.AddInitial(appliedScript);
        }

        [Test]
        public void NoSchemaHistory_NothingToRollBack()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");

            var plan = planner.MakePlan(null);
            CollectionAssert.IsEmpty(plan);
        }

        [Test]
        public void DeployedSchemaHistory_AllDeployedScriptsInReverseAlphabeticalOrder()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");
            AddScriptToStore("004");

            AddScriptToHistory("001");
            AddScriptToHistory("002");

            var plan = planner.MakePlan(null);
            CollectionAssert.AreEqual(new[] { "002", "001" }, plan);
        }

        [Test]
        public void AllDeployedSchemaHistory_RollbackEverything()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");

            AddScriptToHistory("001");
            AddScriptToHistory("002");
            AddScriptToHistory("003");

            var plan = planner.MakePlan(null);
            CollectionAssert.AreEqual(new[] { "003", "002", "001" }, plan);
        }

        [Test]
        public void RolledBackHistory_AlreadyRolledBackScriptsAreIgnored()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");
            AddScriptToStore("004");

            AddScriptToHistory("001");
            AddRolledBackScriptToHistory("002");
            AddScriptToHistory("003");
            AddRolledBackScriptToHistory("004");

            var plan = planner.MakePlan(null);
            CollectionAssert.AreEqual(new[] { "003", "001" }, plan);
        }

        [Test]
        public void RolledBackHistory_RolledBackThenRedeployedScriptsAreRolledBack()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");

            AddRolledBackScriptToHistory("001");
            AddRolledBackScriptToHistory("001");  // Multiple rollbacks are a-okay
            AddScriptToHistory("001");

            var plan = planner.MakePlan(null);
            CollectionAssert.AreEqual(new[] { "001" }, plan);
        }

        [Test]
        public void RolledBackHistoryThatsNotInScriptStoreIsIgnored()
        {
            // Ignored because while it was unknown state, it has been rolled back, so is gone now.

            AddScriptToStore("001");
            AddScriptToStore("002");

            AddScriptToHistory("001");
            AddScriptToHistory("002");
            AddRolledBackScriptToHistory("ABC");

            var plan = planner.MakePlan(null);
            CollectionAssert.AreEqual(new[] { "002", "001" }, plan);
        }

        [Test]
        public void RolledBackHistoryForWhichThereIsNowOnlyADeployIsIgnored()
        {
            AddScriptToStore("001");
            AddScriptWithoutRollbackToStore("002");

            AddScriptToHistory("001");
            AddRolledBackScriptToHistory("002");

            var plan = planner.MakePlan(null);
            CollectionAssert.AreEqual(new[] { "001" }, plan);
        }

        [Test, ExpectedException]
        public void DeployedHistoryThatsNotInScriptStore_ThrowBecauseThisDbHasUnknownState()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");

            AddScriptToHistory("001");
            AddScriptToHistory("002");
            AddScriptToHistory("003");

            planner.MakePlan(null);
        }

        [Test, ExpectedException]
        public void DeployedHistoryWhichHasNoRollbackThrows()
        {
            // Because we don't know how to rollback as far as was requested (all the way)
            AddScriptToStore("001");
            AddScriptWithoutRollbackToStore("002");
            AddScriptToStore("003");

            AddScriptToHistory("001");
            AddScriptToHistory("002");
            AddScriptToHistory("003");

            planner.MakePlan(null);
        }

        [Test]
        public void NonNullDestinationStopsPlansToLeaveDestinationDeployed()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");

            AddScriptToHistory("001");
            AddScriptToHistory("002");
            AddScriptToHistory("003");

            var plan = planner.MakePlan("002");
            CollectionAssert.AreEqual(new[] { "003" }, plan);
        }

        [Test]
        public void NonNullDestinationSameAsFirstScriptLeavesFirstScriptDeployed()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");

            AddScriptToHistory("001");
            AddScriptToHistory("002");
            AddScriptToHistory("003");

            var plan = planner.MakePlan("001");
            CollectionAssert.AreEqual(new[] { "003", "002" }, plan);
        }

        [Test]
        public void DeployedHistoryWhichHasNoRollbackButDoesNotNeedToBeRolledBackIsIgnored()
        {
            // Because we can at least get back to the destination
            AddScriptWithoutRollbackToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");

            AddScriptToHistory("001");
            AddScriptToHistory("002");
            AddScriptToHistory("003");

            var plan = planner.MakePlan("001");
            CollectionAssert.AreEqual(new[] { "003", "002" }, plan);
        }

        [Test, ExpectedException]
        public void DeployedHistoryWhichHasNoRollbackButDoesNeedToBeRolledBackThrows()
        {
            // Because we don't know how to rollback as far as was requested
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptWithoutRollbackToStore("003");

            AddScriptToHistory("001");
            AddScriptToHistory("002");
            AddScriptToHistory("003");

            planner.MakePlan("002");
        }

        [Test, ExpectedException]
        public void DestinationIsNotDeployedThrowsToProvideFeedback()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");

            AddScriptToHistory("001");
            AddScriptToHistory("002");

            planner.MakePlan("003");
        }
    }
}
