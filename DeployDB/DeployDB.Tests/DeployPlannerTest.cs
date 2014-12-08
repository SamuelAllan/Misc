using DeployDB.Tests.Fakes;

using NUnit.Framework;

namespace DeployDB.Tests
{
    [TestFixture]
    public class DeployPlannerTest
    {
        private FakeScriptStore scriptStore;
        private FakeSchemaHistory schemaHistory;
        private DeployPlanner planner;
        private Clock clock;

        [SetUp]
        public void Setup()
        {
            scriptStore = new FakeScriptStore();
            schemaHistory = new FakeSchemaHistory();
            planner = new DeployPlanner(scriptStore, schemaHistory);
            clock = new AutoIncrementClock();
        }

        private void AddScriptToStore(string name)
        {
            scriptStore.Add(new Script(name, "ABC", "DEF"));
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
        public void NoSchemaHistory_AllScriptsInAlphabeticalOrder()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");

            var plan = planner.MakePlan(null);
            CollectionAssert.AreEqual(new[] { "001", "002" }, plan);
        }

        [Test]
        public void DeployedSchemaHistory_AllUndeployedScriptsInAlphabeticalOrder()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");
            AddScriptToStore("004");

            AddScriptToHistory("001");

            var plan = planner.MakePlan(null);
            CollectionAssert.AreEqual(new[] { "002", "003", "004" }, plan);
        }

        [Test]
        public void AllDeployedSchemaHistory_NothingPlanned()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");

            AddScriptToHistory("001");
            AddScriptToHistory("002");
            AddScriptToHistory("003");

            var plan = planner.MakePlan(null);
            CollectionAssert.IsEmpty(plan);
        }

        [Test]
        public void RolledBackHistory_RolledBackScriptsArePlannedForRedeployment()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");
            AddScriptToStore("004");

            AddScriptToHistory("001");
            AddRolledBackScriptToHistory("002");
            AddRolledBackScriptToHistory("002"); // Multiple rollbacks are a-okay
            AddRolledBackScriptToHistory("003");

            var plan = planner.MakePlan(null);
            CollectionAssert.AreEqual(new[] { "002", "003", "004" }, plan);
        }

        [Test]
        public void RolledBackHistory_RolledBackThenRedeployedScriptsAreNotDeployedAgain()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");

            AddRolledBackScriptToHistory("001");
            AddRolledBackScriptToHistory("001");  // Multiple rollbacks are a-okay
            AddScriptToHistory("001");

            var plan = planner.MakePlan(null);
            CollectionAssert.AreEqual(new[] { "002" }, plan);
        }

        [Test, ExpectedException]
        public void ExtraHistoryThatsNotInScriptStore_ThrowBecauseThisDbHasUnknownState()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");

            AddScriptToHistory("001");
            AddScriptToHistory("002");
            AddScriptToHistory("003");

            planner.MakePlan(null);
        }

        [Test]
        public void NonNullDestinationStopsPlanAfterDeployingDestination()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");

            var plan = planner.MakePlan("002");
            CollectionAssert.AreEqual(new[] { "001", "002" }, plan);
        }

        [Test]
        public void NonNullDestinationSameAsLastScriptDeploysAll()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");

            var plan = planner.MakePlan("003");
            CollectionAssert.AreEqual(new[] { "001", "002", "003" }, plan);
        }

        [Test, ExpectedException]
        public void DestinationIsAlreadyDeployedThrowsToProvideFeedback()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");

            AddScriptToHistory("001");
            AddScriptToHistory("002");

            planner.MakePlan("002");
        }

        [Test, ExpectedException]
        public void ThereIsNoScriptForDestinationThrows()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");
            AddScriptToStore("003");

            planner.MakePlan("ABC");
        }

        [Test, ExpectedException]
        public void ThereIsNoScriptForDestinationAndAllOtherScriptsAreDeployedThrows()
        {
            AddScriptToStore("001");
            AddScriptToStore("002");

            AddScriptToHistory("001");
            AddScriptToHistory("002");

            planner.MakePlan("ABC");
        }
    }
}
