using System;
using System.Drawing;
using System.Windows.Forms;
using BOOSEInterpreter.Core.Command;
using BOOSE;

namespace BOOSEInterpreter.Canvas
{
    /// <summary>
    /// An <see cref="ICanvas"/> implementation that renders drawing operations onto a
    /// <see cref="PictureBox"/> control by maintaining an internal <see cref="Bitmap"/> and
    /// associated <see cref="Graphics"/> instance.
    /// </summary>
    /// <remarks>
    /// The <see cref="PanelCanvas"/> class provides simple drawing primitives (lines, circles,
    /// rectangles, triangles and text) and manages an in-memory bitmap which is assigned to the
    /// provided <see cref="PictureBox"/> via its <see cref="PictureBox.Image"/> property.
    /// Methods are not thread-safe and should be called from the UI thread that owns the
    /// <see cref="PictureBox"/>. The class expects color values to be provided as a
    /// <see cref="System.Drawing.Color"/> stored in the <see cref="PenColour"/> property.
    /// </remarks>
    public class PanelCanvas : ICanvas
    {
        /// <summary>
        /// The backing <see cref="Bitmap"/> instance that represents the pixel buffer used for
        /// all drawing operations. This bitmap is assigned to the host <see cref="PictureBox"/>
        /// so that the UI displays the current drawing state.
        /// </summary>
        private Bitmap bitmap;

        /// <summary>
        /// The <see cref="Graphics"/> object created from <see cref="bitmap"/>. All drawing
        /// primitives (lines, shapes, text) are executed through this object. It is recreated
        /// whenever the internal bitmap is replaced via <see cref="Set(int,int)"/>.
        /// </summary>
        private Graphics graphics;

        /// <summary>
        /// The current X coordinate of the logical pen. Several drawing operations (for
        /// example <see cref="DrawTo(int,int)"/>, <see cref="WriteText(string)"/>) use this
        /// position as their origin or start point. Coordinates are expressed in pixels relative
        /// to the top-left corner of the canvas.
        /// </summary>
        public int Xpos { get; set; }

        /// <summary>
        /// The current Y coordinate of the logical pen. See <see cref="Xpos"/> for additional
        /// details about the coordinate system and usage.
        /// </summary>
        public int Ypos { get; set; }

        /// <summary>
        /// The colour used for subsequent drawing operations. The property type is <see cref="object"/>
        /// for compatibility with the surrounding interpreter infrastructure, but callers should
        /// assign and expect a <see cref="Color"/> value. The default value is
        /// <see cref="Color.Black"/>.
        /// </summary>
        /// <remarks>
        /// When used, callers typically cast this value to <see cref="Color"/>. Setting the
        /// property to an incompatible type will result in an <see cref="InvalidCastException"/>
        /// at the point of drawing.
        /// </remarks>
        public object PenColour { get; set; } = Color.Black;

        /// <summary>
        /// The <see cref="PictureBox"/> control that is used to present the current
        /// <see cref="Bitmap"/> to the user. The control's <see cref="PictureBox.Image"/> property
        /// is updated whenever the bitmap changes.
        /// </summary>
        private readonly PictureBox output;

        /// <summary>
        /// Creates a new <see cref="PanelCanvas"/> bound to the supplied <see cref="PictureBox"/>.
        /// </summary>
        /// <param name="box">The <see cref="PictureBox"/> that will display the drawing surface.
        /// Must not be <c>null</c>. The caller is responsible for ensuring the control is
        /// created on the UI thread.</param>
        /// <remarks>
        /// If <paramref name="box"/> already has a non-zero size at construction time, an
        /// internal bitmap is created immediately with that size by calling
        /// <see cref="Set(int,int)"/>. Otherwise the surface will be created when the UI
        /// initializes the control size and an explicit call to <see cref="Set(int,int)"/> is
        /// made.
        /// </remarks>
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
        /// Allocates a new internal drawing surface with the specified pixel dimensions and
        /// resets the rendering context.
        /// </summary>
        /// <param name="width">The width of the new bitmap in pixels. Must be greater than zero.</param>
        /// <param name="height">The height of the new bitmap in pixels. Must be greater than zero.</param>
        /// <remarks>
        /// Any existing bitmap/graphics objects are replaced. The new surface is cleared to
        /// white and assigned to the bound <see cref="PictureBox"/> control. Callers should
        /// ensure the provided dimensions match the display control to avoid scaling artifacts.
        /// </remarks>
        public void Set(int width, int height)
        {
            bitmap = new Bitmap(width, height);
            graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            output.Image = bitmap;
        }

        /// <summary>
        /// Updates the logical pen position to the provided coordinates without emitting any
        /// drawing primitives. Subsequent drawing operations will use this position as their
        /// origin or start point.
        /// </summary>
        /// <param name="x">The new X coordinate in pixels.</param>
        /// <param name="y">The new Y coordinate in pixels.</param>
        public void MoveTo(int x, int y)
        {
            Xpos = x;
            Ypos = y;
        }

        /// <summary>
        /// Draws a straight line from the current logical pen position to the specified
        /// destination coordinates. After the call the pen position is updated to the target
        /// point.</summary>
        /// <param name="x">Destination X coordinate in pixels.</param>
        /// <param name="y">Destination Y coordinate in pixels.</param>
        /// <remarks>
        /// The line is drawn using a pen of width 2 and the colour provided by
        /// <see cref="PenColour"/>. The method calls <see cref="Refresh"/> to update the
        /// UI after drawing. If <see cref="PenColour"/> cannot be cast to
        /// <see cref="Color"/>, an exception will be thrown.
        /// </remarks>
        public void DrawTo(int x, int y)
        {
            using Pen p = new Pen((Color)PenColour, 2);
            graphics.DrawLine(p, Xpos, Ypos, x, y);

            Xpos = x;
            Ypos = y;

            Refresh();
        }

