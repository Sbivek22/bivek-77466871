using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreter.Core;
using BOOSEInterpreterTests.TestHelpers;

namespace BOOSEInterpreterTests.Core
{
    /// <summary>
    /// Unit tests for <see cref="AppCommandFactory"/>.
    /// Verifies that known BOOSE keywords are mapped to non-null command instances.
    /// </summary>
    [TestClass]
    public class AppCommandFactoryTests
    {
        /// <summary>
        /// Ensures that the factory returns a valid command object for each supported keyword.
        /// This test confirms that the command mapping is not missing any expected facilities.
        /// </summary>
        [TestMethod]
        public void MakeCommand_KnownKeywords_NotNull()
        {
            var c = new RecordingCanvas();
            var f = new AppCommandFactory(c);

            string[] keys =
            {
                "int", "real", "boolean",
                "array", "peek", "poke",
                "if", "else", "while", "for", "end",
                "method", "call",
                "circle", "write"
            };

            foreach (var k in keys)
                Assert.IsNotNull(f.MakeCommand(k), $"Null command for keyword '{k}'");
        }
    }
}
