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
            if (this.Program == null)
                throw new CommandException("Write has not been initialised with a StoredProgram.");

            string expr = (ParameterList ?? string.Empty).Trim();


            // If writing a boolean variable, print true/false
            if (this.Program.VariableExists(expr))
            {
                var ev = this.Program.GetVariable(expr);
                if (ev is BOOSEInterpreter.Core.Replace.AppBoolean ab)
                {
                    canvas.WriteText(ab.BoolValue ? "true" : "false");
                    return;
                }
            }


            // Use your evaluator so AppReal works correctly (15.5*10.0 => 155.0)
            string output = BOOSEInterpreter.Core.Runtime.BooseEval.String(this.Program, expr);

            canvas.WriteText(output);
        }

    }
}
