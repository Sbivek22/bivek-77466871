using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System;

namespace BOOSEInterpreter
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.RichTextBox inputBox;
        private System.Windows.Forms.PictureBox outputCanvas;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.Label lblInput;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.RichTextBox debugBox;
        private System.Windows.Forms.Label lblDebug;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Form1));
            inputBox = new RichTextBox();
            outputCanvas = new PictureBox();
            runButton = new Button();
            lblInput = new Label();
            lblOutput = new Label();
            textBox1 = new TextBox();
            label1 = new Label();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            loadToolStripMenuItem = new ToolStripMenuItem();
            codeToolStripMenuItem = new ToolStripMenuItem();
            imageToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            codeToolStripMenuItem1 = new ToolStripMenuItem();
            imageToolStripMenuItem1 = new ToolStripMenuItem();
            clearToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            debugBox = new RichTextBox();
            lblDebug = new Label();
            button1 = new Button();
            ((ISupportInitialize)outputCanvas).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // inputBox
            // 
            inputBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            inputBox.BackColor = Color.Silver;
            inputBox.Font = new Font("Consolas", 11F);
            inputBox.Location = new Point(22, 74);
            inputBox.Name = "inputBox";
            inputBox.Size = new Size(388, 379);
            inputBox.TabIndex = 0;
            inputBox.Text = "";
            // 
            // outputCanvas
            // 
            outputCanvas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            outputCanvas.BackColor = Color.White;
            outputCanvas.BorderStyle = BorderStyle.Fixed3D;
            outputCanvas.Location = new Point(432, 74);
            outputCanvas.Name = "outputCanvas";
            outputCanvas.Size = new Size(490, 379);
            outputCanvas.SizeMode = PictureBoxSizeMode.StretchImage;
            outputCanvas.TabIndex = 1;
            outputCanvas.TabStop = false;
            // 
            // runButton
            // 
            runButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            runButton.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            runButton.Location = new Point(54, 549);
            runButton.Name = "runButton";
            runButton.Size = new Size(120, 40);
            runButton.TabIndex = 2;
            runButton.Text = "RUN";
            runButton.Click += runButton_Click;
            // 
            // lblInput
            // 
            lblInput.AutoSize = true;
            lblInput.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblInput.Location = new Point(135, 41);
            lblInput.Name = "lblInput";
            lblInput.Size = new Size(167, 30);
            lblInput.TabIndex = 3;
            lblInput.Text = "Multiline Input";
            // 
            // lblOutput
            // 
            lblOutput.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblOutput.AutoSize = true;
            lblOutput.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblOutput.Location = new Point(620, 36);
            lblOutput.Name = "lblOutput";
            lblOutput.Size = new Size(88, 30);
            lblOutput.TabIndex = 4;
            lblOutput.Text = "Output";
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.BackColor = Color.Silver;
            textBox1.Location = new Point(22, 503);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(386, 31);
            textBox1.TabIndex = 5;
            textBox1.KeyDown += textBox1_KeyDown;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            label1.Location = new Point(94, 464);
            label1.Name = "label1";
            label1.Size = new Size(180, 30);
            label1.TabIndex = 6;
            label1.Text = "SingleLine Input";
            // 
            // menuStrip1
            // 
            menuStrip1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            menuStrip1.BackColor = SystemColors.ControlDark;
            menuStrip1.Dock = DockStyle.None;
            menuStrip1.ImageScalingSize = new Size(24, 24);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(127, 33);
            menuStrip1.TabIndex = 8;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, loadToolStripMenuItem, saveToolStripMenuItem, clearToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.Size = new Size(153, 34);
            newToolStripMenuItem.Text = "New";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { codeToolStripMenuItem, imageToolStripMenuItem });
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new Size(153, 34);
            loadToolStripMenuItem.Text = "Load";
            // 
            // codeToolStripMenuItem
            // 
            codeToolStripMenuItem.Name = "codeToolStripMenuItem";
            codeToolStripMenuItem.Size = new Size(164, 34);
            codeToolStripMenuItem.Text = "Code";
            codeToolStripMenuItem.Click += codeToolStripMenuItem_Click;
            // 
            // imageToolStripMenuItem
            // 
            imageToolStripMenuItem.Name = "imageToolStripMenuItem";
            imageToolStripMenuItem.Size = new Size(164, 34);
            imageToolStripMenuItem.Text = "Image";
            imageToolStripMenuItem.Click += imageToolStripMenuItem_Click;
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { codeToolStripMenuItem1, imageToolStripMenuItem1 });
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(153, 34);
            saveToolStripMenuItem.Text = "Save";
            // 
            // codeToolStripMenuItem1
            // 
            codeToolStripMenuItem1.Name = "codeToolStripMenuItem1";
            codeToolStripMenuItem1.Size = new Size(164, 34);
            codeToolStripMenuItem1.Text = "Code";
            codeToolStripMenuItem1.Click += codeToolStripMenuItem1_Click;
            // 
            // imageToolStripMenuItem1
            // 
            imageToolStripMenuItem1.Name = "imageToolStripMenuItem1";
            imageToolStripMenuItem1.Size = new Size(164, 34);
            imageToolStripMenuItem1.Text = "Image";
            imageToolStripMenuItem1.Click += imageToolStripMenuItem1_Click;
            // 
            // clearToolStripMenuItem
            // 
            clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            clearToolStripMenuItem.Size = new Size(153, 34);
            clearToolStripMenuItem.Text = "Clear";
            clearToolStripMenuItem.Click += clearToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(153, 34);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(65, 29);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(164, 34);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // debugBox
            // 
            debugBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            debugBox.BackColor = Color.Black;
            debugBox.Font = new Font("Consolas", 10F);
            debugBox.ForeColor = Color.White;
            debugBox.Location = new Point(432, 498);
            debugBox.Name = "debugBox";
            debugBox.ReadOnly = true;
            debugBox.Size = new Size(490, 91);
            debugBox.TabIndex = 7;
            debugBox.Text = "";
            // 
            // lblDebug
            // 
            lblDebug.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblDebug.AutoSize = true;
            lblDebug.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblDebug.Location = new Point(626, 461);
            lblDebug.Name = "lblDebug";
            lblDebug.Size = new Size(82, 30);
            lblDebug.TabIndex = 9;
            lblDebug.Text = "Debug";
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button1.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            button1.Location = new Point(200, 549);
            button1.Name = "button1";
            button1.Size = new Size(120, 40);
            button1.TabIndex = 7;
            button1.Text = "Check";
            button1.Click += runButton_Click;
            // 
            // Form1
            // 
            BackColor = Color.DarkCyan;
            ClientSize = new Size(938, 606);
            Controls.Add(textBox1);
            Controls.Add(lblOutput);
            Controls.Add(label1);
            Controls.Add(lblInput);
            Controls.Add(button1);
            Controls.Add(runButton);
            Controls.Add(outputCanvas);
            Controls.Add(inputBox);
            Controls.Add(menuStrip1);
            Controls.Add(debugBox);
            Controls.Add(lblDebug);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "BIVEK 8582";
            Load += Form1_Load;
            ((ISupportInitialize)outputCanvas).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Label label1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
        private ToolStripMenuItem codeToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem imageToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem codeToolStripMenuItem1;
        private ToolStripMenuItem imageToolStripMenuItem1;
        private ToolStripMenuItem clearToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private Button button1;
    }
}
