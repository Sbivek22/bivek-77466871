using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Unrestricted real-number variable command.
    ///
    /// Note: BOOSE's built-in <see cref="BOOSE.Real"/> is sealed. This class aims to be
    /// API-compatible enough for the coursework, while remaining simple.
    /// </summary>
    public class AppReal : Evaluation
    {
        private bool initialised;
        private readonly List<string> extraDeclared = new();

        /// <summary>
        /// Real value (hides <see cref="Evaluation.Value"/> which is an int).
        /// </summary>
        public new double Value { get; set; }

        public AppReal() { }

        public AppReal(string name, double initialValue)
        {
            VarName = name;
            Value = initialValue;
            Expression = initialValue.ToString(CultureInfo.InvariantCulture);
            initialised = true;
            // Keep the base int Value roughly in sync (helps if something reads Evaluation.Value).
            base.Value = (int)Math.Round(initialValue);
        }

        public AppReal(string name, string initialValue)
            : this(name, ParseToDouble(initialValue))
        {
        }

        public override void Compile()
        {
            if (this.Program == null)
                throw new CommandException("Real has not been initialised with a StoredProgram.");

            string text = (ParameterList ?? string.Empty).Trim();
            if (text.Length == 0)
                throw new CommandException("Real requires a variable name.");

            ParseAndDeclare(text);

            if (!this.Program.VariableExists(VarName))
                this.Program.AddVariable(this);
        }

        public override void Execute()
        {
            if (this.Program == null)
                throw new CommandException("Real has not been initialised with a StoredProgram.");

            // Ensure extra declared variables exist.
            foreach (string name in extraDeclared)
            {
                if (!this.Program.VariableExists(name))
                    this.Program.AddVariable(new AppReal(name, 0.0) { Local = Local });
            }

            double d = BooseEval.Double(this.Program, Expression ?? "0");
            Value = d;
            base.Value = (int)Math.Round(d);

            if (!this.Program.VariableExists(VarName))
            {
                this.Program.AddVariable(this);
            }
            else
            {
                // Update the stored instance (do NOT call UpdateVariable(name, double))
                var existing = this.Program.GetVariable(VarName);
                if (existing is AppReal ar)
                {
                    ar.Value = d;
                    //ar.base.Value = (int)Math.Round(d); // if this line gives an access error, remove it
                }
                else
                {
                    existing.Value = (int)Math.Round(d);
                }
            }

            // Attempt to update using BOOSE's real overload.
            //this.Program.UpdateVariable(VarName, d);

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
                Value = ParseToDouble(right);
                base.Value = (int)Math.Round(Value);
                initialised = true;
                return;
            }

            // Format 2: a,b,c
            var names = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(n => NormaliseName(n))
                .Where(n => n.Length > 0)
                .ToList();

            if (names.Count == 0)
                throw new CommandException("Real requires a variable name.");

            VarName = names[0];
            Value = 0.0;
            base.Value = 0;
            Expression = "0";
            initialised = true;

            for (int i = 1; i < names.Count; i++)
                extraDeclared.Add(names[i]);
        }

        private static double ParseToDouble(string? raw)
        {
            raw = (raw ?? string.Empty).Trim();
            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                return d;
            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
                return i;
            return 0.0;
        }

        private static string NormaliseName(string raw)
        {
            raw = (raw ?? string.Empty).Trim();
            if (raw.Length == 0) return string.Empty;

            var parts = raw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length == 0 ? string.Empty : parts[^1].Trim();
        }
    }
}
