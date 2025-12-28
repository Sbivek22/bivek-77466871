using BOOSE;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using BOOSEInterpreter.Canvas;
using BOOSEInterpreter.Core;

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

        // ============================================================
        // RUN MULTILINE BUTTON
        // ============================================================

        /// <summary>
        /// Executes the multiline BOOSE code written in the input box.
        /// Clears the text box after successful execution but preserves the code for saving.
        /// </summary>
        private void runButton_Click(object sender, EventArgs e)
        {
            try
            {
                string code = inputBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(code))
                {
                    Log("ERROR: Multiline code is empty.");
                    return;
                }

                Log("Running Multiline:\n" + code);

                //lastExecutedCode = code; // preserve code
                //lastExecutedCode = code + Environment.NewLine;   // ensures commands do not join

                //fullCode += code + Environment.NewLine;   // add multiline commands to full code


                inputBox.Clear(); // clear UI display

                parser.ParseProgram(code);
                program.Run();

                // SAVE ONLY WHEN NO ERROR OCCURRED
                lastExecutedCode = code + Environment.NewLine;
                fullCode += code + Environment.NewLine;

                outputCanvas.Refresh();
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
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
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
