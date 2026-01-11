using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Replace
{
    /// <summary>
    /// Unit tests for the BOOSE <c>while</c> loop implementation.
    /// Verifies that the loop executes repeatedly while the condition is true.
    /// </summary>
    [TestClass]
    public class AppWhileTests
    {
        /// <summary>
        /// Ensures that a <c>while</c> loop iterates the correct number of times
        /// and updates variables as expected during each iteration.
        /// </summary>
        [TestMethod]
        public void While_Loops()
        {
            var (p, _) = InterpreterHarness.Run(
                "int x = 3\n" +
                "int count = 0\n" +
                "while x > 0\n" +
                "count = count + 1\n" +
                "x = x - 1\n" +
                "end while\n"
            );

            Assert.AreEqual(3, InterpreterHarness.GetInt(p, "count"));
        }
    }
}
