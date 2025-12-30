using System;
using BOOSE;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Replace replacement for BOOSE Else.
    /// 
    /// Notes:
    /// - Compile-time: pops the corresponding IF off the stack, sets its end line to this else,
    ///   then pushes itself so that the End command pairs with Else.
    /// - Runtime: if we reach 'else' (meaning the IF branch was executed), we skip to after end.
    /// </summary>
    public class AppElse : CompoundCommand
    {
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
