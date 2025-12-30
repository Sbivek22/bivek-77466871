using BOOSE;
using BOOSEInterpreter.Canvas;

namespace BOOSEInterpreter.Core.Command
{
    /// <summary>
    /// Application-specific implementation of the <see cref="Write"/> command that renders
    /// evaluated text to a <see cref="PanelCanvas"/> instance.
    /// </summary>
    /// <remarks>
    /// <see cref="AppWrite"/> evaluates the textual expression contained in
    /// <see cref="ParameterList"/> in the context of the associated <see cref="Program"/>
    /// and writes the resulting string to the canvas using <see cref="PanelCanvas.WriteText(string)"/>.
    /// If the expression refers to a boolean variable stored in the program, the command
    /// writes "true" or "false" instead of the variable object's textual form.
    /// </remarks>
    public class AppWrite : Write
    {
        /// <summary>
        /// The canvas used to render text for this command.
        /// </summary>
        private readonly PanelCanvas canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppWrite"/> class with the provided canvas.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> used to draw the output text. Must not be <c>null</c>.</param>
        public AppWrite(PanelCanvas c)
        {
            canvas = c;
        }

        /// <summary>
        /// Executes the write command. The method validates that a <see cref="Program"/> is
        /// associated with the command, reads the expression from <see cref="ParameterList"/>
        /// and evaluates it to a string using <see cref="BOOSEInterpreter.Core.Runtime.BooseEval.String"/>.
        /// If the expression names a boolean variable in the program, the boolean value is
        /// printed as "true"/"false". The resulting text is written to the canvas via
        /// <see cref="PanelCanvas.WriteText(string)"/>.
        /// </summary>
        /// <exception cref="CommandException">Thrown when the command has not been initialised
        /// with a <see cref="Program"/>.</exception>
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
