using System;
using BOOSE;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Replace replacement for BOOSE End.
    /// End is responsible for closing compound blocks:
    /// - end if
    /// - end while
    /// - end for
    /// - end method
    /// </summary>
    public class AppEnd : CompoundCommand
    {
        private string endType = string.Empty;

        public override void Compile()
        {
            if (this.Program == null)
                throw new CommandException("End has not been initialised with a StoredProgram.");

            LineNumber = this.Program.Count - 1;
            endType = (ParameterList ?? string.Empty).Trim().ToLowerInvariant();

            var popped = this.Program.Pop();
            if (popped == null)
                throw new CommandException("End found without an opening block.");

            // Validate matching.
            if (endType.Contains("if"))
            {
                if (popped is not CompoundCommand cc || (cc.CondType != conditionalTypes.commIF && popped is not AppElse))
                    throw new CommandException("'end if' does not match the current block.");
            }
            else if (endType.Contains("while"))
            {
                if (popped is not CompoundCommand cc || cc.CondType != conditionalTypes.commWhile)
                    throw new CommandException("'end while' does not match the current block.");
            }
            else if (endType.Contains("for"))
            {
                if (popped is not AppFor)
                    throw new CommandException("'end for' does not match the current block.");
            }
            else if (endType.Contains("method"))
            {
                if (popped is not AppMethod)
                    throw new CommandException("'end method' does not match the current block.");
            }

            CorrespondingCommand = popped;
            popped.EndLineNumber = LineNumber;
        }

        public override void Execute()
        {
            if (this.Program == null)
                throw new CommandException("End has not been initialised with a StoredProgram.");

            // No corresponding command means it's a no-op.
            if (CorrespondingCommand == null)
                return;

            // IF / ELSE: do nothing.
            if (CorrespondingCommand is CompoundCommand cc && cc.CondType == conditionalTypes.commIF)
                return;
            if (CorrespondingCommand is AppElse)
                return;

            // WHILE: jump back to the while line.
            if (CorrespondingCommand is CompoundCommand whileCmd && whileCmd.CondType == conditionalTypes.commWhile)
            {
                this.Program.PC = whileCmd.LineNumber;
                return;
            }

            // FOR: increment and jump back into the body if still running.
            if (CorrespondingCommand is AppFor forCmd)
            {
                bool cont = forCmd.Advance(this.Program);
                if (cont)
                    this.Program.PC = forCmd.LineNumber + 1;
                return;
            }

            // METHOD: return.
            if (CorrespondingCommand is AppMethod)
            {
                if (!MethodRuntime.Instance.HasFrame)
                {
                    // We reached end method as part of normal flow (should be skipped already).
                    return;
                }

                var frame = MethodRuntime.Instance.Pop();

                // Delete locals.
                foreach (string v in frame.Locals)
                {
                    if (this.Program.VariableExists(v))
                        this.Program.DeleteVariable(v);
                }

                this.Program.PC = frame.ReturnPc;
            }
        }
    }
}
