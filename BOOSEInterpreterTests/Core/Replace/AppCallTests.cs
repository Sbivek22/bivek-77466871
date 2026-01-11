using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;
using System.Drawing;

namespace BOOSEInterpreterTests.Core.Replace
{
    /// <summary>
    /// Unit tests for the BOOSE <c>call</c> command.
    /// These tests verify that declared methods can be invoked correctly
    /// and that multiple method calls can coexist in the same program.
    /// </summary>
    [TestClass]
    public class AppCallTests
    {
        /// <summary>
        /// Verifies that the interpreter can successfully call two different methods
        /// within the same program execution.
        /// The body of this test will be completed once the method-call behaviour
        /// and expected outcomes are finalised.
        /// </summary>
        [TestMethod]
        public void Call_WorksForTwoMethods()
        {
            var (p, _) = InterpreterHarness.Run(
                ""
            );
        }
    }
}
