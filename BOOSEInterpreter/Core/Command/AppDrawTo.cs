using BOOSE;
using BOOSEInterpreter.Canvas;

namespace BOOSEInterpreter.Core.Command
{
    /// <summary>
    /// Command that draws a line from the current pen position to the specified coordinates.
    /// Inherits from <see cref="CommandTwoParameters"/>, which provides a parsed <see cref="ParameterList"/>.
    /// </summary>
    public class AppDrawTo : CommandTwoParameters
    {
        /// <summary>
        /// The canvas used to render the line.
        /// </summary>
        private readonly PanelCanvas canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDrawTo"/> class.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> the command will operate on.</param>
        public AppDrawTo(PanelCanvas c) => canvas = c;

        /// <summary>
        /// Executes the draw operation using the first two parameters from <see cref="ParameterList"/>.
        /// Parameters are expected to be numeric; they are converted to integers and used as X and Y.
        /// </summary>
        public override void Execute()
        {
            int x = (int)(double)ParameterList[0];
            int y = (int)(double)ParameterList[1];

            canvas.DrawTo(x, y);
        }
    }
}
