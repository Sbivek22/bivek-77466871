using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Command
{
    [TestClass]
    public class AppRectTests
    {
        [TestMethod]
        public void Rect_RecordsCall()
        {
            var (_, c) = InterpreterHarness.Run("rect 50,60\n");
            Assert.AreEqual((50, 60, false), c.RectCalls[0]);
        }
    }
}
