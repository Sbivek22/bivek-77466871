using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Replace
{
    /// <summary>
    /// Unit tests for the BOOSE <c>for</c> loop implementation.
    /// Validates loop execution using the comma-separated loop syntax.
    /// </summary>
    [TestClass]
    public class AppForTests
    {
        /// <summary>
        /// Ensures that the comma-form <c>for</c> loop (<c>for i,1,10,2</c>)
        /// iterates over odd values (1,3,5,7,9) and accumulates them correctly.
        /// Expected sum: 1 + 3 + 5 + 7 + 9 = 25.
        /// </summary>
        [TestMethod]
        public void For_CommaForm_SumsOdds()
        {
            var (p, _) = InterpreterHarness.Run(
                "int sum = 0\n" +
                "for i,1,10,2\n" +
                "sum = sum + i\n" +
                "end for\n"
            );

            Assert.AreEqual(25, InterpreterHarness.GetInt(p, "sum"));
        }
    }
}
