using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BOOSEInterpreter.Canvas;
using BOOSEInterpreter.Core;
using BOOSE;
using System.Drawing;
using System.Windows.Forms;
using BOOSEInterpreter;

namespace BOOSEInterpreterTests
{
    /// <summary>
    /// Unit tests for the <see cref="PanelCanvas"/> drawing surface.
    /// </summary>
    /// <remarks>
    /// This test fixture exercises basic drawing operations provided by <see cref="PanelCanvas"/>
    /// such as moving the pen, drawing lines and shapes, and integrating with the interpreter
    /// pipeline (<see cref="Parser"/> + <see cref="StoredProgram"/>). Tests create an off-screen
    /// <see cref="PictureBox"/> so no visual UI interaction is required and tests remain
    /// deterministic when run in CI pipelines.
    /// </remarks>
    [TestClass]
    public class CanvasTests
    {
        /// <summary>
        /// Creates a fresh <see cref="PictureBox"/> configured as an off-screen drawing target
        /// for use by the canvas unit tests.
        /// </summary>
        /// <remarks>
        /// The returned <see cref="PictureBox"/> has a preset 400x400 size and an initial
        /// <see cref="Bitmap"/> assigned to its <see cref="PictureBox.Image"/> property. Using
        /// an in-memory <see cref="PictureBox"/> avoids showing any UI while still providing a
        /// valid drawing surface backed by a <see cref="Bitmap"/>.
        /// </remarks>
        /// <returns>A configured <see cref="PictureBox"/> with a writable <see cref="Bitmap"/> image.</returns>
        /// <example>
        /// <code>
        /// PictureBox box = CreatePictureBox();
        /// PanelCanvas canvas = new PanelCanvas(box);
        /// </code>
        /// </example>
        private PictureBox CreatePictureBox()
        {
            return new PictureBox()
            {
                Width = 400,
                Height = 400,
                Image = new Bitmap(400, 400)
            };
        }

        // ============================================================
        // 1. MOVETO TEST
        // ============================================================

        /// <summary>
        /// Verifies that <see cref="PanelCanvas.MoveTo(int,int)"/> updates the internal cursor coordinates.
        /// </summary>
        /// <remarks>
        /// Arrange: create a fresh canvas. Act: call <see cref="PanelCanvas.MoveTo(int,int)"/>.
        /// Assert: the <see cref="PanelCanvas.Xpos"/> and <see cref="PanelCanvas.Ypos"/> values
        /// reflect the requested coordinates.
        /// </remarks>
        /// <example>
        /// <code>
        /// PictureBox box = CreatePictureBox();
        /// PanelCanvas canvas = new PanelCanvas(box);
        /// canvas.MoveTo(50,60);
        /// Assert.AreEqual(50, canvas.Xpos);
        /// </code>
        /// </example>
        /// <seealso cref="PanelCanvas.MoveTo(int,int)"/>
        [TestMethod]
        public void Test_MoveTo_UpdatesCursorCorrectly()
        {
            // Arrange
            PictureBox box = CreatePictureBox();
            PanelCanvas canvas = new PanelCanvas(box);

            // Act
            canvas.MoveTo(50, 60);

            // Assert
            Assert.AreEqual(50, canvas.Xpos, "Xpos should match the MoveTo X value.");
            Assert.AreEqual(60, canvas.Ypos, "Ypos should match the MoveTo Y value.");
        }

        // ============================================================
        // 2. DRAWTO TEST
        // ============================================================

        /// <summary>
        /// Verifies that <see cref="PanelCanvas.DrawTo(int,int)"/> draws a line and updates the cursor
        /// to the target coordinates.
        /// </summary>
        /// <remarks>
        /// Arrange: create a fresh canvas and set the starting position to (0,0).
        /// Act: call <see cref="PanelCanvas.DrawTo(int,int)"/> to draw to (120,80).
        /// Assert: the canvas position is updated to the target coordinates. The test does not
        /// validate the pixel contents of the bitmap; it focuses on state changes performed by the method.
        /// </remarks>
        /// <example>
        /// <code>
        /// canvas.MoveTo(0,0);
        /// canvas.DrawTo(120,80);
        /// Assert.AreEqual(120, canvas.Xpos);
        /// </code>
        /// </example>
        /// <seealso cref="PanelCanvas.DrawTo(int,int)"/>
        [TestMethod]
        public void Test_DrawTo_UpdatesCursorCorrectly()
        {
            // Arrange
            PictureBox box = CreatePictureBox();
            PanelCanvas canvas = new PanelCanvas(box);

            // Start at 0,0
            canvas.MoveTo(0, 0);

            // Act
            canvas.DrawTo(120, 80);

            // Assert
            Assert.AreEqual(120, canvas.Xpos, "Xpos should update to DrawTo target X.");
            Assert.AreEqual(80, canvas.Ypos, "Ypos should update to DrawTo target Y.");
        }

        // ============================================================
        // 3. MULTILINE PROGRAM TEST
        // ============================================================

        /// <summary>
        /// Executes a small multiline program through the interpreter and ensures the resulting
        /// pen position and drawing operations are applied to the canvas as expected.
        /// </summary>
        /// <remarks>
        /// Arrange: create a canvas, a factory and program/parser pipeline. The program contains
        /// three commands that move the pen, draw a line and draw a circle. Act: parse the program
        /// and run it. Assert: check the final pen coordinates correspond to the last drawing command.
        /// This test verifies integration between <see cref="AppCommandFactory"/>, <see cref="Parser"/>
        /// and <see cref="StoredProgram"/> when operating on a real <see cref="PanelCanvas"/>.
        /// </remarks>
        /// <example>
        /// <code>
        /// string code = "moveto 10,10\ndrawto 100,50\ncircle 20\n";
        /// parser.ParseProgram(code);
        /// program.Run();
        /// Assert.AreEqual(100, canvas.Xpos);
        /// </code>
        /// </example>
        /// <seealso cref="Parser"/>
        /// <seealso cref="StoredProgram"/>
        /// <seealso cref="AppCommandFactory"/>
        [TestMethod]
        public void Test_MultilineProgram_RunsCorrectly()
        {
            // Arrange
            PictureBox box = CreatePictureBox();
            PanelCanvas canvas = new PanelCanvas(box);
            AppCommandFactory factory = new AppCommandFactory(canvas);
            StoredProgram program = new StoredProgram(canvas);
            Parser parser = new Parser(factory, program);

            string code =
                "moveto 10,10\n" +
                "drawto 100,50\n" +
                "circle 20\n";

            // Act
            parser.ParseProgram(code);
            program.Run();

            // Assert: Final position should be from drawto (100,50)
            Assert.AreEqual(100, canvas.Xpos, "Final Xpos incorrect after multiline program execution.");
            Assert.AreEqual(50, canvas.Ypos, "Final Ypos incorrect after multiline program execution.");
        }
    }
}
