using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BOOSE;
using BOOSEInterpreter.Core;

namespace BOOSEInterpreterTests.TestHelpers
{
    /// <summary>
    /// Runs BOOSE programs in tests using the same preprocessing/reset behaviour as Form1.
    /// </summary>
    public static class InterpreterHarness
    {
        public static (StoredProgram program, RecordingCanvas canvas) Run(string code)
        {
            // 1) Reset BOOSE.dll restriction counters (static fields) so tests don't accumulate limits
            ResetBooseStaticRestrictionCounters();

            // 2) Reset method runtime state between tests
            ResetMethodGlobals();

            // 3) Normalize input like Form1 does
            code = Normalize(code);

            // 4) Rewrite assignment-only lines into typed lines (critical!)
            code = RewriteAssignments(code);

            var canvas = new RecordingCanvas();
            var factory = new AppCommandFactory(canvas);
            var program = new StoredProgram(canvas);
            var parser = new Parser(factory, program);

            parser.ParseProgram(code);
            program.Run();

            return (program, canvas);
        }

        public static int GetInt(StoredProgram p, string name) => p.GetVariable(name).Value;

        public static double GetReal(StoredProgram p, string name)
        {
            var v = p.GetVariable(name);
            if (v is BOOSEInterpreter.Core.Replace.AppReal ar) return ar.Value;
            return v.Value;
        }

        public static bool GetBool(StoredProgram p, string name)
        {
            var v = p.GetVariable(name);
            if (v is BOOSEInterpreter.Core.Replace.AppBoolean ab) return ab.BoolValue;
            return v.Value != 0;
        }

        private static string Normalize(string code)
        {
            return (code ?? "")
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Trim('\uFEFF')
                .Trim();
        }

        /// <summary>
        /// Same logic as Form1.RewriteAssignments: converts "x = expr" into "int x = expr" / "real x = expr"
        /// based on previously declared types (also method return + parameter types).
        /// </summary>
        private static string RewriteAssignments(string code)
        {
            var lines = code.Split('\n');
            var declaredTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Pass 1: collect declared variable types + method return/parameter types
            foreach (var rawLine in lines)
            {
                var line = (rawLine ?? "").Trim().Trim('\uFEFF');
                if (line.Length == 0) continue;

                if (line.StartsWith("int ", StringComparison.OrdinalIgnoreCase))
                {
                    var name = line.Split(' ', '=', ',')[1];
                    declaredTypes[name] = "int";
                }
                else if (line.StartsWith("real ", StringComparison.OrdinalIgnoreCase))
                {
                    var name = line.Split(' ', '=', ',')[1];
                    declaredTypes[name] = "real";
                }
                else if (line.StartsWith("boolean ", StringComparison.OrdinalIgnoreCase))
                {
                    var name = line.Split(' ', '=', ',')[1];
                    declaredTypes[name] = "boolean";
                }
                else if (line.StartsWith("method ", StringComparison.OrdinalIgnoreCase))
                {
                    // method <returnType> <methodName> [<type> <name>, ...]
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3)
                    {
                        string returnType = parts[1].Trim();
                        string methodName = parts[2].Trim();

                        if (!string.IsNullOrWhiteSpace(methodName) && !string.IsNullOrWhiteSpace(returnType))
                            declaredTypes[methodName] = returnType;

                        // parameters after method name (comma-separated)
                        int idx = line.IndexOf(methodName, StringComparison.OrdinalIgnoreCase);
                        if (idx >= 0)
                        {
                            string remainder = line.Substring(idx + methodName.Length).Trim();
                            if (!string.IsNullOrWhiteSpace(remainder))
                            {
                                var paramDecls = remainder.Split(',')
                                    .Select(p => p.Trim())
                                    .Where(p => p.Length > 0);

                                foreach (var decl in paramDecls)
                                {
                                    var t = decl.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (t.Length == 2)
                                        declaredTypes[t[1]] = t[0];
                                }
                            }
                        }
                    }
                }
            }

            // Pass 2: rewrite assignment-only lines
            for (int i = 0; i < lines.Length; i++)
            {
                var line = (lines[i] ?? "").Trim();
                if (line.Length == 0) continue;

                bool looksLikeAssignment = line.Contains("=");

                bool isTypedDecl =
                    line.StartsWith("int ", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("real ", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("boolean ", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("array ", StringComparison.OrdinalIgnoreCase);

                bool isControl =
                    line.StartsWith("if ", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("while ", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("for ", StringComparison.OrdinalIgnoreCase) ||
                    line.StartsWith("method ", StringComparison.OrdinalIgnoreCase);

                if (looksLikeAssignment && !isTypedDecl && !isControl)
                {
                    var left = line.Split('=')[0].Trim();
                    if (declaredTypes.TryGetValue(left, out var type))
                    {
                        lines[i] = $"{type} {line}";
                    }
                }
            }

            return string.Join("\n", lines);
        }

        /// <summary>
        /// Mirrors Form1.ResetBooseStaticRestrictionCounters.
        /// BOOSE.dll uses static counters to enforce coursework restrictions; we reset them per test run.
        /// </summary>
        private static void ResetBooseStaticRestrictionCounters()
        {
            ResetAllPrivateStaticIntFields(typeof(BOOSE.Boolean));
            ResetAllPrivateStaticIntFields(typeof(BOOSE.Int));
            ResetAllPrivateStaticIntFields(typeof(BOOSE.Real));

            ResetAllPrivateStaticIntFields(typeof(BOOSE.If));
            ResetAllPrivateStaticIntFields(typeof(BOOSE.Else));
            ResetAllPrivateStaticIntFields(typeof(BOOSE.While));
            ResetAllPrivateStaticIntFields(typeof(BOOSE.For));
            ResetAllPrivateStaticIntFields(typeof(BOOSE.End));
            ResetAllPrivateStaticIntFields(typeof(BOOSE.Method));
            ResetAllPrivateStaticIntFields(typeof(BOOSE.Array));

            var asm = typeof(StoredProgram).Assembly;

            foreach (var t in asm.GetTypes())
            {
                if (t.Namespace != "BOOSE") continue;

                foreach (var f in t.GetFields(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    // reset only normal static int fields (NOT constants/readonly)
                    if (f.FieldType == typeof(int) && !f.IsInitOnly)
                    {
                        try { f.SetValue(null, 0); } catch { /* best effort */ }
                    }
                }
            }

        }

        private static void ResetAllPrivateStaticIntFields(Type t)
        {
            try
            {
                foreach (var f in t.GetFields(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    if (f.FieldType == typeof(int))
                        f.SetValue(null, 0);
                }
            }
            catch
            {
                // best effort
            }
        }

        private static void ResetMethodGlobals()
        {
            var asm = typeof(AppCommandFactory).Assembly;

            var regType = asm.GetType("BOOSEInterpreter.Core.Replace.MethodRegistry");
            var regInst = regType?.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
            regType?.GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance)?.Invoke(regInst, null);

            var rtType = asm.GetType("BOOSEInterpreter.Core.Replace.MethodRuntime");
            var rtInst = rtType?.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
            rtType?.GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance)?.Invoke(rtInst, null);
        }
    }
}
