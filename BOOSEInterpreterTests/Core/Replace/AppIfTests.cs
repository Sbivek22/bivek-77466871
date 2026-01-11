using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Replace
{
    /// <summary>
    /// Unit tests for the BOOSE <c>if</c> control structure.
    /// Verifies that conditional execution works when the condition is true.
    /// </summary>
    [TestClass]
    public class AppIfTests
    {
        /// <summary>
        /// Ensures that when an <c>if</c> condition evaluates to true,
        /// the body of the <c>if</c> statement executes and updates program state.
        /// </summary>
        [TestMethod]
        public void If_TrueBranchRuns()
        {
            var (p, _) = InterpreterHarness.Run(
                "int x = 5\n" +
                "int r = 0\n" +
                "if x < 10\n" +
                "r = 1\n" +
                "end if\n"
            );

            Assert.AreEqual(1, InterpreterHarness.GetInt(p, "r"));
        }
    }
}
