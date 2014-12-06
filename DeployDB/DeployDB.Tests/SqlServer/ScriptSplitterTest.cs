using DeployDB.SqlServer;

using NUnit.Framework;

namespace DeployDB.Tests.SqlServer
{
    [TestFixture]
    public class ScriptSplitterTest
    {
        [Test]
        public void Split_NoGO()
        {
            const string script = @"ABC
DEF";
            var parts = ScriptSplitter.Split(script);
            CollectionAssert.AreEqual(new [] { script }, parts);
        }

        [Test]
        public void Split_TwoParts()
        {
            const string script = @"ABC
GO
DEF";
            var parts = ScriptSplitter.Split(script);
            CollectionAssert.AreEqual(new[] { "ABC", "DEF" }, parts);
        }

        [Test]
        public void Split_ThreeParts()
        {
            const string script = @"ABC
GO
DEF
GO
GHI";
            var parts = ScriptSplitter.Split(script);
            CollectionAssert.AreEqual(new[] { "ABC", "DEF", "GHI" }, parts);
        }

        [Test]
        public void Split_CaseInsensitiveGO()
        {
            const string script = @"ABC
gO
DEF";
            var parts = ScriptSplitter.Split(script);
            CollectionAssert.AreEqual(new[] { "ABC", "DEF" }, parts);
        }

        [Test]
        public void Split_IgnoreWhitespaceOnlyParts()
        {
            const string script = @"ABC
GO
   
GO
DEF";
            var parts = ScriptSplitter.Split(script);
            CollectionAssert.AreEqual(new[] { "ABC", "DEF" }, parts);
        }

        [Test]
        public void Split_GoMustBeAloneOnLine()
        {
            const string script = @"ABC GO DEF";
            var parts = ScriptSplitter.Split(script);
            CollectionAssert.AreEqual(new[] { "ABC GO DEF" }, parts);
        }

        [Test]
        public void Split_GoCanBeSurroundedByWhitespace()
        {
            const string script = @"ABC
  GO    
DEF";
            var parts = ScriptSplitter.Split(script);
            CollectionAssert.AreEqual(new[] { "ABC", "DEF" }, parts);
        }

        [Test]
        public void Split_MultilineParts()
        {
            const string script = @"ABC
DEF
GO
GHI
JKL";
            var parts = ScriptSplitter.Split(script);
            CollectionAssert.AreEqual(new[] { "ABC\r\nDEF", "GHI\r\nJKL" }, parts);
        }

        [Test]
        public void Split_LastPartIsEmpty()
        {
            const string script = @"ABC
GO
    ";
            var parts = ScriptSplitter.Split(script);
            CollectionAssert.AreEqual(new[] { "ABC" }, parts);
        }
    }
}
