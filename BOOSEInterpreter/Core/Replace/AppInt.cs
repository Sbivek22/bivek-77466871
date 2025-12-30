using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Unrestricted integer variable command used to declare and manage integer variables
    /// within a <see cref="StoredProgram"/>.
    /// </summary>
    /// <remarks>
    /// Supports declaring multiple variables in a single statement (comma-separated) and
    /// optional initialisers. Initialiser expressions are not evaluated at compile time to
    /// allow method parameter dependencies; evaluation occurs during <see cref="Execute"/>.
    /// </remarks>
    public class AppInt : Evaluation
    {
        private bool initialised;
        private readonly List<string> extraDeclared = new();

        /// <summary>
        /// Default constructor for an uninitialised integer variable declaration.
        /// </summary>
        public AppInt() { }

        /// <summary>
        /// Creates an <see cref="AppInt"/> with the specified name and initial integer value.
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="initialValue">Initial integer value.</param>
        public AppInt(string name, int initialValue)
        {
            VarName = name;
            Value = initialValue;
            Expression = initialValue.ToString(CultureInfo.InvariantCulture);
            initialised = true;
        }

        /// <summary>
        /// Creates an <see cref="AppInt"/> with the specified name and an initialiser
        /// expression provided as text (parsed with <see cref="ParseToInt"/>).
        /// </summary>
        /// <param name="name">Variable name.</param>
        /// <param name="initialValue">Initial value text.</param>
        public AppInt(string name, string initialValue)
        {
            VarName = name;
            Value = ParseToInt(initialValue);
            Expression = initialValue;
            initialised = true;
        }

        /// <summary>
        /// Parses the declaration(s) from <see cref="ParameterList"/>, records the primary
        /// variable and any additional comma-separated declarations and registers the
        /// variable with the program if necessary.
        /// </summary>
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

        /// <summary>
        /// Ensures any extra variables declared alongside the primary variable exist, evaluates
        /// the initializer expression and stores the resulting integer value in the program's
        /// variable table.
        /// </summary>
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

        /// <summary>
        /// Parses the textual declaration which may either be a single assignment
        /// (<c>name=expr</c>) or a comma-separated list of names. Populates the primary
        /// variable, optional expression and any extra declarations.
        /// </summary>
        /// <param name="text">Declaration text.</param>
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

        /// <summary>
        /// Parses a textual value into an integer. Floating values are rounded to the nearest
        /// integer and non-numeric input yields zero.
        /// </summary>
        /// <param name="raw">The input text to parse.</param>
        /// <returns>The parsed integer value.</returns>
        private static int ParseToInt(string? raw)
        {
            raw = (raw ?? string.Empty).Trim();

            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
                return i;

            if (double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                return (int)Math.Round(d);

            return 0;
        }

        /// <summary>
        /// Normalises a variable declaration token by returning the last whitespace-separated
        /// token (allowing forms such as "int x").
        /// </summary>
        /// <param name="raw">Raw token text.</param>
        /// <returns>Normalised variable name or empty string if input invalid.</returns>
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
