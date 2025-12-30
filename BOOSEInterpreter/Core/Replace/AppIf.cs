using System;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Replacement implementation for BOOSE <c>if</c> that evaluates a conditional
    /// expression and controls flow into the true or false branch.
    /// </summary>
    public class AppIf : CompoundCommand
    {
        /// <summary>
        /// Compiles the <c>if</c> command by recording its line number, storing the
        /// conditional expression from <see cref="ParameterList"/>, setting the command
        /// type and pushing the command to the program stack for later pairing with
        /// an <c>else</c> or <c>end</c>.
        /// </summary>
        /// <exception cref="CommandException">Thrown when the command is not associated with a program.</exception>
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

        /// <summary>
        /// Evaluates the condition at runtime and, if the condition is false, advances the
        /// program counter to the first command of the false branch (either after the
        /// <c>else</c> line or after the matching <c>end</c> when no else exists).
        /// </summary>
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
