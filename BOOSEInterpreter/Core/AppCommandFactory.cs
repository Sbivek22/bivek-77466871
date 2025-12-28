using BOOSE;
using BOOSEInterpreter.Canvas;
using BOOSEInterpreter.Core.Command;

namespace BOOSEInterpreter.Core
{
    /// <summary>
    /// Factory that creates application-specific command implementations for the BOOSE interpreter.
    /// Extends the base <see cref="CommandFactory"/> to return commands that operate on the
    /// application's <see cref="PanelCanvas"/>.
    /// </summary>
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
        /// Creates an <see cref="ICommand"/> instance for the given command <paramref name="name"/>.
        /// Returns application-specific command implementations where available; otherwise falls back
        /// to the base factory behavior.
        /// </summary>
        /// <param name="name">The command name to create (case-insensitive).</param>
        /// <returns>An <see cref="ICommand"/> instance for the requested command.</returns>
        public override ICommand MakeCommand(string name)
        {
            switch (name.ToLower())
            {
                case "moveto": return new AppMoveTo(canvas);
                case "drawto": return new AppDrawTo(canvas);
                case "circle": return new AppCircle(canvas);  // unrestricted
                case "rect": return new AppRect(canvas);
                case "pencolour": return new AppPenColour(canvas);
                case "write": return new AppWrite(canvas);
                case "tri": return new AppTriCommand(canvas);
            }

            return base.MakeCommand(name);
        }
    }
}
