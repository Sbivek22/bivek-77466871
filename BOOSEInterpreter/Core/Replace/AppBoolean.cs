using System;
using System.Globalization;
using BOOSE;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Replace
{

    /// <summary>
    /// Unrestricted boolean variable implementation.
    /// </summary>
    /// <remarks>
    /// Stores a boolean value and integrates with the program's variable table. The
    /// declaration may optionally provide an initialiser expression (e.g. "boolean x = true").
    /// The evaluation and storage keep <see cref="BoolValue"/> and the base
    /// <see cref="Evaluation.Value"/> int representation in sync.
    /// </remarks>
    public class AppBoolean : Evaluation
    {
        /// <summary>
        /// The boolean value represented by this variable.
        /// </summary>
        public bool BoolValue { get; set; }

        /// <summary>
        /// Parses the declaration from <see cref="ParameterList"/>, extracting the variable name
        /// and optional initialiser expression. Registers the variable with the program if it
        /// does not already exist.
        /// </summary>
        /// <exception cref="CommandException">Thrown when no program is associated or when the
        /// declaration is missing a variable name.</exception>
        public override void Compile()
        {
            if (Program == null)
                throw new CommandException("Boolean has not been initialised with a StoredProgram.");

            string text = (ParameterList ?? string.Empty).Trim();
            if (text.Length == 0)
                throw new CommandException("Boolean requires a variable name.");

            int eq = text.IndexOf('=');
            if (eq >= 0)
            {
                VarName = text.Substring(0, eq).Trim().Split(' ')[^1];
                Expression = text.Substring(eq + 1).Trim();
            }
            else
            {
                VarName = text.Trim().Split(' ')[^1];
                Expression = "false";
            }

            if (!Program.VariableExists(VarName))
                Program.AddVariable(this);
        }

        /// <summary>
        /// Evaluates the boolean expression, updates <see cref="BoolValue"/> and the base
        /// integer <see cref="Evaluation.Value"/>, and stores or updates the variable in the
        /// program's variable table.
        /// </summary>
        /// <exception cref="CommandException">Thrown when the command is not associated with a
        /// <see cref="Program"/>.</exception>
        public override void Execute()
        {
            if (Program == null)
                throw new CommandException("Boolean has not been initialised with a StoredProgram.");

            bool b = BooseEval.Bool(Program, Expression ?? "false");
            BoolValue = b;
            base.Value = b ? 1 : 0;

            if (!Program.VariableExists(VarName))
            {
                Program.AddVariable(this);
            }
            else
            {
                var existing = Program.GetVariable(VarName);
                if (existing is AppBoolean ab)
                {
                    ab.BoolValue = b;
                    ab.Value = b ? 1 : 0;
                }
                else
                {
                    existing.Value = b ? 1 : 0;
                }
            }
        }
    }
}
