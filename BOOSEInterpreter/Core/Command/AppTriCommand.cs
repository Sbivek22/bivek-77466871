using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Command
{
#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
    /// <summary>
    /// Command that draws an isosceles triangle on the canvas. The command expects two
    /// parameters: the base width and the triangle height, both evaluated as integers.
    /// </summary>
    /// <remarks>
    /// <see cref="AppTriCommand"/> derives from <see cref="CommandTwoParameters"/>, which
    /// provides access to the textual <see cref="ParameterList"/> and the current
    /// <see cref="Program"/>. Parameter evaluation uses
    /// <see cref="BOOSEInterpreter.Core.Runtime.BooseEval.Int"/>. Errors in initialization
    /// or parameter parsing result in a <see cref="CommandException"/>.
    /// </remarks>
    public class AppTriCommand : CommandTwoParameters
    {
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved

        private readonly ICanvas _canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppTriCommand"/> class.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> to draw onto. Must not be <c>null</c>.</param>
        public AppTriCommand(ICanvas c) => _canvas = c;
        /// <summary>
        /// Executes the command by validating, parsing and evaluating the width and height
        /// parameters and invoking <see cref="PanelCanvas.Tri(int,int)"/> to draw the
        /// triangle.
        /// </summary>
        /// <exception cref="CommandException">Thrown when the command has no program or when
        /// parameters are missing, empty, or not exactly two values.</exception>
        public override void Execute()
        {
            if (Program == null)
                throw new CommandException("Tri has not been initialised with a StoredProgram.");

            string raw = (ParameterList ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(raw))
                throw new CommandException("Tri requires 2 parameters.");

            string[] parts = raw.Contains(",")
                ? raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                : raw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new CommandException("Tri requires exactly 2 parameters.");

            int width = BooseEval.Int(Program, parts[0]);
            int height = BooseEval.Int(Program, parts[1]);

            _canvas.Tri(width, height);
        }
    }
}
