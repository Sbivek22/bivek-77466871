using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Unrestricted integer variable command.
    /// </summary>
    public class AppInt : Evaluation
    {
        private bool initialised;
        private readonly List<string> extraDeclared = new();

        public AppInt() { }

        public AppInt(string name, int initialValue)
        {
            VarName = name;
            Value = initialValue;
            Expression = initialValue.ToString(CultureInfo.InvariantCulture);
            initialised = true;
        }

        public AppInt(string name, string initialValue)
        {
            VarName = name;
            Value = ParseToInt(initialValue);
            Expression = initialValue;
            initialised = true;
        }

        public override void Compile()
        {
            if (this.Program == null)
                throw new CommandException("Int has not been initialised with a StoredProgram.");

            string text = (ParameterList ?? string.Empty).Trim();
            if (text.Length == 0)
                throw new CommandException("Int requires a variable name.");

            ParseAndDeclare(text);

            // Add declaration to the variable table immediately.
            if (!this.Program.VariableExists(VarName))
                this.Program.AddVariable(this);
        }

        public override void Execute()
        {
            if (this.Program == null)
                throw new CommandException("Int has not been initialised with a StoredProgram.");

            // Ensure any extra declared variables exist.
            foreach (string name in extraDeclared)
            {
                if (!this.Program.VariableExists(name))
                    this.Program.AddVariable(new AppInt(name, 0) { Local = Local });
            }

            int result = BooseEval.Int(this.Program, (Expression ?? "0").Trim());
            Value = result;

            if (!this.Program.VariableExists(VarName))
                this.Program.AddVariable(this);
            else
                this.Program.UpdateVariable(VarName, result);

            initialised = true;
        }

        private void ParseAndDeclare(string text)
        {
            // Format 1: var=expr
            int eq = text.IndexOf('=');
            if (eq >= 0)
            {
                string left = text.Substring(0, eq).Trim();
                string right = text.Substring(eq + 1).Trim();

                VarName = NormaliseName(left);
                Expression = right;
                // IMPORTANT: do not attempt to evaluate the expression at compile time.
                // In methods, initialisers often refer to parameters/local variables that do
                // not exist until runtime (e.g. "int mullMethod = one * two").
                // We'll evaluate Expression during Execute().
                if (int.TryParse(right, NumberStyles.Integer, CultureInfo.InvariantCulture, out int lit))
                    Value = lit;
                else
                    Value = 0;
                initialised = true;
                return;
            }

            // Format 2: a,b,c
            var names = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(n => NormaliseName(n))
                .Where(n => n.Length > 0)
                .ToList();

            if (names.Count == 0)
                throw new CommandException("Int requires a variable name.");

            VarName = names[0];
            Value = 0;
            Expression = "0";
            initialised = true;

            for (int i = 1; i < names.Count; i++)
                extraDeclared.Add(names[i]);
        }

        private static int ParseToInt(string? raw)
        {
            raw = (raw ?? string.Empty).Trim();

            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
                return i;

            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                return (int)Math.Round(d);

            return 0;
        }

        private static string NormaliseName(string raw)
        {
            raw = (raw ?? string.Empty).Trim();
            if (raw.Length == 0) return string.Empty;

            // Allow "int x" and take the last token.
            var parts = raw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 0 ? string.Empty : parts[^1].Trim();
        }
    }
}
