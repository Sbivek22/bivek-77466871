using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Command
{
    [TestClass]
    public class AppWriteTests
    {
        [TestMethod]
        public void Write_PrintsValue()
        {
            var (_, c) = InterpreterHarness.Run(
                "int x = 10\n" +
                "write x\n"
            );

            Assert.IsTrue(c.WriteCalls.Count == 1);
            Assert.IsTrue(c.WriteCalls[0].Contains("10"));
        }
    }
}
