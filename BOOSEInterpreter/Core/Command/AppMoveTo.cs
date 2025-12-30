using BOOSE;
using BOOSEInterpreter.Canvas;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Command
{
#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
    /// <summary>
    /// Command that updates the current pen position without drawing. Expects two
    /// parameters (X and Y) which are evaluated and converted to integer coordinates.
    /// </summary>
    /// <remarks>
    /// <see cref="AppMoveTo"/> inherits from <see cref="CommandTwoParameters"/>, which
    /// supplies the textual <see cref="ParameterList"/>. Parameter evaluation is delegated
    /// to <see cref="BOOSEInterpreter.Core.Runtime.BooseEval.Int"/>. The command throws a
    /// <see cref="CommandException"/> if it is not initialised with a program or if
    /// parameters are missing or malformed.
    /// </remarks>
    public class AppMoveTo : CommandTwoParameters
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
    {
        /// <summary>
        /// The canvas whose current pen position will be modified.
        /// </summary>
        private readonly PanelCanvas _canvas;

        /// <summary>
        /// Initializes a new <see cref="AppMoveTo"/> instance that operates on the provided canvas.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> to update. Must not be <c>null</c>.</param>
        public AppMoveTo(PanelCanvas c) => _canvas = c;

        /// <summary>
        /// Executes the move operation: validates parameters, evaluates them to integers and
        /// calls <see cref="PanelCanvas.MoveTo(int,int)"/> to set the new pen position.
        /// </summary>
        /// <exception cref="CommandException">Thrown when the command has no program or when
        /// parameters are missing or not exactly two in number.</exception>
        public override void Execute()
        {
            if (Program == null)
                throw new CommandException("MoveTo has not been initialised with a StoredProgram.");

            string raw = (ParameterList ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(raw))
                throw new CommandException("MoveTo requires 2 parameters.");

            string[] parts = raw.Contains(",")
                ? raw.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)
                : raw.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new CommandException("MoveTo requires exactly 2 parameters.");

            int x = BooseEval.Int(Program, parts[0]);
            int y = BooseEval.Int(Program, parts[1]);

            _canvas.MoveTo(x, y);
        }


    }
}
