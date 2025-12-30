using BOOSE;
using BOOSEInterpreter.Canvas;
using BOOSEInterpreter.Core.Command;
using BOOSEInterpreter.Core.Replace;

namespace BOOSEInterpreter.Core
{
    /// <summary>
    /// Factory that creates application-specific command implementations for the BOOSE interpreter.
    /// </summary>
    /// <remarks>
    /// <see cref="AppCommandFactory"/> extends the base <see cref="CommandFactory"/> and returns
    /// command implementations that integrate with the application's <see cref="PanelCanvas"/> where
    /// drawing or canvas access is required. For language constructs that have unrestricted
    /// replacements (e.g. <c>int</c>, <c>real</c>, <c>array</c>, control-flow commands), the factory
    /// returns the corresponding <c>App*</c> replacement implementations. If a command name is not
    /// recognised the factory falls back to the base implementation by calling <see cref="CommandFactory.MakeCommand"/>.
    /// </remarks>
    public class AppCommandFactory : CommandFactory
    {
        /// <summary>
        /// The canvas instance that created commands will use for rendering.
        /// </summary>
        private readonly PanelCanvas canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppCommandFactory"/> class.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> that commands should draw to.</param>
        public AppCommandFactory(PanelCanvas c)
        {
            canvas = c;
        }

        /// <summary>
        /// Create an <see cref="ICommand"/> instance for the provided command <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The command name to create (case-insensitive).</param>
        /// <returns>An <see cref="ICommand"/> instance that implements the requested command. If the
        /// command name is not handled by this factory the method delegates to <see cref="CommandFactory.MakeCommand"/>.</returns>
        /// <remarks>
        /// The factory maps a set of known command names to their application-specific implementations.
        /// Drawing-related commands receive the <see cref="PanelCanvas"/> instance supplied to the
        /// factory constructor. Control-flow and replacement commands (such as <c>int</c>, <c>real</c>,
        /// <c>array</c>, <c>if</c>, <c>for</c>, <c>while</c>, <c>end</c>, <c>method</c>, <c>call</c>) are
        /// provided as unrestricted replacements by returning the corresponding <c>App*</c> types.
        /// </remarks>
        public override ICommand MakeCommand(string name)
        {
            switch (name.ToLower())
            {
                // ===== Unrestricted replacements (Part 2 requirements) =====
                case "int": return new AppInt();
                case "real": return new AppReal();
                case "boolean": return new AppBoolean();

                case "array": return new AppArray();
                case "peek": return new AppPeek();
                case "poke": return new AppPoke();

                case "if": return new AppIf();
                case "else": return new AppElse();
                case "while": return new AppWhile();
                case "for": return new AppFor();
                case "end": return new AppEnd();

                case "method": return new AppMethod();
                case "call": return new AppCall();

                case "moveto": return new AppMoveTo(canvas);
                case "drawto": return new AppDrawTo(canvas);
                case "circle": return new AppCircle(canvas);  // unrestricted
                case "rect": return new AppRect(canvas);
                case "pen":
                case "pencolour": return new AppPenColour(canvas);
                case "write": return new AppWrite(canvas);
                case "tri": return new AppTriCommand(canvas);
            }

            return base.MakeCommand(name);
        }
    }
}
