using System;
using System.Linq;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Unrestricted replacement for BOOSE <c>poke</c> which writes a value into an array
    /// element by evaluating a supplied expression.
    /// </summary>
    public sealed class AppPoke : BOOSE.Command
    {
        private string arrayName = string.Empty;
        private string rowExp = "0";
        private string colExp = "0";
        private string valueExp = "0";

        /// <summary>
        /// Minimal parameter validation; full parsing and validation occur in <see cref="Parse"/>.
        /// </summary>
        public override void CheckParameters(string[] parameters)
        {
            // Keep minimal; Parse() enforces correctness.
        }

        /// <summary>
        /// Parses the parameter list to determine the target array, indices and value expression.
        /// </summary>
        public override void Compile()
        {
            base.Compile();
            Parse((ParameterList ?? string.Empty).Trim());
        }

        /// <summary>
        /// Executes the poke operation by evaluating the row/column expressions and delegating
        /// to <see cref="AppArray.SetCell(int,int,string)"/> which handles type-specific storage.
        /// </summary>
        /// <exception cref="CommandException">Thrown when the command is not associated with a program
        /// or when the named array is not found.</exception>
        public override void Execute()
        {
            if (this.Program == null)
                throw new CommandException("Poke has not been initialised with a StoredProgram.");

            Evaluation ev = this.Program.GetVariable(arrayName);
            if (ev is not AppArray arr)
                throw new CommandException($"'{arrayName}' is not an array.");

            int row = BooseEval.Int(this.Program, rowExp);
            int col = BooseEval.Int(this.Program, colExp);

            // AppArray handles real/int internally using BooseEval.
            arr.SetCell(row, col, valueExp);
        }

        /// <summary>
        /// Parses the textual parameters supporting forms like
        /// <c>prices 5 = 99.99</c>, <c>prices,5,99.99</c> or <c>arr 2 3 = 99</c> and extracts
        /// the array name, row/col expressions and the value expression.
        /// </summary>
        /// <param name="paramList">Raw parameter text.</param>
        private void Parse(string paramList)
        {
            string s = (paramList ?? string.Empty).Trim();
            if (s.Length == 0)
                throw new CommandException("Poke requires parameters.");

            // Accept: "prices 5 = 99.99"  OR  "prices,5,99.99"  OR  "arr 2 3 = 99"
            var tokens = s.Replace("=", " ")
                .Replace("(", " ").Replace(")", " ")
                .Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => t.Length > 0)
                .ToArray();

            // tokens: [array, row, value]  OR  [array, row, col, value]
            if (tokens.Length < 3)
                throw new CommandException("Poke syntax: poke array row [col] = value");

            arrayName = tokens[0];
            rowExp = tokens[1];

            if (tokens.Length == 3)
            {
                colExp = "0";
                valueExp = tokens[2];
            }
            else
            {
                colExp = tokens[2];
                valueExp = tokens[^1];
            }
        }
    }
}
