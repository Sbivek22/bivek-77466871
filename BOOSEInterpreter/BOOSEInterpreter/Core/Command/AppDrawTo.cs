using BOOSE;
using BOOSEInterpreter.Canvas;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Command
{

#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
    /// <summary>
    /// Command that draws a line from the current pen position to the specified coordinates.
    /// Inherits from <see cref="CommandTwoParameters"/>, which provides a parsed <see cref="ParameterList"/>.
    /// </summary>
    public class AppDrawTo : CommandTwoParameters
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
    {
        /// <summary>
        /// The canvas used to render the line.
        /// </summary>
        private readonly PanelCanvas _canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDrawTo"/> class.
        /// </summary>
        /// <param name="c">The <see cref="PanelCanvas"/> the command will operate on.</param>
        public AppDrawTo(PanelCanvas c) => _canvas = c;


#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
        /// <summary>
        /// Executes the draw operation using the first two parameters from <see cref="ParameterList"/>.
        /// Parameters are expected to be numeric; they are converted to integers and used as X and Y.
        /// </summary>
        //public override void Execute()
        //{
        //    base.Execute();

        //    int x = (int)(double)ParameterList[0];
        //    int y = (int)(double)ParameterList[1];

        //    canvas.DrawTo(x, y);
        //}

        public override void Execute()
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
        {
            if (Program == null)
                throw new CommandException("DrawTo has not been initialised with a StoredProgram.");

            string raw = (ParameterList ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(raw))
                throw new CommandException("DrawTo requires 2 parameters.");

            string[] parts = raw.Contains(",")
                ? raw.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries)
                : raw.Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new CommandException("DrawTo requires exactly 2 parameters.");

            int x = BooseEval.Int(Program, parts[0]);
            int y = BooseEval.Int(Program, parts[1]);

            _canvas.DrawTo(x, y);
        }
    }

}
