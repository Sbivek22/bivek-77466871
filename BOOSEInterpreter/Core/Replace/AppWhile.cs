using System;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Represents an unrestricted BOOSE <c>while</c> loop command.
    /// </summary>
    /// <remarks>
    /// During compilation, this command records the loop's start line and pushes itself
    /// onto the program's compound stack so it can later be matched with a corresponding
    /// <c>End</c> command.
    /// At runtime, <see cref="Execute"/> evaluates the loop condition and advances the
    /// program counter to exit the loop when the condition is false.
    /// The loop condition expression is taken from <see cref="Command.ParameterList"/>
    /// and stored in <see cref="CompoundCommand.Expression"/> during <see cref="Compile"/>.
    /// </remarks>
    public class AppWhile : CompoundCommand
    {
        /// <summary>
        /// Compiles the <c>while</c> command by capturing the current line number, storing the
        /// conditional expression, and pushing this command onto the program compound stack.
        /// </summary>
        /// <remarks>
        /// This enables a matching <c>End</c> command (e.g., end-while) to resolve the loop
        /// boundaries and set <see cref="CompoundCommand.EndLineNumber"/>.
        /// </remarks>
        /// <exception cref="CommandException">
        /// Thrown when <see cref="Command.Program"/> has not been assigned.
        /// </exception>
        public override void Compile()
        {
            if (this.Program == null)
                throw new CommandException("While has not been initialised with a StoredProgram.");

            LineNumber = this.Program.Count - 1;
            CondType = conditionalTypes.commWhile;
            Expression = (ParameterList ?? string.Empty).Trim();
            this.Program.Push(this);
        }

        /// <summary>
        /// Executes the <c>while</c> command by evaluating the stored conditional expression.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="BooseEval.Bool(StoredProgram, string)"/> to evaluate
        /// <see cref="CompoundCommand.Expression"/> in the current program context.
        /// If the expression evaluates to <c>false</c>, execution skips the loop body by setting
        /// <see cref="StoredProgram.PC"/> to <c><see cref="CompoundCommand.EndLineNumber"/> + 1</c>.
        /// If the expression evaluates to <c>true</c>, execution continues into the loop body.
        /// </remarks>
        /// <exception cref="CommandException">
        /// Thrown when <see cref="Command.Program"/> has not been assigned.
        /// </exception>
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
