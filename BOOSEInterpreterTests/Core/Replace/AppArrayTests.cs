using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreter.Core.Replace;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Replace
{
    /// <summary>
    /// Unit tests for the BOOSE <c>array</c> declaration command.
    /// Verifies correct creation and configuration of array variables.
    /// </summary>
    [TestClass]
    public class AppArrayTests
    {
        /// <summary>
        /// Ensures that a one-dimensional integer array is declared correctly,
        /// with the expected element type and dimensions.
        /// </summary>
        [TestMethod]
        public void Array_Declare1D_Int_HasCorrectSize()
        {
            var (p, _) = InterpreterHarness.Run("array int nums 10\n");
            var v = p.GetVariable("nums") as AppArray;

            Assert.IsNotNull(v);
            Assert.AreEqual("int", v!.ElementType);
            Assert.AreEqual(10, v.Rows);
            Assert.AreEqual(1, v.Cols);
        }
    }
}
