using BOOSE;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using BOOSEInterpreter.Canvas;
using BOOSEInterpreter.Core;
using BOOSEInterpreter.Core.Replace;
using BOOSEInterpreter.Core.Command;
using BOOSEInterpreter.Core.Runtime;



namespace BOOSEInterpreter
{
    /// <summary>
    /// Main form class for the BOOSE Interpreter application.
    /// Handles UI interaction, parsing, execution, and canvas updates.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Parser instance used for interpreting BOOSE code.
        /// </summary>
        private Parser parser;


        /// <summary>
        /// Stores the compiled BOOSE program for execution.
        /// </summary>
        private StoredProgram program;

        /// <summary>
        /// Custom canvas used for drawing BOOSE graphics.
        /// </summary>
        private PanelCanvas canvas;

        /// <summary>
        /// Factory that creates BOOSE commands, including custom commands.
        /// </summary>
        private AppCommandFactory factory;

        /// <summary>
        /// Stores the previously executed code so it can be saved even after the input box is cleared.
        /// </summary>
        private string lastExecutedCode = "";
        /// <summary>
        /// Stores ALL executed commands (history) used when saving code.
        /// </summary>
        private string fullCode = "";


        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// Sets up all required BOOSE components and UI bindings.
        /// </summary>

        public Form1()
        {
            InitializeComponent();

            // Attach Enter event
            textBox1.KeyDown += textBox1_KeyDown;
            this.MinimumSize = this.Size;   //Lock minimum size
            this.MaximumSize = new Size(1800, 1800);  // optional: allow maximize
            this.MinimumSize = this.Size;
            // Create canvas FIRST
            canvas = new PanelCanvas(outputCanvas);

            // Interpreter components
            factory = new AppCommandFactory(canvas);
            program = new StoredProgram(canvas);
            parser = new Parser(factory, program);

            // Initialize canvas once form loads
            this.Load += (s, e) =>
            {
                canvas.Set(outputCanvas.Width, outputCanvas.Height);
                debugBox.AppendText("Canvas ready.\n");
            };
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Optional: refresh if resized
            if (outputCanvas.Image == null)
            {
                canvas.Set(outputCanvas.Width, outputCanvas.Height);
            }
            outputCanvas.BackColor = Color.White;
            debugBox.AppendText("Canvas initialized.\n");
        }


        // ============================================================
        // DEBUG HELPER
        // ============================================================

        /// <summary>
        /// Appends a message to the debug console area.
        /// </summary>
        /// <param name="message">Message to output.</param>
        private void Log(string message)
        {
            debugBox.AppendText(message + Environment.NewLine);
        }

