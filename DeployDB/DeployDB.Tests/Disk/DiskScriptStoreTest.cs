using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DeployDB.Disk;

using NUnit.Framework;

namespace DeployDB.Tests.Disk
{
    [TestFixture]
    public class DiskScriptStoreTest
    {
        private const string scriptsDir = "scripts";
        private DiskScriptStore store;

        [SetUp]
        public void Setup()
        {
            if (Directory.Exists(scriptsDir))
                Directory.Delete(scriptsDir, true);
            Directory.CreateDirectory(scriptsDir);
            store = new DiskScriptStore(scriptsDir);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(scriptsDir))
                Directory.Delete(scriptsDir, true);
        }

        private static void CreateScript(string name, string deploy)
        {
            WriteFileInScriptDir(name + ".deploy.sql", deploy);            
        }

        private static void WriteFileInScriptDir(string filename, string content)
        {
            string filepath = Path.Combine(scriptsDir, filename);
            File.WriteAllText(filepath, content);
        }

        private static void CreateScript(string name, string deploy, string rollback)
        {
            WriteFileInScriptDir(name + ".deploy.sql", deploy);
            WriteFileInScriptDir(name + ".rollback.sql", rollback);
        }

        private static void CreateJustRollbackScript(string name, string rollback)
        {
            WriteFileInScriptDir(name + ".rollback.sql", rollback);
        }

        [Test]
        public void GetAll_NoDirectory()
        {
            store = new DiskScriptStore("IDontExist");
            var result = store.Scripts;
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetAll_EmptyDirectory()
        {
            var result = store.Scripts;
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetAll_FullScript()
        {
            CreateScript("001", "ABC", "DEF");
            Script result = store.Scripts.Single();

            Assert.AreEqual("001", result.Name, "Name");
            Assert.AreEqual("ABC", result.Deploy, "Deploy");
            Assert.AreEqual("DEF", result.Rollback, "Rollback");
        }

        [Test]
        public void GetAll_JustDeploy()
        {
            CreateScript("001", "ABC");
            Script result = store.Scripts.Single();

            Assert.AreEqual("001", result.Name, "Name");
            Assert.AreEqual("ABC", result.Deploy, "Deploy");
            Assert.IsNull(result.Rollback, "Rollback");
        }

        [Test, ExpectedException]
        public void GetAll_JustRollback()
        {
            // Decided I liked the exception because it identifies a definitely broken set of scripts, and so they should probably not be run.
            CreateJustRollbackScript("001", "DEF");
            var scripts = store.Scripts;
            Console.WriteLine("Must enumerate the scripts to execute code. - " + scripts.Count());
        }

        [Test]
        public void GetAll_InvalidExtensionsIgnored()
        {
            // No exceptions for these guys - you can include a readme.txt or whatever without problems.
            //
            // *.sql I'm not so sure about - potentially a spelling error in the file names, but also maybe a deliberate renaming to 
            // disable a script. Hard to think what a valid reason for putting other *.sql files in the scripts dir might be though.
            //
            // *.deploy.* and *.rollback.* are also hard - could be a spelling error when saving a file (eg 001.deploy.txt), or
            // could be a deliberate "commenting out" of a script.
            //
            // Current decision is to ignore, but that could change in future.
            WriteFileInScriptDir("001.whatever.you.like", "ABC");
            WriteFileInScriptDir("002.txt", "ABC");
            WriteFileInScriptDir("003.deploy.txt", "ABC");
            WriteFileInScriptDir("003.rollback.txt", "ABC");
            WriteFileInScriptDir("004.sql", "ABC");
            WriteFileInScriptDir("005.ignore.sql", "ABC");

            var result = store.Scripts;
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void GetAll_MultipleScripts()
        {
            CreateScript("001", "ABC");
            CreateScript("002", "DEF", "GHI");
            CreateScript("003", "JKL", "MNO");

            var scripts = store.Scripts;

            Script one = scripts.Single(x => x.Name == "001");
            Assert.AreEqual("ABC", one.Deploy, "one.Deploy");
            Assert.IsNull(one.Rollback, "one.Rollback");

            Script two = scripts.Single(x => x.Name == "002");
            Assert.AreEqual("DEF", two.Deploy, "two.Deploy");
            Assert.AreEqual("GHI", two.Rollback, "two.Rollback");

            Script three = scripts.Single(x => x.Name == "003");
            Assert.AreEqual("JKL", three.Deploy, "three.Deploy");
            Assert.AreEqual("MNO", three.Rollback, "three.Rollback");
        }

        [Test]
        public void GetAll_SubdirectoriesAreIgnored()
        {
            string subDir = Path.Combine(scriptsDir, "hello");
            Directory.CreateDirectory(subDir);

            string filePath = Path.Combine(subDir, "001.deploy.sql");
            File.WriteAllText(filePath, "ABC");

            var result = store.Scripts;
            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public void Indexer_GetScriptsByName()
        {
            CreateScript("001", "ABC");
            CreateScript("002", "DEF", "GHI");
            CreateScript("003", "JKL", "MNO");

            Script one = store["001"];
            Assert.AreEqual("ABC", one.Deploy, "one.Deploy");
            Assert.IsNull(one.Rollback, "one.Rollback");

            Script two = store["002"];
            Assert.AreEqual("DEF", two.Deploy, "two.Deploy");
            Assert.AreEqual("GHI", two.Rollback, "two.Rollback");

            Script three = store["003"];
            Assert.AreEqual("JKL", three.Deploy, "three.Deploy");
            Assert.AreEqual("MNO", three.Rollback, "three.Rollback");
        }

        [Test, ExpectedException(typeof(KeyNotFoundException))]
        public void Indexer_ScriptNotExist()
        {
            Script one = store["001"];
            Assert.Fail("Found script: " + one.Name + "!");
        }
    }
}
