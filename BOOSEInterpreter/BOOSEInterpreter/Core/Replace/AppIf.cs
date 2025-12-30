using System;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Unrestricted replacement for BOOSE If.
    /// </summary>
    public class AppIf : CompoundCommand
    {
        public override void Compile()
        {
            if (this.Program == null)
                throw new CommandException("If has not been initialised with a StoredProgram.");

            LineNumber = this.Program.Count - 1;
            CondType = conditionalTypes.commIF;

            // Condition expression is held in the ParameterList string.
            Expression = (ParameterList ?? string.Empty).Trim();
            this.Program.Push(this);
        }

        public override void Execute()
        {
            if (this.Program == null)
                throw new CommandException("If has not been initialised with a StoredProgram.");

            bool condition = BooseEval.Bool(this.Program, Expression ?? string.Empty);
            if (!condition)
            {
                // EndLineNumber points to either:
                // - Else line (if an else exists)
                // - End line (if there is no else)
                // In both cases we want to jump to the first executable line of the false branch.
                this.Program.PC = EndLineNumber + 1;
            }
        }
    }
}
