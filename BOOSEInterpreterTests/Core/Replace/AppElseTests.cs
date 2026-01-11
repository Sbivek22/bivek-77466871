using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Replace
{
    /// <summary>
    /// Unit tests for the BOOSE <c>else</c> branch within an <c>if</c> statement.
    /// Verifies that the false branch executes when the condition evaluates to false.
    /// </summary>
    [TestClass]
    public class AppElseTests
    {
        /// <summary>
        /// Ensures that when the <c>if</c> condition is false, the <c>else</c> branch runs
        /// and updates program state as expected.
        /// </summary>
        [TestMethod]
        public void Else_FalseBranchRuns()
        {
            var (p, _) = InterpreterHarness.Run(
                "int x = 50\n" +
                "int r = 0\n" +
                "if x < 10\n" +
                "r = 1\n" +
                "else\n" +
                "r = 2\n" +
                "end if\n"
            );

            Assert.AreEqual(2, InterpreterHarness.GetInt(p, "r"));
        }
    }
}
