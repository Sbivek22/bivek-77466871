using System;
using System.Collections.Generic;
using System.Drawing;
using BOOSE;

namespace BOOSEInterpreterTests.TestHelpers
{
    /// <summary>
    /// A fake implementation of <see cref="ICanvas"/> for unit testing.
    /// This canvas does not render to a UI; instead, it records drawing calls
    /// so that tests can assert correct behaviour.
    /// </summary>
    public sealed class RecordingCanvas : ICanvas
    {
        /// <summary>
        /// Backing bitmap required by the <see cref="ICanvas"/> interface.
        /// </summary>
        private Bitmap _bmp = new Bitmap(1, 1);

        /// <summary>
        /// Gets or sets the current X position of the drawing cursor.
        /// </summary>
        public int Xpos { get; set; }

        /// <summary>
        /// Gets or sets the current Y position of the drawing cursor.
        /// </summary>
        public int Ypos { get; set; }

        /// <summary>
        /// Gets or sets the current pen colour.
        /// </summary>
        public object PenColour { get; set; } = Color.Black;

        /// <summary>
        /// Records all calls made to <see cref="MoveTo(int, int)"/>.
        /// </summary>
        public List<(int x, int y)> MoveToCalls { get; } = new();

        /// <summary>
        /// Records all calls made to <see cref="DrawTo(int, int)"/>.
        /// </summary>
        public List<(int x, int y)> DrawToCalls { get; } = new();

        /// <summary>
        /// Records all calls made to <see cref="Circle(int, bool)"/>.
        /// </summary>
        public List<int> CircleCalls { get; } = new();

        /// <summary>
        /// Records all calls made to <see cref="Rect(int, int, bool)"/>.
        /// </summary>
        public List<(int w, int h, bool filled)> RectCalls { get; } = new();

        /// <summary>
        /// Records all calls made to <see cref="Tri(int, int)"/>.
        /// </summary>
        public List<(int w, int h)> TriCalls { get; } = new();

        /// <summary>
        /// Records all calls made to <see cref="SetColour(int, int, int)"/>.
        /// </summary>
        public List<(int r, int g, int b)> ColourCalls { get; } = new();

        /// <summary>
        /// Records all calls made to <see cref="WriteText(string)"/>.
        /// </summary>
        public List<string> WriteCalls { get; } = new();

        /// <summary>
        /// Sets the size of the internal bitmap.
        /// </summary>
        /// <param name="width">Bitmap width.</param>
        /// <param name="height">Bitmap height.</param>
        public void Set(int width, int height)
        {
            if (width <= 0) width = 1;
            if (height <= 0) height = 1;

            _bmp.Dispose();
            _bmp = new Bitmap(width, height);
        }

        /// <summary>
        /// Moves the drawing cursor to the specified position.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        public void MoveTo(int x, int y)
        {
            Xpos = x;
            Ypos = y;
            MoveToCalls.Add((x, y));
        }

        /// <summary>
        /// Draws a line from the current cursor position to the specified position.
        /// </summary>
        /// <param name="x">Destination X coordinate.</param>
        /// <param name="y">Destination Y coordinate.</param>
        public void DrawTo(int x, int y)
        {
            Xpos = x;
            Ypos = y;
            DrawToCalls.Add((x, y));
        }

        /// <summary>
        /// Draws a circle with the specified radius.
        /// </summary>
        /// <param name="radius">Circle radius.</param>
        /// <param name="filled">Whether the circle is filled.</param>
        public void Circle(int radius, bool filled) =>
            CircleCalls.Add(radius);

        /// <summary>
        /// Draws a rectangle with the specified dimensions.
        /// </summary>
        /// <param name="width">Rectangle width.</param>
        /// <param name="height">Rectangle height.</param>
        /// <param name="filled">Whether the rectangle is filled.</param>
        public void Rect(int width, int height, bool filled) =>
            RectCalls.Add((width, height, filled));

        /// <summary>
        /// Draws a triangle with the specified width and height.
        /// </summary>
        /// <param name="width">Triangle width.</param>
        /// <param name="height">Triangle height.</param>
        public void Tri(int width, int height) =>
            TriCalls.Add((width, height));

        /// <summary>
        /// Clears the canvas. No operation for the fake canvas.
        /// </summary>
        public void Clear()
        {
            // No-op
        }

        /// <summary>
        /// Resets the canvas state and clears all recorded calls.
        /// </summary>
        public void Reset()
        {
            Xpos = 0;
            Ypos = 0;
            PenColour = Color.Black;

            MoveToCalls.Clear();
            DrawToCalls.Clear();
            CircleCalls.Clear();
            RectCalls.Clear();
            TriCalls.Clear();
            ColourCalls.Clear();
            WriteCalls.Clear();
        }

        /// <summary>
        /// Sets the current pen colour.
        /// </summary>
        /// <param name="r">Red component.</param>
        /// <param name="g">Green component.</param>
        /// <param name="b">Blue component.</param>
        public void SetColour(int r, int g, int b)
        {
            PenColour = Color.FromArgb(r, g, b);
            ColourCalls.Add((r, g, b));
        }

        /// <summary>
        /// Writes text at the current cursor position.
        /// </summary>
        /// <param name="text">Text to write.</param>
        public void WriteText(string text) =>
            WriteCalls.Add(text ?? string.Empty);

        /// <summary>
        /// Returns the underlying bitmap for compatibility with <see cref="ICanvas"/>.
        /// </summary>
        /// <returns>The internal bitmap.</returns>
        public object getBitmap() => _bmp;
    }
}
