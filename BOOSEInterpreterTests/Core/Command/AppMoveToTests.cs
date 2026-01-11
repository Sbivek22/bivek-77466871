using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Command
{
    [TestClass]
    public class AppMoveToTests
    {
        [TestMethod]
        public void MoveTo_RecordsPosition()
        {
            var (_, c) = InterpreterHarness.Run("moveto 10,20\n");
            Assert.AreEqual((10, 20), c.MoveToCalls[0]);
        }
    }
}
