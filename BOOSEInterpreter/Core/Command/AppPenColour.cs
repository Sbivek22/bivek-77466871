using BOOSE;
using System;
using BOOSEInterpreter.Canvas;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Command
{
#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
    /// <summary>
    /// Command that sets the pen colour used for subsequent drawing operations. Expects
    /// three numeric parameters representing the red, green and blue channels (0-255).
    /// </summary>
    /// <remarks>
    /// <see cref="AppPenColour"/> inherits from <see cref="CommandThreeParameters"/>, which
    /// provides the <see cref="ParameterList"/> textual data. Parameters are evaluated using
    /// <see cref="BOOSEInterpreter.Core.Runtime.BooseEval.Int"/> and applied to the canvas via
    /// <see cref="PanelCanvas.SetColour(int,int,int)"/>. The command will throw a
    /// <see cref="CommandException"/> when not initialised with a program or when parameters
    /// are missing or incorrectly formed.
    /// </remarks>
    public class AppPenColour : CommandThreeParameters
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
    {
        /// <summary>
        /// The target canvas whose pen colour will be modified.
        /// </summary>
        private readonly PanelCanvas _canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppPenColour"/> command.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> to update. Must not be <c>null</c>.</param>
        public AppPenColour(PanelCanvas c) => _canvas = c;

        /// <summary>
        /// Executes the command by parsing three color components (R, G, B) from the
        /// <see cref="ParameterList"/>, evaluating them to integers and calling
        /// <see cref="PanelCanvas.SetColour(int,int,int)"/>.
        /// </summary>
        /// <exception cref="CommandException">Thrown when the command has no program, when the
        /// parameter list is empty or not exactly three values.</exception>
        public override void Execute()
        {
            if (Program == null)
                throw new CommandException("PenColour has not been initialised with a StoredProgram.");

            string raw = (ParameterList ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(raw))
                throw new CommandException("PenColour requires 3 parameters.");

            string[] parts = raw.Contains(",")
                ? raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                : raw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3)
                throw new CommandException("PenColour requires exactly 3 parameters.");

            int r = BooseEval.Int(Program, parts[0]);
            int g = BooseEval.Int(Program, parts[1]);
            int b = BooseEval.Int(Program, parts[2]);

            _canvas.SetColour(r, g, b);
        }
    }
}
