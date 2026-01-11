using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Replace
{
    /// <summary>
    /// Unit tests for the BOOSE <c>end</c> command.
    /// These tests validate that block-structured commands (e.g. <c>if</c>, <c>while</c>, <c>for</c>)
    /// are properly closed by their corresponding <c>end</c> statements.
    /// </summary>
    [TestClass]
    public class AppEndTests
    {
        /// <summary>
        /// Verifies that an <c>end if</c> statement correctly terminates an <c>if</c> block,
        /// allowing the program to continue parsing and executing subsequent statements.
        /// The body of this test will be completed once the desired end-block behaviour
        /// assertions are finalised.
        /// </summary>
        [TestMethod]
        public void End_ClosesIfBlock_Correctly()
        {
            var (p, _) = InterpreterHarness.Run(
                ""
            );
        }
    }
}