        /// <summary>
        /// BOOSE.dll implements coursework restrictions using <b>static counters</b> inside several
        /// BOOSE classes (e.g., BOOSE.Boolean/Int/Real). Those counters persist for the lifetime of
        /// the application process, so running multiple example programs without closing the app
        /// can incorrectly accumulate counts and trigger "Variable limit reached" errors.
        /// 
        /// We reset those counters on each run so each program starts from a clean restriction state.
        /// This mirrors what happens naturally when you close and reopen the application.
        /// </summary>
        private static void ResetBooseStaticRestrictionCounters()
        {
            // Reset variable-type counters (these are the ones that commonly surface in errors).
            ResetAllPrivateStaticIntFields(typeof(BOOSE.Boolean));
            ResetAllPrivateStaticIntFields(typeof(BOOSE.Int));
            ResetAllPrivateStaticIntFields(typeof(BOOSE.Real));

            // Reset common control-flow restriction counters in case the BOOSE implementations are
            // ever used (or mixed) during parsing.
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
                {
                    if (f.FieldType == typeof(int))
                        f.SetValue(null, 0);
                }
            }
            catch
            {
                // Best-effort reset only; never block execution if reflection is unavailable.
            }
        }

        private string RewriteAssignments(string code)
        {
            var lines = code.Split('\n');
            var declaredTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // First pass: collect declared variable types
            foreach (var rawLine in lines)
            {
                var line = rawLine.Trim().Trim('\uFEFF');

                if (line.StartsWith("int "))
                    declaredTypes[line.Split(' ', '=', ',')[1]] = "int";
                else if (line.StartsWith("real "))
                    declaredTypes[line.Split(' ', '=', ',')[1]] = "real";

                // Also capture method return/parameter types so method bodies like:
                //   mulMethod = one * two
                // can be rewritten to a typed assignment that our interpreter supports.
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

                        // Parse parameters after method name.
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

            // Second pass: rewrite assignment-only lines
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
                    {
                        lines[i] = $"{type} {line}";
                    }
                }
            }

            return string.Join("\n", lines);
        }


        // ============================================================
        // RUN MULTILINE BUTTON
        // ============================================================

        private void runButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure BOOSE.dll restriction counters do not accumulate across multiple runs
                // within the same application session.
                ResetBooseStaticRestrictionCounters();

                // Normalize (important for BOOSE parsing + BOM issues)
                string code = (inputBox.Text ?? "")
                    .Replace("\r\n", "\n")
                    .Replace("\r", "\n")
                    .Trim('\uFEFF')
                    .Trim();

                if (string.IsNullOrWhiteSpace(code))
                {
                    Log("ERROR: Multiline code is empty.");
                    return;
                }

                Log("Running Multiline:\n" + code);

                // Ensure canvas has a bitmap to draw onto
                if (outputCanvas.Width > 0 && outputCanvas.Height > 0)
                {
                    if (outputCanvas.Image == null || canvas.getBitmap() == null)
                    {
                        canvas.Set(outputCanvas.Width, outputCanvas.Height);
                        outputCanvas.Image = (Bitmap)canvas.getBitmap();
                    }
                }

                // Recreate program+parser each run to avoid stacking old commands
                //program = new StoredProgram(canvas);
                //parser = new Parser(factory, program);

                // Reset method runtime state between runs (prevents stale frames/methods when a previous
                // execution terminated early).
                BOOSEInterpreter.Core.Replace.MethodRegistry.Instance.Clear();
                BOOSEInterpreter.Core.Replace.MethodRuntime.Instance.Clear();

                code = RewriteAssignments(code);   // ← ADD THIS LINE

                parser.ParseProgram(code);
                program.Run();

                // IMPORTANT: assign bitmap back (so PictureBox displays latest drawing)
                outputCanvas.Image = (Bitmap)canvas.getBitmap();
                outputCanvas.Refresh();

                // Clear UI only after successful execution
                inputBox.Clear();

                // Save history only after success
                lastExecutedCode = code + Environment.NewLine;
                fullCode += code + Environment.NewLine;

                Log("Program executed successfully.");
            }
            catch (Exception ex)
            {
                Log("ERROR: " + ex.Message);
            }
        }


        // ============================================================
        // RUN SINGLE LINE
        // ============================================================

        /// <summary>
        /// Unused text-changed event for the single-line input box.
        /// </summary>
        private void textBox1_TextChanged(object sender, EventArgs e) { }

        /// <summary>
        /// Parses and executes a single BOOSE command entered by the user.
        /// </summary>
        /// <param name="line">The command to execute.</param>
        private void RunSingleLine(string line)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line))
                    return;

                Log("Running: " + line);

                //lastExecutedCode += line + Environment.NewLine;



                ICommand command = parser.ParseCommand(line);
                command.Execute();

                lastExecutedCode += line + Environment.NewLine;
                fullCode += line + Environment.NewLine;   // add single-line commands to full code


                outputCanvas.Refresh();
            }
            catch (Exception ex)
            {
                Log("ERROR: " + ex.Message);
            }
        }

        /// <summary>
        /// Detects when the user presses Enter in the single-line input box
        /// and immediately executes the command.
        /// </summary>
        private void textBox1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                RunSingleLine(textBox1.Text.Trim());
                textBox1.Clear();
            }
        }

        // ============================================================
        // MENU: NEW
        // ============================================================

        /// <summary>
        /// Clears all input, output, canvas, and debug logs.
        /// Resets workspace to blank.
        /// </summary>
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputBox.Clear();
            debugBox.Clear();
            textBox1.Clear();
            canvas.Clear();
            outputCanvas.Image = (Bitmap)canvas.getBitmap();

            lastExecutedCode = "";
            fullCode = "";
            program = new StoredProgram(canvas);
            parser = new Parser(factory, program);

            Log("New workspace created.");
        }

        // ============================================================
        // MENU: LOAD CODE
        // ============================================================

        /// <summary>
        /// Loads BOOSE code from a .boose file into the multiline input box.
        /// </summary>
        private void codeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "BOOSE Code (*.boose)|*.boose";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string loadedCode = File.ReadAllText(dlg.FileName);

                inputBox.Text = loadedCode;
                Log("Loaded BOOSE code.");

                // >>> CLEAR OLD HISTORY so wrong commands do NOT stay in memory
                lastExecutedCode = "";
                fullCode = "";

                // >>> REBUILD HISTORY ONLY FROM THE FILE CONTENT
                lastExecutedCode = loadedCode + Environment.NewLine;
                fullCode = loadedCode + Environment.NewLine;
            }
        }


        // ============================================================
        // MENU: LOAD IMAGE
        // ============================================================

        /// <summary>
        /// Loads an external image and displays it on the drawing canvas.
        /// </summary>
        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image Files|*.png;*.jpg;*.jpeg";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                outputCanvas.Image = Image.FromFile(dlg.FileName);
                Log("Image loaded to canvas.");
            }
        }

        // ============================================================
        // MENU: SAVE CODE
        // ============================================================

        /// <summary>
        /// Saves the last executed BOOSE code to a .boose file.
        /// Works even if the input box is empty.
        /// </summary>
        private void codeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "BOOSE Code (*.boose)|*.boose";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                //File.WriteAllText(dlg.FileName, lastExecutedCode);
                File.WriteAllText(dlg.FileName, fullCode);   // save full command history

                Log("Code saved.");
            }
        }

        // ============================================================
        // MENU: SAVE IMAGE
        // ============================================================

        /// <summary>
        /// Saves the current drawing canvas image as a PNG file.
        /// </summary>
        private void imageToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "PNG Image|*.png";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                outputCanvas.Image.Save(dlg.FileName);
                Log("Canvas image saved.");
            }
        }

        // ============================================================
        // MENU: CLEAR CANVAS
        // ============================================================

        /// <summary>
        /// Clears the drawing canvas and the debug output.
        /// </summary>
        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputBox.Clear();
            canvas.Clear();
            debugBox.Clear();
            outputCanvas.Image = (Bitmap)canvas.getBitmap();
            Log("Canvas cleared.");
        }

        // ============================================================
        // MENU: EXIT
        // ============================================================

        /// <summary>
        /// Safely closes the application.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // ============================================================
        // MENU: ABOUT
        // ============================================================

        /// <summary>
        /// Shows basic information about the BOOSE Interpreter application.
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("BOOSE Interpreter\nDeveloped by Bivek Kumar sah", "About", MessageBoxButtons.OK);
        }

        /// <summary>
        /// Click handler for the input label. Intentionally empty (no action required).
        /// </summary>
        /// <param name="sender">The control that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void lblInput_Click(object sender, EventArgs e) { }


        /// <summary>
        /// Click handler for the output label. Intentionally empty (no action required).
        /// </summary>
        /// <param name="sender">The control that raised the event.</param>
        /// <param name="e">Event arguments.</param>
        private void lblOutput_Click(object sender, EventArgs e) { }
    }
}
