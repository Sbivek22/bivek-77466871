using System;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Unrestricted replacement for BOOSE While.
    /// </summary>
    public class AppWhile : CompoundCommand
    {
        public override void Compile()
        {
            if (this.Program == null)
                throw new CommandException("While has not been initialised with a StoredProgram.");

            LineNumber = this.Program.Count - 1;
            CondType = conditionalTypes.commWhile;
            Expression = (ParameterList ?? string.Empty).Trim();
            this.Program.Push(this);
        }

        public override void Execute()
        {
            if (this.Program == null)
                throw new CommandException("While has not been initialised with a StoredProgram.");

            bool condition = BooseEval.Bool(this.Program, Expression ?? string.Empty);
            if (!condition)
            {
                // Skip loop body (jump after the matching end while).
                this.Program.PC = EndLineNumber + 1;
            }
        }
    }
}
