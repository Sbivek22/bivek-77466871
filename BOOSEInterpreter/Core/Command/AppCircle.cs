using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Command
{
#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
    /// <summary>
    /// Command that draws a circle on the application canvas. The command evaluates a single
    /// parameter expression (typically an integer radius) and instructs the provided
    /// <see cref="PanelCanvas"/> instance to draw an unfilled circle at the current pen
    /// position.
    /// </summary>
    /// <remarks>
    /// <see cref="AppCircle"/> inherits from <see cref="CommandOneParameter"/>, which provides
    /// the textual <see cref="ParameterList"/> and the <see cref="Program"/> context used for
    /// evaluation. The command uses <see cref="BOOSEInterpreter.Core.Runtime.BooseEval"/>
    /// to convert the parameter expression to an integer radius. The command will throw a
    /// <see cref="CommandException"/> if it is not initialised with a valid program or if the
    /// parameter is missing or invalid.
    /// </remarks>
    public class AppCircle : CommandOneParameter
    {
        /// <summary>
        /// The canvas instance used to render the circle.
        /// </summary>
        private readonly ICanvas _canvas;
        /// <summary>
        /// Initializes a new instance of the <see cref="AppCircle"/> class using the
        /// specified <see cref="PanelCanvas"/> for rendering.
        /// </summary>
        /// <param name="c">The canvas that will receive the circle drawing commands. Must not be <c>null</c>.</param>
        public AppCircle(ICanvas c) => _canvas = c;
        /// <summary>
        /// Executes the circle command. The method:
        /// - Validates that the command has been initialised with a <see cref="Program"/>.
        /// - Reads and trims the textual <see cref="ParameterList"/> and ensures a single
        ///   parameter is present.
        /// - Evaluates the expression to an integer radius using
        ///   <see cref="BOOSEInterpreter.Core.Runtime.BooseEval.Int"/>.
        /// - Calls <see cref="PanelCanvas.Circle(int,bool)"/> with the computed radius and
        ///   <c>false</c> for the filled flag (draw outline only).
        /// </summary>
        /// <exception cref="CommandException">Thrown when the command has no associated program or when
        /// the required parameter is missing/invalid.</exception>
        public override void Execute()
        {
            if (Program == null)
                throw new CommandException("Circle has not been initialised with a StoredProgram.");

            string exp = (ParameterList ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(exp))
                throw new CommandException("Circle requires 1 parameter.");

            int radius = BooseEval.Int(Program, exp);
            _canvas.Circle(radius, false);
        }
    }
}
