using BOOSE;
using System;
using BOOSEInterpreter.Canvas;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Command
{

#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
    /// <summary>
    /// Command that sets the pen colour for subsequent drawing operations.
    /// Inherits from <see cref="CommandThreeParameters"/>, which provides a parsed <see cref="ParameterList"/>.
    /// </summary>
    public class AppPenColour : CommandThreeParameters
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
    {
        /// <summary>
        /// The canvas whose drawing colour will be changed.
        /// </summary>
        private readonly PanelCanvas _canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppPenColour"/> class.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> the command will operate on.</param>
        public AppPenColour(PanelCanvas c) => _canvas = c;


#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
        /// <summary>
        /// Executes the command by reading three numeric parameters (R, G, B) from
        /// <see cref="ParameterList"/>, converting them to integers, and applying the
        /// resulting colour to the canvas via <see cref="PanelCanvas.SetColour"/>.
        /// </summary>
        public override void Execute()
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
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
