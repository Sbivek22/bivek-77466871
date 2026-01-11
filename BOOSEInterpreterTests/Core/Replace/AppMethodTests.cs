using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Replace
{
    /// <summary>
    /// Unit tests for BOOSE <c>method</c> declaration and invocation.
    /// These tests validate that methods can be defined and executed by the interpreter.
    /// </summary>
    [TestClass]
    public class AppMethodTests
    {
        /// <summary>
        /// Verifies that a method can be declared and subsequently called without errors.
        /// The body of this test will be completed once method invocation behaviour
        /// is finalised.
        /// </summary>
        [TestMethod]
        public void Method_DeclaresAndCanBeCalled()
        {
            var (p, _) = InterpreterHarness.Run(
                ""
            );

        }
    }
}
