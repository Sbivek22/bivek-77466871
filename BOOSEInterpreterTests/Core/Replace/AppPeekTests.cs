using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Replace
{
    /// <summary>
    /// Unit tests for the BOOSE <c>peek</c> command.
    /// Verifies that values can be read correctly from array elements.
    /// </summary>
    [TestClass]
    public class AppPeekTests
    {
        /// <summary>
        /// Ensures that <c>peek</c> retrieves the correct value from a real-number array
        /// and assigns it to a real variable.
        /// </summary>
        [TestMethod]
        public void Peek_ReadsArrayCell()
        {
            var (p, _) = InterpreterHarness.Run(
                "array real prices 10\n" +
                "poke prices 5 = 99.99\n" +
                "real y\n" +
                "peek y = prices 5\n"
            );

            Assert.AreEqual(99.99, InterpreterHarness.GetReal(p, "y"), 0.000001);
        }
    }
}
