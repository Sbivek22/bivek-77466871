using System;
using BOOSE;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Replacement implementation for BOOSE <c>else</c> which pairs with an earlier
    /// <c>if</c> compound command.
    /// </summary>
    /// <remarks>
    /// During compilation the corresponding <c>if</c> command is popped from the program
    /// stack and its end-line is set so that control flow can locate the else branch. At
    /// runtime reaching an <c>else</c> means the <c>if</c> branch executed, so the runtime
    /// should skip the else body and continue after the matching <c>end</c>.
    /// </remarks>
    public class AppElse : CompoundCommand
    {
        /// <summary>
        /// Performs compile-time pairing with the matching <c>if</c> command and registers
        /// this <c>else</c> on the program stack so that the matching <c>end</c> pairs
        /// correctly.
        /// </summary>
        /// <exception cref="CommandException">Thrown when not associated with a program or when
        /// there is no matching <c>if</c> on the stack.</exception>
        public override void Compile()
        {
            if (this.Program == null)
                throw new CommandException("Else has not been initialised with a StoredProgram.");

            LineNumber = this.Program.Count - 1;

            var popped = this.Program.Pop();
            if (popped is not CompoundCommand cc || cc.CondType != conditionalTypes.commIF)
                throw new CommandException("Else found without a matching If.");

            // IF false should jump to the first line of the else-branch, so set end line number to this else line.
            cc.EndLineNumber = LineNumber;

            CorrespondingCommand = cc;
            this.Program.Push(this);
        }

        /// <summary>
        /// At runtime, skip the else body since execution reached else only when the if
        /// branch was executed. The program counter is set to the command after the
        /// matching <c>end</c>.
        /// </summary>
        public override void Execute()
        {
            if (this.Program == null)
                throw new CommandException("Else has not been initialised with a StoredProgram.");

            // We only execute ELSE when the IF condition was true (because IF false jumps to line after else).
            // So, skip the else body to after the matching end.
            this.Program.PC = EndLineNumber + 1;
        }
    }
}
