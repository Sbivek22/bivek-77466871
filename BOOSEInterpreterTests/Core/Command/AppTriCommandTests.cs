using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core.Command
{
    [TestClass]
    public class AppTriCommandTests
    {
        [TestMethod]
        public void Tri_RecordsCall()
        {
            var (_, c) = InterpreterHarness.Run("tri 30,40\n");
            Assert.AreEqual((30, 40), c.TriCalls[0]);
        }
    }
}
