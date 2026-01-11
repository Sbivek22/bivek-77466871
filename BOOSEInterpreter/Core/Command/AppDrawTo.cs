using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Command
{
    /// <summary>
    /// Command that draws a straight line from the current pen position to the specified
    /// coordinates. The command expects two parameters (X and Y) which are evaluated in the
    /// current program context and converted to integer pixel coordinates.
    /// </summary>
    /// <remarks>
    /// <see cref="AppDrawTo"/> inherits from <see cref="CommandTwoParameters"/>, which
    /// provides access to the textual <see cref="ParameterList"/> and the
    /// <see cref="Program"/> execution context. The command uses
    /// <see cref="BOOSEInterpreter.Core.Runtime.BooseEval.Int"/> to evaluate each parameter.
    /// A <see cref="CommandException"/> is thrown when the command is not initialised with a
    /// program or when parameters are missing or invalid.
    /// </remarks>
    public class AppDrawTo : CommandTwoParameters
    {
        private readonly ICanvas _canvas;
        /// <summary>
        /// The canvas that will be used to render the line.
        /// </summary>
        public AppDrawTo(ICanvas c) => _canvas = c;
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDrawTo"/> class.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> the command will draw onto. Must not be <c>null</c>.</param>

        public override void Execute()
        {
            /// <summary>
            /// Executes the command by validating and parsing the two required parameters, evaluating
            /// them to integer coordinates, and invoking <see cref="PanelCanvas.DrawTo(int,int)"/>.
            /// </summary>
            /// <exception cref="CommandException">Thrown when the command lacks a program, when the
            /// parameter list is empty, or when the wrong number of parameters is provided.</exception>

            if (Program == null)
                throw new CommandException("DrawTo has not been initialised with a StoredProgram.");

            string raw = (ParameterList ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(raw))
                throw new CommandException("DrawTo requires 2 parameters.");

            string[] parts = raw.Contains(",")
                ? raw.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)
                : raw.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new CommandException("DrawTo requires exactly 2 parameters.");

            int x = BooseEval.Int(Program, parts[0]);
            int y = BooseEval.Int(Program, parts[1]);

            _canvas.DrawTo(x, y);
        }
    }
}
