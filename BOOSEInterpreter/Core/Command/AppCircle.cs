using BOOSE;
using BOOSEInterpreter.Canvas;

namespace BOOSEInterpreter.Core.Command
{
    /// <summary>
    /// Command that draws a circle on the application's canvas.
    /// Implements <see cref="ICommand"/> so it can be used by the interpreter.
    /// </summary>
    public class AppCircle : ICommand
    {
        /// <summary>
        /// The canvas used to render the circle.
        /// </summary>
        private readonly PanelCanvas canvas;

        /// <summary>
        /// Radius of the circle to draw. Set during <see cref="Set"/>.
        /// </summary>
        private int radius;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppCircle"/> class.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> used for drawing.</param>
        public AppCircle(PanelCanvas c)
        {
            canvas = c;
        }

        /// <summary>
        /// Configures the command with program context and the parameter list.
        /// Expected <paramref name="paramList"/> is a single integer representing the radius.
        /// </summary>
        /// <param name="program">The <see cref="StoredProgram"/> context (unused).</param>
        /// <param name="paramList">A string containing the parameter(s) for the command.</param>
        public void Set(StoredProgram program, string paramList)
        {
            radius = int.Parse(paramList);
        }

        /// <summary>
        /// Performs compilation-time checks or transformations for the command.
        /// This command imposes no compile-time restrictions.
        /// </summary>
        public void Compile()
        {
            // no restrictions
        }

        /// <summary>
        /// Validates parameter array for this command. This implementation accepts any parameters
        /// (validation is performed in <see cref="Set"/> by parsing the string).
        /// </summary>
        /// <param name="parameters">Array of parameter strings to check.</param>
        public void CheckParameters(string[] parameters)
        {
            // unrestricted → do nothing
        }

        /// <summary>
        /// Executes the command by drawing a circle with the configured radius on the canvas.
        /// </summary>
        public void Execute()
        {
            canvas.Circle(radius, false);
        }
    }
}
