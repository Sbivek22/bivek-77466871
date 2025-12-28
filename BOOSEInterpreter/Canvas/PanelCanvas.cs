using System;
using System.Drawing;
using System.Windows.Forms;
using BOOSE;

namespace BOOSEInterpreter.Canvas
{
    /// <summary>
    /// An implementation of <see cref="ICanvas"/> that draws onto a <see cref="PictureBox"/> control.
    /// The class manages an internal <see cref="Bitmap"/> and <see cref="Graphics"/> object used
    /// for rendering shapes, text and images.
    /// </summary>
    /// 
    public class PanelCanvas : ICanvas
    {
        /// <summary>
        /// Backing bitmap used as the drawing surface.
        /// </summary>
        private Bitmap bitmap;

        /// <summary>
        /// Graphics object obtained from <see cref="bitmap"/> used to perform drawing operations.
        /// </summary>
        private Graphics graphics;

        /// <summary>
        /// Current X position for drawing operations.
        /// </summary>
        public int Xpos { get; set; }

        /// <summary>
        /// Current Y position for drawing operations.
        /// </summary>
        public int Ypos { get; set; }

        /// <summary>
        /// Current pen/brush colour used for drawing. Stored as <see cref="object"/> to match the
        /// interpreter's canvas interface but is expected to hold a <see cref="Color"/> instance.
        /// Defaults to <see cref="Color.Black"/>.
        /// </summary>
        public object PenColour { get; set; } = Color.Black;

        /// <summary>
        /// The <see cref="PictureBox"/> control that displays the canvas bitmap to the user.
        /// </summary>
        private readonly PictureBox output;

        /// <summary>
        /// Initializes a new instance of the <see cref="PanelCanvas"/> class using the provided
        /// <see cref="PictureBox"/> as the display target. If the control already has a positive
        /// size, the internal bitmap/graphics objects are created immediately.
        /// </summary>
        /// <param name="box">The picture box that will display the canvas.</param>
        public PanelCanvas(PictureBox box)
        {
            output = box;

            // Automatically create graphics immediately if the control size is available.
            if (box.Width > 0 && box.Height > 0)
            {
                Set(box.Width, box.Height);
            }
        }

        /// <summary>
        /// Creates or replaces the internal bitmap and graphics objects with the specified size
        /// and clears the surface to white.
        /// </summary>
        /// <param name="width">Width of the drawing surface in pixels.</param>
        /// <param name="height">Height of the drawing surface in pixels.</param>
        public void Set(int width, int height)
        {
            bitmap = new Bitmap(width, height);
            graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            output.Image = bitmap;
        }

        /// <summary>
        /// Moves the current pen position to the specified coordinates without drawing.
        /// </summary>
        /// <param name="x">New X coordinate.</param>
        /// <param name="y">New Y coordinate.</param>
        public void MoveTo(int x, int y)
        {
            Xpos = x;
            Ypos = y;
        }

        /// <summary>
        /// Draws a line from the current pen position to the specified coordinates and updates
        /// the current position to the target point.
        /// </summary>
        /// <param name="x">Target X coordinate.</param>
        /// <param name="y">Target Y coordinate.</param>
        public void DrawTo(int x, int y)
        {
            using Pen p = new Pen((Color)PenColour, 2);
            graphics.DrawLine(p, Xpos, Ypos, x, y);

            Xpos = x;
            Ypos = y;

            Refresh();
        }

        /// <summary>
        /// Draws a circle centered at the current pen position.
        /// </summary>
        /// <param name="radius">Radius of the circle in pixels.</param>
        /// <param name="filled">If <c>true</c>, the circle is filled; otherwise only the outline is drawn.</param>
        public void Circle(int radius, bool filled)
        {
            Rectangle rect = new Rectangle(Xpos - radius, Ypos - radius, radius * 2, radius * 2);

            if (filled)
                graphics.FillEllipse(new SolidBrush((Color)PenColour), rect);
            else
                graphics.DrawEllipse(new Pen((Color)PenColour, 2), rect);

            Refresh();
        }

        /// <summary>
        /// Draws a rectangle with the top-left corner at the current pen position.
        /// </summary>
        /// <param name="width">Width of the rectangle in pixels.</param>
        /// <param name="height">Height of the rectangle in pixels.</param>
        /// <param name="filled">If <c>true</c>, the rectangle is filled; otherwise only the outline is drawn.</param>
        public void Rect(int width, int height, bool filled)
        {
            Rectangle rect = new Rectangle(Xpos, Ypos, width, height);

            if (filled)
                graphics.FillRectangle(new SolidBrush((Color)PenColour), rect);
            else
                graphics.DrawRectangle(new Pen((Color)PenColour, 2), rect);

            Refresh();
        }

        /// <summary>
        /// Draws an isosceles triangle with the base starting at the current pen position.
        /// The triangle's base spans <paramref name="width"/> pixels and its height is <paramref name="height"/> pixels.
        /// </summary>
        /// <param name="width">Width of the triangle base in pixels.</param>
        /// <param name="height">Height of the triangle in pixels.</param>
        public void Tri(int width, int height)
        {
            Point p1 = new Point(Xpos, Ypos + height);
            Point p2 = new Point(Xpos + width, Ypos + height);
            Point p3 = new Point(Xpos + width / 2, Ypos);

            graphics.DrawPolygon(new Pen((Color)PenColour, 2), new[] { p1, p2, p3 });

            Refresh();
        }

        /// <summary>
        /// Clears the drawing surface to white and refreshes the display.
        /// </summary>
        public void Clear()
        {
            graphics.Clear(Color.White);
            Refresh();
        }

        /// <summary>
        /// Resets the drawing state: pen position to (0,0) and colour to black.
        /// </summary>
        public void Reset()
        {
            Xpos = 0;
            Ypos = 0;
            PenColour = Color.Black;
        }

        /// <summary>
        /// Sets the pen colour using RGB components.
        /// </summary>
        /// <param name="r">Red component (0-255).</param>
        /// <param name="g">Green component (0-255).</param>
        /// <param name="b">Blue component (0-255).</param>
        public void SetColour(int r, int g, int b)
        {
            PenColour = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Writes text at the current pen position using a default font.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        public void WriteText(string text)
        {
            using Font f = new Font("Arial", 14);
            using Brush b = new SolidBrush((Color)PenColour);
            graphics.DrawString(text, f, b, Xpos, Ypos);

            Refresh();
        }

        /// <summary>
        /// Returns the underlying bitmap used by the canvas. The return type is <see cref="object"/>
        /// to match the usage in the application UI code.
        /// </summary>
        /// <returns>The <see cref="Bitmap"/> instance used as the drawing surface.</returns>
        public object getBitmap() => bitmap;

        /// <summary>
        /// Updates the picture box to display the current bitmap and forces a redraw of the control.
        /// </summary>
        private void Refresh()
        {
            output.Image = bitmap;
            output.Refresh();
        }
    }
}
