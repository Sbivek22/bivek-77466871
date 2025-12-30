using BOOSE;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Unrestricted implementation of the BOOSE <c>method</c> declaration.
    /// 
    /// The BOOSE library's <see cref="BOOSE.Method"/> class enforces restrictions (e.g., only a
    /// single method). This class rewrites the behaviour so that multiple methods are allowed.
    /// 
    /// Syntax example:
    /// <code>
    /// method int mulMethod int one, int two
    ///   mulMethod = one * two
    /// end method
    /// </code>
    /// </summary>
    public sealed class AppMethod : CompoundCommand
    {
        private readonly List<(string Type, string Name)> methodParameters = new();

        /// <summary>
        /// Gets the return type as declared (e.g. <c>int</c>, <c>real</c>).
        /// </summary>
        public string ReturnType { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the method name.
        /// </summary>
        public string MethodName { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the ordered list of method parameters.
        /// </summary>
        public IReadOnlyList<(string Type, string Name)> MethodParameters => methodParameters;

        /// <summary>
        /// BOOSE base method does not validate beyond compile-time parsing.
        /// This override intentionally performs no additional checks.
        /// </summary>
        public override void CheckParameters(string[] parameter)
        {
            // BOOSE base method does not validate beyond compile-time parsing.
        }

        /// <summary>
        /// Parses the method signature (return type, name and parameters) and registers
        /// the declaration in the program so that <c>call</c> can locate it.
        /// </summary>
        /// <exception cref="CommandException">Thrown when the declaration is missing or
        /// malformed, or when no program is associated with this command.</exception>
        public override void Compile()
        {
            if (this.Program == null)
                throw new CommandException("Method has not been initialised with a StoredProgram.");

            // The Command base class stores everything after the keyword in ParameterList.
            // e.g. "int mulMethod int one, int two"
            var sig = (ParameterList ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(sig))
                throw new CommandException("Method declaration missing signature.");

            // First two tokens: return type + method name.
            var parts = sig.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2)
                throw new CommandException("Method declaration must include return type and name.");

            ReturnType = parts[0].Trim();
            MethodName = parts[1].Trim();

            // Remaining text (if any) is parameter declarations, possibly containing commas.
            methodParameters.Clear();
            string remainder = sig.Substring(sig.IndexOf(MethodName, StringComparison.Ordinal) + MethodName.Length).Trim();

            if (!string.IsNullOrWhiteSpace(remainder))
            {
                // Expect: "int one, int two" (commas optional if only one param)
                var paramDecls = remainder.Split(',')
                    .Select(p => p.Trim())
                    .Where(p => p.Length > 0)
                    .ToArray();

                foreach (var decl in paramDecls)
                {
                    var tokens = decl.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length != 2)
                        throw new CommandException($"Invalid parameter declaration '{decl}'. Expected '<type> <name>'.");

                    methodParameters.Add((tokens[0].Trim(), tokens[1].Trim()));
                }
            }

            // Stack pairing with 'end method' is handled by our AppEnd.Compile.
            this.Program.Push(this);
        }

        /// <summary>
        /// Registers the method declaration in the runtime <see cref="MethodRegistry"/>
        /// and skips execution of the method body at top-level program execution. The
        /// method body is entered only when a corresponding <c>call</c> command is executed.
        /// </summary>
        public override void Execute()
        {
            // Register the method so it can be called.
            MethodRegistry.Instance.Register(this);

            // During normal (top-level) execution, method bodies are skipped.
            // Execution will jump into the body when a 'call' happens.
            if (this.Program == null)
                throw new CommandException("Method has not been initialised with a StoredProgram.");

            this.Program.PC = EndLineNumber + 1;
        }
    }
}
