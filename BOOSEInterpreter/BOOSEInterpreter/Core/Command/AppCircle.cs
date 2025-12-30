using BOOSE;
using System;
using BOOSEInterpreter.Canvas;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Command
{
    public class AppCircle : CommandOneParameter
    {
        private readonly PanelCanvas _canvas;

        public AppCircle(PanelCanvas c) => _canvas = c;

        public override void Execute()
        {
            if (Program == null)
                throw new CommandException("Circle has not been initialised with a StoredProgram.");

            // Use our evaluator (BOOSE trial evaluator can reject '*' which breaks examples
            // like: circle count * 10).
            string exp = (ParameterList ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(exp))
                throw new CommandException("Circle requires 1 parameter.");

            int radius = BooseEval.Int(Program, exp);
            _canvas.Circle(radius, false);
        }
    }
}
