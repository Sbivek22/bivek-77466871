using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Command
{
    [TestClass]
    public class AppDrawToTests
    {
        [TestMethod]
        public void DrawTo_RecordsLine()
        {
            var (_, c) = InterpreterHarness.Run(
                "moveto 0,0\n" +
                "drawto 10,20\n"
            );

            Assert.AreEqual((10, 20), c.DrawToCalls[0]);
        }
    }
}
