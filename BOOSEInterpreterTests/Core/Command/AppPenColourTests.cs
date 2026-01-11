using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Command
{
    [TestClass]
    public class AppPenColourTests
    {
        [TestMethod]
        public void PenColour_SetsColour()
        {
            var (_, c) = InterpreterHarness.Run("pen 0,255,0\n");
            Assert.AreEqual((0, 255, 0), c.ColourCalls[0]);
        }
    }
}
