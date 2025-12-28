using BOOSE;
using BOOSEInterpreter.Canvas;

namespace BOOSEInterpreter.Core.Command
{
    /// <summary>
    /// Command that moves the current drawing position to the specified coordinates.
    /// Inherits from <see cref="CommandTwoParameters"/>, which provides a parsed <see cref="ParameterList"/>.
    /// </summary>
    public class AppMoveTo : CommandTwoParameters
    {
        /// <summary>
        /// The canvas used to update the current pen position.
        /// </summary>
        private readonly PanelCanvas canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppMoveTo"/> command.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> the command will operate on.</param>
        public AppMoveTo(PanelCanvas c) => canvas = c;

        /// <summary>
        /// Executes the move operation using the first two parameters from <see cref="ParameterList"/>.
        /// Parameters are expected to be numeric; they are converted to integers and used as X and Y.
        /// </summary>
        public override void Execute()
        {
            int x = (int)(double)ParameterList[0];
            int y = (int)(double)ParameterList[1];
            canvas.MoveTo(x, y);
        }
    }
}
