using BOOSE;
using System;
using BOOSEInterpreter.Canvas;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Command
{
    public class AppRect : CommandTwoParameters
    {
    private readonly PanelCanvas _canvas;

    public AppRect(PanelCanvas c) => _canvas = c;

        public override void Execute()
        {
            if (Program == null)
                throw new CommandException("Rect has not been initialised with a StoredProgram.");

            string raw = (ParameterList ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(raw))
                throw new CommandException("Rect requires 2 parameters.");

            // Support both comma and space forms.
            string[] parts = raw.Contains(",")
                ? raw.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                : raw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
                throw new CommandException("Rect requires exactly 2 parameters.");

            int width = BooseEval.Int(Program, parts[0]);
            int height = BooseEval.Int(Program, parts[1]);

            _canvas.Rect(width, height, false);
        }
    }
}
