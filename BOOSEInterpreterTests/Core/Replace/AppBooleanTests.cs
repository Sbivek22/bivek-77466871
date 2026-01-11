using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Replace
{
    /// <summary>
    /// Unit tests for boolean variable declaration and evaluation.
    /// Verifies that boolean expressions (comparisons) are evaluated correctly.
    /// </summary>
    [TestClass]
    public class AppBooleanTests
    {
        /// <summary>
        /// Ensures that a boolean variable can be initialised using a comparison expression
        /// and that the evaluated result is stored correctly.
        /// </summary>
        [TestMethod]
        public void Boolean_ComparisonInitialiser_Works()
        {
            var (p, _) = InterpreterHarness.Run(
                "int x = 5\n" +
                "boolean ok = x < 10\n"
            );

            Assert.IsTrue(InterpreterHarness.GetBool(p, "ok"));
        }
    }
}
