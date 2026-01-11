using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Replace
{
    /// <summary>
    /// Unit tests for the BOOSE <c>poke</c> command.
    /// Validates that values are correctly written into array elements.
    /// </summary>
    [TestClass]
    public class AppPokeTests
    {
        /// <summary>
        /// Ensures that <c>poke</c> assigns the specified value into the given array index,
        /// and that the value can be retrieved using <c>peek</c>.
        /// </summary>
        [TestMethod]
        public void Poke_SetsArrayCell()
        {
            var (p, _) = InterpreterHarness.Run(
                "array int nums 10\n" +
                "poke nums 5 = 99\n" +
                "int x\n" +
                "peek x = nums 5\n"
            );

            Assert.AreEqual(99, InterpreterHarness.GetInt(p, "x"));
        }
    }
}
