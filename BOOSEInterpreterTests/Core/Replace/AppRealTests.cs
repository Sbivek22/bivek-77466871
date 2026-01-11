using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Replace
{
    /// <summary>
    /// Unit tests for real-number variable handling and expression evaluation.
    /// </summary>
    [TestClass]
    public class AppRealTests
    {
        /// <summary>
        /// Verifies that arithmetic expressions involving <c>real</c> variables
        /// are evaluated correctly and preserve floating-point precision.
        /// </summary>
        [TestMethod]
        public void Real_Expression_Works()
        {
            var (p, _) = InterpreterHarness.Run(
                "real x = 2.5\n" +
                "real y = x * 2\n"
            );

            Assert.AreEqual(5.0, InterpreterHarness.GetReal(p, "y"), 0.000001);
        }
    }
}
