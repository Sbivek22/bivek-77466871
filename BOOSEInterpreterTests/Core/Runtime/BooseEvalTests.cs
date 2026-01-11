using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreter.Core.Runtime;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Runtime
{
    /// <summary>
    /// Unit tests for <see cref="BooseEval"/> expression evaluation helpers.
    /// Verifies numeric and boolean evaluation using variables defined in a program.
    /// </summary>
    [TestClass]
    public class BooseEvalTests
    {
        /// <summary>
        /// Confirms that <see cref="BooseEval.Int"/> can evaluate an arithmetic expression
        /// that references a previously declared integer variable.
        /// </summary>
        [TestMethod]
        public void Int_EvaluatesExpression()
        {
            var (p, _) = InterpreterHarness.Run("int x = 10\n");
            Assert.AreEqual(30, BooseEval.Int(p, "x * 3"));
        }

        /// <summary>
        /// Confirms that <see cref="BooseEval.Bool"/> can evaluate a boolean comparison
        /// that references a previously declared integer variable.
        /// </summary>
        [TestMethod]
        public void Bool_EvaluatesComparison()
        {
            var (p, _) = InterpreterHarness.Run("int x = 10\n");
            Assert.IsTrue(BooseEval.Bool(p, "x > 5"));
        }
    }
}
