using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Command
{
    [TestClass]

    public class AppCircleTests
    {
        [TestMethod]
        public void Circle_UsesExpression()
        {
            var (_, c) = InterpreterHarness.Run(
                "int count = 3\n" +
                "circle count * 10\n"
            );

            Assert.AreEqual(30, c.CircleCalls[0]);
        }
    }
}
