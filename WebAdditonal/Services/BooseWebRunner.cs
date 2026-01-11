using System.Reflection;
using BOOSE;
using BOOSEInterpreter.Core;
using BOOSEInterpreter.Core.Replace;
using WebAdditonal.Canvas;
using WebAdditonal.Models;

namespace WebAdditonal.Services
{
    public sealed class BooseWebRunner
    {
        public RunResponse Run(RunRequest req)
        {
            string code = (req.Code ?? "")
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Trim('\uFEFF')
                .Trim();

            if (string.IsNullOrWhiteSpace(code))
            {
                return new RunResponse { Success = false, Output = "ERROR: Code is empty." };
            }

            try
            {
                ResetBooseStaticRestrictionCounters();

                //// match your WinForms run behavior
                //MethodRegistry.Instance.Clear();
                //MethodRuntime.Instance.Clear();

                code = RewriteAssignments(code);

                using var canvas = new WebBitmapCanvas();
                canvas.Set(Math.Max(1, req.Width), Math.Max(1, req.Height));

                var factory = new AppCommandFactory(canvas);
                var program = new StoredProgram(canvas);
                var parser = new Parser(factory, program);

                parser.ParseProgram(code);
                program.Run();

                var png = canvas.ExportPng();
                return new RunResponse
                {
                    Success = true,
                    Output = "Program executed successfully.",
                    ImageBase64 = Convert.ToBase64String(png)
                };
            }
            catch (Exception ex)
            {
                return new RunResponse { Success = false, Output = "ERROR: " + ex.Message };
            }
        }

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
        }

        private static void ResetAllPrivateStaticIntFields(Type t)
        {
            try
            {
                foreach (var f in t.GetFields(BindingFlags.NonPublic | BindingFlags.Static))
                    if (f.FieldType == typeof(int))
                        f.SetValue(null, 0);
            }
            catch
            {
                // best-effort
            }
        }

        // copied from your Form1.cs behavior
        private static string RewriteAssignments(string code)
        {
            var lines = code.Split('\n');
            var declaredTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim().Trim('\uFEFF');

                if (line.StartsWith("int "))
                    declaredTypes[line.Split(' ', '=', ',')[1]] = "int";
                else if (line.StartsWith("real "))
                    declaredTypes[line.Split(' ', '=', ',')[1]] = "real";
                else if (line.StartsWith("method ", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3)
                    {
                        string returnType = parts[1].Trim();
                        string methodName = parts[2].Trim();
                        if (!string.IsNullOrWhiteSpace(methodName) && !string.IsNullOrWhiteSpace(returnType))
                            declaredTypes[methodName] = returnType;

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

            for (int i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();

                if (line.Contains("=") &&
                    !line.StartsWith("int ") &&
                    !line.StartsWith("real ") &&
                    !line.StartsWith("array ") &&
                    !line.StartsWith("if ") &&
                    !line.StartsWith("while ") &&
                    !line.StartsWith("for "))
                {
                    var left = line.Split('=')[0].Trim();
                    if (declaredTypes.TryGetValue(left, out var type))
                        lines[i] = $"{type} {line}";
                }
            }

            return string.Join("\n", lines);
        }
    }
}
