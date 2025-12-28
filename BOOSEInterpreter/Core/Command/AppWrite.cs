using BOOSE;
using BOOSEInterpreter.Canvas;

namespace BOOSEInterpreter.Core.Command
{
    /// <summary>
    /// Application-specific implementation of the <see cref="Write"/> command.
    /// When executed, this command writes the evaluated expression text to the provided canvas.
    /// </summary>
    public class AppWrite : Write
    {
        /// <summary>
        /// The canvas used to render text for this command.
        /// </summary>
        private readonly PanelCanvas canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppWrite"/> class.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> used to draw the output text.</param>
        public AppWrite(PanelCanvas c)
        {
            canvas = c;
        }

        /// <summary>
        /// Executes the write command. Calls the base implementation to evaluate the expression
        /// and then writes the resulting text to the canvas.
        /// </summary>
        public override void Execute()
        {
            base.Execute();
            canvas.WriteText(evaluatedExpression.ToString());
        }
    }
}
