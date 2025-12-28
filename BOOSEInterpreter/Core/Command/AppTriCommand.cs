using BOOSE;
using BOOSEInterpreter.Canvas;

namespace BOOSEInterpreter.Core.Command
{
    /// <summary>
    /// Command that draws a triangle on the application's canvas.
    /// Implements <see cref="ICommand"/> so it can be used by the interpreter.
    /// </summary>
    public class AppTriCommand : ICommand
    {
        /// <summary>
        /// The canvas used to render the triangle.
        /// </summary>
        private readonly PanelCanvas canvas;

        /// <summary>
        /// Width of the triangle to draw. Set during <see cref="Set"/>.
        /// </summary>
        private int w, h;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppTriCommand"/> class.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> used for drawing.</param>
        public AppTriCommand(PanelCanvas c)
        {
            canvas = c;
        }

        /// <summary>
        /// Configures the command with program context and the parameter list.
        /// Expected <paramref name="paramList"/> contains two comma-separated integers: width and height.
        /// </summary>
        /// <param name="program">The <see cref="StoredProgram"/> context (unused).</param>
        /// <param name="paramList">A comma-separated string with width and height.</param>
        public void Set(StoredProgram program, string paramList)
        {
            var p = paramList.Split(',');

            w = int.Parse(p[0]);
            h = int.Parse(p[1]);
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
        /// Executes the command by drawing a triangle with the configured width and height on the canvas.
        /// </summary>
        public void Execute()
        {
            canvas.Tri(w, h);
        }
    }
}
