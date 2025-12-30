using System;
using System.Globalization;
using System.Linq;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Unrestricted replacement for BOOSE <c>peek</c> which reads a value from an array into
    /// a program variable.
    /// </summary>
    /// <remarks>
    /// The command accepts several syntaxes such as <c>y = prices 5</c> or
    /// <c>y,prices,5</c>. Numeric conversions are handled to create or update the target
    /// variable with the appropriate numeric type (int or real) based on the array element
    /// type.
    /// </remarks>
    public sealed class AppPeek : BOOSE.Command
    {
        private string targetVar = string.Empty;
        private string arrayName = string.Empty;
        private string rowExp = "0";
        private string colExp = "0";

        /// <summary>
        /// Lightweight parameter validation; detailed parsing is performed in <see cref="Parse"/>.
        /// </summary>
        public override void CheckParameters(string[] parameters)
        {
            // Keep minimal; Parse() enforces correctness.
        }

        /// <summary>
        /// Parses and validates the parameter list for the peek command.
        /// </summary>
        public override void Compile()
        {
            base.Compile();
            Parse((ParameterList ?? string.Empty).Trim());
        }

        /// <summary>
        /// Executes the peek operation by reading a value from the named array at the
        /// evaluated indices and creating or updating the target variable in the program.
        /// </summary>
        /// <exception cref="CommandException">Thrown when not attached to a program or when the
        /// named array does not exist or is not an array.</exception>
        public override void Execute()
        {
            if (this.Program == null)
                throw new CommandException("Peek has not been initialised with a StoredProgram.");

            Evaluation ev = this.Program.GetVariable(arrayName);
            if (ev is not AppArray arr)
                throw new CommandException($"'{arrayName}' is not an array.");

            int row = BooseEval.Int(this.Program, rowExp);
            int col = BooseEval.Int(this.Program, colExp);

            string valueStr = arr.GetCell(row, col);

            // If target doesn't exist, create it as AppReal/AppInt depending on array element type.
            if (!this.Program.VariableExists(targetVar))
            {
                if (arr.ElementType == "real" &&
                    double.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double d0))
                {
                    this.Program.AddVariable(new AppReal(targetVar, d0) { Local = false });
                }
                else
                {
                    int.TryParse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i0);
                    this.Program.AddVariable(new AppInt { VarName = targetVar, Value = i0, Local = false });
                }
                return;
            }

            // Update existing variable safely (do NOT call UpdateVariable(name, double) because BOOSE expects BOOSE.Real).
            if (arr.ElementType == "real")
            {
                if (double.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                {
                    var existing = this.Program.GetVariable(targetVar);
                    if (existing is AppReal ar)
                    {
                        ar.Value = d;
                        ((Evaluation)ar).Value = (int)Math.Round(d);
                        ar.Expression = d.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        // fallback
                        existing.Value = (int)Math.Round(d);
                    }
                }
                return;
            }

            // int array
            if (int.TryParse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int i))
                this.Program.UpdateVariable(targetVar, i);
            else if (double.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double dd))
                this.Program.UpdateVariable(targetVar, (int)Math.Round(dd));
        }

        /// <summary>
        /// Parses the textual parameter list supporting forms with '=' or comma/space separated
        /// tokens and extracts the target variable, array name and index expressions.
        /// </summary>
        /// <param name="paramList">Raw parameter text from the command.</param>
        private void Parse(string paramList)
        {
            string s = (paramList ?? string.Empty).Trim();
            if (s.Length == 0)
                throw new CommandException("Peek requires parameters.");

            // Typical: "y = prices 5"   OR   "y=prices 5"   OR   "y,prices,5"
            // Normalize commas and '=' into tokens.
            var raw = s.Replace("=", " = ")
                .Replace("(", " ").Replace(")", " ")
                .Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => t.Length > 0)
                .ToArray();

            // Remove accidental leading "peek" if it ever appears in ParameterList.
            raw = raw.Where(t => !t.Equals("peek", StringComparison.OrdinalIgnoreCase)).ToArray();

            int eqIndex = System.Array.IndexOf(raw, "=");

            if (eqIndex >= 0)
            {
                if (eqIndex == 0)
                    throw new CommandException("Peek requires a target variable before '='.");

                // target is everything before '='; keep only last token as the variable name.
                targetVar = raw[eqIndex - 1];

                // After '=': array row [col]
                if (eqIndex + 2 >= raw.Length)
                    throw new CommandException("Peek syntax: target = peek array row [col]");

                arrayName = raw[eqIndex + 1];
                rowExp = raw[eqIndex + 2];
                colExp = (eqIndex + 3 < raw.Length) ? raw[eqIndex + 3] : "0";
                return;
            }

            // No '=' form: "target array row [col]"
            if (raw.Length < 3)
                throw new CommandException("Peek syntax: target = peek array row [col]");

            targetVar = raw[0];
            arrayName = raw[1];
            rowExp = raw[2];
            colExp = (raw.Length >= 4) ? raw[3] : "0";
        }
    }
}