        /// <summary>
        /// Draws a circle (ellipse with equal width and height) centered on the current pen
        /// position. The circle may be filled or outlined depending on <paramref name="filled"/>.
        /// </summary>
        /// <param name="radius">The radius of the circle in pixels. Must be non-negative.</param>
        /// <param name="filled">If <c>true</c> the shape is filled using a solid brush, otherwise
        /// only the outline is rendered using a pen.</param>
        /// <remarks>
        /// The drawing operation uses <see cref="PenColour"/> for both outline and fill. Calling
        /// code should ensure the radius is reasonable for the current canvas size.
        /// </remarks>
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
        /// Draws a rectangle whose top-left corner is the current pen position. The rectangle
        /// may be filled or outlined depending on <paramref name="filled"/>.
        /// </summary>
        /// <param name="width">The width of the rectangle in pixels.</param>
        /// <param name="height">The height of the rectangle in pixels.</param>
        /// <param name="filled">If <c>true</c> the interior is filled; otherwise only the
        /// border is drawn.</param>
        /// <remarks>
        /// Width/height values that cause the rectangle to extend outside the bitmap will
        /// be clipped by the underlying <see cref="Graphics"/> implementation.
        /// </remarks>
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
        /// Draws an isosceles triangle where the base's left corner is at the current pen
        /// position and the triangle extends downwards by <paramref name="height"/> pixels.
        /// </summary>
        /// <param name="width">The width of the triangle base in pixels.</param>
        /// <param name="height">The vertical height of the triangle in pixels.</param>
        /// <remarks>
        /// The triangle's vertices are calculated so that the apex is centered horizontally
        /// above the base. The shape is drawn using the current <see cref="PenColour"/> as
        /// the outline colour with a pen width of 2.
        /// </remarks>
        public void Tri(int width, int height)
        {
            Point p1 = new Point(Xpos, Ypos + height);
            Point p2 = new Point(Xpos + width, Ypos + height);
            Point p3 = new Point(Xpos + width / 2, Ypos);

            graphics.DrawPolygon(new Pen((Color)PenColour, 2), new[] { p1, p2, p3 });

            Refresh();
        }

        /// <summary>
        /// Clears the entire drawing surface by filling it with white and requests a UI
        /// refresh so the cleared state becomes visible to the user.
        /// </summary>
        /// <remarks>
        /// This method only clears the internal <see cref="Bitmap"/> and does not change pen
        /// position or colour; to reset those values use <see cref="Reset"/>.
        /// </remarks>
        public void Clear()
        {
            graphics.Clear(Color.White);
            Refresh();
        }

        /// <summary>
        /// Resets the drawing state by setting the logical pen position to (0,0) and the
        /// drawing colour to black. Does not clear the bitmap; use <see cref="Clear"/> to
        /// erase existing content.
        /// </summary>
        public void Reset()
        {
            Xpos = 0;
            Ypos = 0;
            PenColour = Color.Black;
        }

        /// <summary>
        /// Sets the current drawing colour using explicit red, green and blue components.
        /// </summary>
        /// <param name="r">Red channel (0-255).</param>
        /// <param name="g">Green channel (0-255).</param>
        /// <param name="b">Blue channel (0-255).</param>
        /// <remarks>
        /// Values outside the 0–255 range will be clamped by the
        /// <see cref="System.Drawing.Color.FromArgb(int,int,int)"/> method.
        /// </remarks>
        public void SetColour(int r, int g, int b)
        {
            PenColour = Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Renders the provided text string at the current pen coordinates using a default
        /// sans-serif font. After drawing the method triggers a UI refresh.
        /// </summary>
        /// <param name="text">The text to draw. If <c>null</c> the method will throw an
        /// <see cref="ArgumentNullException"/> when attempting to draw; callers should ensure
        /// a valid string is provided.</param>
        /// <remarks>
        /// The method uses an <see cref="Font"/> of Arial, size 14 and draws the string using
        /// a <see cref="SolidBrush"/> created from <see cref="PenColour"/>.
        /// </remarks>
        public void WriteText(string text)
        {
            using Font f = new Font("Arial", 14);
            using Brush b = new SolidBrush((Color)PenColour);
            graphics.DrawString(text, f, b, Xpos, Ypos);

            Refresh();
        }

        /// <summary>
        /// Returns the underlying <see cref="Bitmap"/> instance used as the in-memory drawing
        /// surface. The method signature returns <see cref="object"/> for compatibility with
        /// existing caller expectations in the application code.
        /// </summary>
        /// <returns>The <see cref="Bitmap"/> currently used as the canvas pixel buffer, or
        /// <c>null</c> if <see cref="Set(int,int)"/> has not been called.</returns>
        public object getBitmap() => bitmap;

        /// <summary>
        /// Assigns the current internal bitmap to the bound <see cref="PictureBox.Image"/>
        /// property and forces an immediate repaint of the control so that recent drawing
        /// operations become visible.
        /// </summary>
        /// <remarks>
        /// This helper method is private because it manipulates UI elements; it should be
        /// invoked from the UI thread. The method performs a direct assignment to the
        /// <see cref="PictureBox.Image"/> property which may replace any previously set image
        /// reference.
        /// </remarks>
        private void Refresh()
        {
            output.Image = bitmap;
            output.Refresh();
        }
    }
}
