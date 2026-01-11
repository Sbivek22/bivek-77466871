using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Replace
{
    /// <summary>
    /// Unit tests for integer variable declaration and assignment.
    /// Verifies correct storage and evaluation of integer expressions.
    /// </summary>
    [TestClass]
    public class AppIntTests
    {
        /// <summary>
        /// Ensures that integer variables can be declared, assigned,
        /// and used in arithmetic expressions correctly.
        /// </summary>
        [TestMethod]
        public void Int_DeclareAndAssign_Works()
        {
            var (p, _) = InterpreterHarness.Run(
                "int a = 5\n" +
                "int b\n" +
                "b = a * 2\n"
            );

            Assert.AreEqual(5, InterpreterHarness.GetInt(p, "a"));
            Assert.AreEqual(10, InterpreterHarness.GetInt(p, "b"));
        }
    }
}
