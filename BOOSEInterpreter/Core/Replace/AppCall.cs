using BOOSE;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BOOSEInterpreter.Core.Runtime;

namespace BOOSEInterpreter.Core.Replace
{
    /// <summary>
    /// Replace implementation of the BOOSE <c>call</c> command.
    ///
    /// The BOOSE library's <see cref="BOOSE.Call"/> ties into the restricted
    /// <see cref="BOOSE.Method"/> class. This rewritten version works with
    /// <see cref="AppMethod"/> and allows multiple methods/calls.
    /// </summary>
    // Fully qualify the BOOSE base class name because this project also has a
    // BOOSEInterpreter.Core.Command namespace.
    public sealed class AppCall : BOOSE.Command
    {
        private string methodName = string.Empty;
        private readonly List<string> argumentExpressions = new();

        /// <summary>
        /// Validates that at least one parameter (the method name) is provided.
        /// </summary>
        public override void CheckParameters(string[] parameters)
        {
            if (parameters == null || parameters.Length < 1)
                throw new CommandException("Call must include a method name.");
        }

        /// <summary>
        /// Parses the parameter list extracting the target method name and the list of
        /// argument expressions. Argument evaluation is deferred until <see cref="Execute"/>.
        /// </summary>
        public override void Compile()
        {
            base.Compile();

            // ParameterList includes everything after 'call'. Example: "mulMethod 10 5"
            var raw = (ParameterList ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(raw))
                throw new CommandException("Call must include a method name.");

            var parts = raw.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            methodName = parts[0].Trim();

            argumentExpressions.Clear();
            for (int i = 1; i < parts.Length; i++)
            {
                // BOOSE call arguments are simple literals/variables; we still allow expressions.
                argumentExpressions.Add(parts[i].Trim());
            }
        }

        /// <summary>
        /// Executes a method call by resolving the target <see cref="AppMethod"/>, creating
        /// and initialising local parameter variables, ensuring the return variable exists,
        /// pushing a call frame and jumping into the method body.
        /// </summary>
        /// <exception cref="CommandException">Thrown when the command is not attached to a
        /// <see cref="Program"/>, when the method cannot be found, when the argument count
        /// does not match, or when an unsupported parameter type is encountered.</exception>
        public override void Execute()
        {
            if (this.Program == null)
                throw new CommandException("Call has not been initialised with a StoredProgram.");

            AppMethod method;
            try
            {
                method = MethodRegistry.Instance.Get(methodName);
            }
            catch (InvalidOperationException ex)
            {
                throw new CommandException(ex.Message);
            }

            if (method.MethodParameters.Count != argumentExpressions.Count)
                throw new CommandException($"Method '{method.MethodName}' expects {method.MethodParameters.Count} argument(s) but got {argumentExpressions.Count}.");

            // Create local variables for each parameter and assign argument values.
            var locals = new List<string>();
            for (int i = 0; i < method.MethodParameters.Count; i++)
            {
                var (type, name) = method.MethodParameters[i];
                var argExp = argumentExpressions[i];

                // Evaluate now so the method body can use the parameter variables.
                if (type.Equals("int", StringComparison.OrdinalIgnoreCase))
                {
                    int value = (int)Math.Round(BooseEval.Double(this.Program, argExp));
                    var v = new AppInt { VarName = name, Value = value, Local = true };
                    this.Program.AddVariable(v);
                }
                else if (type.Equals("real", StringComparison.OrdinalIgnoreCase))
                {
                    double value = BooseEval.Double(this.Program, argExp);
                    var v = new AppReal { VarName = name, Value = value, Local = true };
                    this.Program.AddVariable(v);
                }
                else if (type.Equals("boolean", StringComparison.OrdinalIgnoreCase))
                {
                    bool b = BooseEval.Bool(this.Program, argExp);
                    // BOOSE stores booleans as 0/1 in EvaluateExpression; we store as 0/1 for compatibility.
                    var v = new AppInt { VarName = name, Value = b ? 1 : 0, Local = true };
                    this.Program.AddVariable(v);
                }
                else
                {
                    throw new CommandException($"Unsupported parameter type '{type}' for method '{method.MethodName}'.");
                }

                locals.Add(name);
            }

            // Ensure the return variable (method name) exists. This matches the example programs,
            // where 'write methodName' reads the value after the call.
            if (!this.Program.VariableExists(method.MethodName))
            {
                if (method.ReturnType.Equals("int", StringComparison.OrdinalIgnoreCase))
                {
                    this.Program.AddVariable(new AppInt { VarName = method.MethodName, Value = 0, Local = false });
                }
                else if (method.ReturnType.Equals("real", StringComparison.OrdinalIgnoreCase))
                {
                    this.Program.AddVariable(new AppReal { VarName = method.MethodName, Value = 0.0, Local = false });
                }
                else
                {
                    // Minimal support for coursework examples.
                    this.Program.AddVariable(new AppInt { VarName = method.MethodName, Value = 0, Local = false });
                }
            }

            // Save the return address (PC already points to the next command, since StoredProgram
            // increments PC before Execute()).
            MethodRuntime.Instance.Push(new MethodFrame(method.MethodName, this.Program.PC, locals));

            // Jump into method body (first command after the 'method' declaration).
            this.Program.PC = method.LineNumber + 1;
        }
    }
}
