using BOOSE;
using BOOSEInterpreter.Canvas;

namespace BOOSEInterpreter.Core.Command
{
    /// <summary>
    /// Command that sets the pen colour for subsequent drawing operations.
    /// Inherits from <see cref="CommandThreeParameters"/>, which provides a parsed <see cref="ParameterList"/>.
    /// </summary>
    public class AppPenColour : CommandThreeParameters
    {
        /// <summary>
        /// The canvas whose drawing colour will be changed.
        /// </summary>
        private readonly PanelCanvas canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppPenColour"/> class.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> the command will operate on.</param>
        public AppPenColour(PanelCanvas c) => canvas = c;

        /// <summary>
        /// Executes the command by reading three numeric parameters (R, G, B) from
        /// <see cref="ParameterList"/>, converting them to integers, and applying the
        /// resulting colour to the canvas via <see cref="PanelCanvas.SetColour"/>.
        /// </summary>
        public override void Execute()
        {
            int r = (int)(double)ParameterList[0];
            int g = (int)(double)ParameterList[1];
            int b = (int)(double)ParameterList[2];

            canvas.SetColour(r, g, b);
        }
    }
}
