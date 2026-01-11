using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Command
{
#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
    /// <summary>
    /// Command that draws an unfilled rectangle on the canvas. The command expects two
    /// parameters representing the rectangle's width and height in pixels.
    /// </summary>
    /// <remarks>
    /// <see cref="AppRect"/> inherits from <see cref="CommandTwoParameters"/>, which provides
    /// the <see cref="ParameterList"/> and <see cref="Program"/> context. Parameters are
    /// evaluated using <see cref="BOOSEInterpreter.Core.Runtime.BooseEval.Int"/>. A
    /// <see cref="CommandException"/> is raised if the command is not initialised or if the
    /// parameters are missing or invalid.
    /// </remarks>
    public class AppRect : CommandTwoParameters
    {
        private readonly ICanvas _canvas;
        /// <summary>
        /// Initializes a new instance of the <see cref="AppRect"/> command.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> to draw onto. Must not be <c>null</c>.</param>
        public AppRect(ICanvas c) => _canvas = c;
        /// <summary>
        /// Executes the command by parsing and evaluating the width and height parameters and
        /// invoking <see cref="PanelCanvas.Rect(int,int,bool)"/> with the evaluated values
        /// and <c>false</c> to indicate an outline-only rectangle.
        /// </summary>
        /// <exception cref="CommandException">Thrown when the command lacks a program or when
        /// parameters are empty or not exactly two values.</exception>
        public override void Execute()
        {
            if (Program == null)
                throw new CommandException("Rect has not been initialised with a StoredProgram.");

            string raw = (ParameterList ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(raw))
                throw new CommandException("Rect requires 2 parameters.");

            string[] parts = raw.Contains(",")
                ? raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                : raw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new CommandException("Rect requires exactly 2 parameters.");

            int width = BooseEval.Int(Program, parts[0]);
            int height = BooseEval.Int(Program, parts[1]);

            _canvas.Rect(width, height, false);
        }
    }
}
