using System;
using System.Text.RegularExpressions;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;
using BOOSEInterpreter.Core.Replace;


namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Unrestricted replacement for BOOSE For.
    /// 
    /// Supports common forms:
    /// <code>
    /// for i=1 to 10 step 1
    /// for i=1,10,1
    /// for i,1,10,1
    /// </code>
    /// </summary>
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

        public string LcvName => lcvName;
        public int Current => current;
        public int To => to;
        public int Step => step;

        public override void Compile()
        {
            if (this.Program == null)
                throw new CommandException("For has not been initialised with a StoredProgram.");

            LineNumber = this.Program.Count - 1;
            CondType = conditionalTypes.commFor;

            ParseSignature((ParameterList ?? string.Empty).Trim());
            this.Program.Push(this);
        }

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
