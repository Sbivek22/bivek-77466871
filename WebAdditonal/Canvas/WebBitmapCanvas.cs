using System.Drawing;
using System.Drawing.Imaging;
using BOOSE;

namespace WebAdditonal.Canvas
{
    public sealed class WebBitmapCanvas : ICanvas, IDisposable
    {
        private Bitmap? bitmap;
        private Graphics? graphics;

        public int Xpos { get; set; }
        public int Ypos { get; set; }
        public object PenColour { get; set; } = Color.Black;

        public void Set(int width, int height)
        {
            graphics?.Dispose();
            bitmap?.Dispose();

            bitmap = new Bitmap(width, height);
            graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
        }

        public void MoveTo(int x, int y) { Xpos = x; Ypos = y; }

        public void DrawTo(int x, int y)
        {
            EnsureReady();
            using Pen p = new Pen((Color)PenColour, 2);
            graphics!.DrawLine(p, Xpos, Ypos, x, y);
            Xpos = x; Ypos = y;
        }

        public void Circle(int radius, bool filled)
        {
            EnsureReady();
            Rectangle rect = new Rectangle(Xpos - radius, Ypos - radius, radius * 2, radius * 2);
            if (filled)
                graphics!.FillEllipse(new SolidBrush((Color)PenColour), rect);
            else
                graphics!.DrawEllipse(new Pen((Color)PenColour, 2), rect);
        }

        public void Rect(int width, int height, bool filled)
        {
            EnsureReady();
            Rectangle rect = new Rectangle(Xpos, Ypos, width, height);
            if (filled)
                graphics!.FillRectangle(new SolidBrush((Color)PenColour), rect);
            else
                graphics!.DrawRectangle(new Pen((Color)PenColour, 2), rect);
        }

        public void Tri(int width, int height)
        {
            EnsureReady();
            Point p1 = new Point(Xpos, Ypos + height);
            Point p2 = new Point(Xpos + width, Ypos + height);
            Point p3 = new Point(Xpos + width / 2, Ypos);
            graphics!.DrawPolygon(new Pen((Color)PenColour, 2), new[] { p1, p2, p3 });
        }

        public void Clear()
        {
            EnsureReady();
            graphics!.Clear(Color.White);
        }

        public void Reset()
        {
            Xpos = 0;
            Ypos = 0;
            PenColour = Color.Black;
        }

        public void SetColour(int r, int g, int b)
        {
            PenColour = Color.FromArgb(r, g, b);
        }

        public void WriteText(string text)
        {
            EnsureReady();
            using Font f = new Font("Arial", 14);
            using Brush b = new SolidBrush((Color)PenColour);
            graphics!.DrawString(text, f, b, Xpos, Ypos);
        }

        public object getBitmap() => bitmap ?? throw new InvalidOperationException("Call Set(width,height) first.");

        public byte[] ExportPng()
        {
            EnsureReady();
            using var ms = new MemoryStream();
            bitmap!.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        private void EnsureReady()
        {
            if (bitmap == null || graphics == null)
                throw new InvalidOperationException("Canvas not initialised. Call Set(width,height).");
        }

        public void Dispose()
        {
            graphics?.Dispose();
            bitmap?.Dispose();
        }
    }
}
