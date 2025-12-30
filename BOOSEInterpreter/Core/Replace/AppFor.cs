using System;
using System.Text.RegularExpressions;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;
using BOOSEInterpreter.Core.Replace;


namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Replacement implementation for BOOSE <c>for</c> loops supporting common syntax
    /// variations such as <c>for i=1 to 10 step 1</c> and comma-separated forms.
    /// </summary>
    /// <remarks>
    /// The command lazily evaluates bounds on first execution (allowing dependencies on
    /// other variables) and uses an internal <c>Advance</c> helper for iteration logic which
    /// is called by the corresponding <c>end for</c> implementation.
    /// </remarks>
    public class AppFor : CompoundCommand
    {
        private static readonly Regex ForRegex =
            new Regex(@"^(?<var>[A-Za-z_][A-Za-z0-9_]*)\s*(=\s*)?(?<from>[^\s,]+)(\s+to\s+(?<to>[^\s,]+))?(\s+step\s+(?<step>[^\s,]+))?.*$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private string lcvName = string.Empty;
        private string fromExp = "0";
        private string toExp = "0";
        private string stepExp = "1";

        private bool initialised;
        private int current;
        private int to;
        private int step;

        /// <summary>
        /// The loop control variable name.
        /// </summary>
        public string LcvName => lcvName;

        /// <summary>
        /// The current integer value of the loop control variable.
        /// </summary>
        public int Current => current;

        /// <summary>
        /// The evaluated loop upper bound.
        /// </summary>
        public int To => to;

        /// <summary>
        /// The evaluated step increment used when advancing the loop.
        /// </summary>
        public int Step => step;

        /// <summary>
        /// Parses the for loop signature and pushes this command onto the program stack
        /// so that the matching <c>end for</c> can pair with it.
        /// </summary>
        /// <exception cref="CommandException">Thrown when not associated with a program or
        /// when the signature is missing/invalid.</exception>
        public override void Compile()
        {
            if (this.Program == null)
                throw new CommandException("For has not been initialised with a StoredProgram.");

            LineNumber = this.Program.Count - 1;
            CondType = conditionalTypes.commFor;

            ParseSignature((ParameterList ?? string.Empty).Trim());
            this.Program.Push(this);
        }

        /// <summary>
        /// Evaluates loop bounds and initialises or updates the loop control variable. If the
        /// loop condition is not met, the program counter is advanced to the command after
        /// the matching <c>end for</c>.
        /// </summary>
        public override void Execute()
        {
            if (this.Program == null)
                throw new CommandException("For has not been initialised with a StoredProgram.");

            // Evaluate bounds lazily (they can depend on variables).
            int from = BooseEval.Int(this.Program, fromExp);
            to = BooseEval.Int(this.Program, toExp);
            step = BooseEval.Int(this.Program, stepExp);
            if (step == 0) step = 1;

            if (!initialised)
            {
                current = from;
                initialised = true;
            }

            // If we have already run, keep current as-is (it will be incremented by End).

            bool shouldRun = step > 0 ? current <= to : current >= to;
            if (!shouldRun)
            {
                // Jump to after end for.
                this.Program.PC = EndLineNumber + 1;
                initialised = false; // reset for next time if program is re-run
                return;
            }

            // Ensure LCV exists and update.
            if (!this.Program.VariableExists(lcvName))
                this.Program.AddVariable(new AppInt(lcvName, current) { Local = Local });
            else
                this.Program.UpdateVariable(lcvName, current);
        }

        /// <summary>
        /// Advances the loop by one step. This method is intended to be called by the
        /// <c>end for</c> implementation and updates the loop control variable accordingly.
        /// </summary>
        /// <param name="program">The program instance used to add/update the loop control variable.</param>
        /// <returns><c>true</c> if the loop should continue after the advance; otherwise <c>false</c>.</returns>
        internal bool Advance(StoredProgram program)
        {
            // Called by End.
            current += step;
            bool shouldRun = step > 0 ? current <= to : current >= to;
            if (shouldRun)
            {
                if (!program.VariableExists(lcvName))
                    program.AddVariable(new AppInt(lcvName, current) { Local = Local });
                else
                    program.UpdateVariable(lcvName, current);
            }
            else
            {
                // Reset for next run.
                initialised = false;
            }

            return shouldRun;
        }

        /// <summary>
        /// Parses the for-loop signature which may be in comma-separated form
        /// (<c>i,1,10,1</c>) or in keyword form (<c>i=1 to 10 step 1</c>).
        /// </summary>
        /// <param name="sig">The raw signature text from the command.</param>
        /// <exception cref="CommandException">Thrown when the signature is empty or invalid.</exception>
        private void ParseSignature(string sig)
        {
            if (sig.Length == 0)
                throw new CommandException("For requires parameters.");

            // Support comma form "i,1,10,1"
            var commaTokens = sig.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (commaTokens.Length >= 3)
            {
                lcvName = commaTokens[0].Trim().Split(' ')[^1];
                fromExp = commaTokens[1].Trim();
                toExp = commaTokens[2].Trim();
                stepExp = commaTokens.Length >= 4 ? commaTokens[3].Trim() : "1";
                return;
            }

            // Support keyword form "i=1 to 10 step 1".
            var m = ForRegex.Match(sig);
            if (!m.Success)
                throw new CommandException($"Invalid for syntax '{sig}'.");

            lcvName = m.Groups["var"].Value.Trim();
            fromExp = m.Groups["from"].Value.Trim();
            toExp = (m.Groups["to"].Success ? m.Groups["to"].Value : "0").Trim();
            stepExp = (m.Groups["step"].Success ? m.Groups["step"].Value : "1").Trim();
        }
    }
}
