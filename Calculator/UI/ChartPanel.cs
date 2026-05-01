using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Calculator.VectorAlgebra;

namespace MatrixCalculator.UI
{
    public class ChartPanel : Panel
    {
        private readonly List<Action<Graphics, Rectangle>> _drawActions = new();

        public ChartPanel()
        {
            BackColor = AppTheme.Surface;
            DoubleBuffered = true;
        }

        public void DrawVector(Vector3D v, Color color)
        {
            _drawActions.Add((g, bounds) =>
            {
                var center = new PointF(bounds.Width / 2f, bounds.Height / 2f);
                float scale = GetScale(bounds);
                var end = new PointF(center.X + (float)(v.X * scale), center.Y - (float)(v.Y * scale));

                using var pen = new Pen(color, 2f) { EndCap = LineCap.ArrowAnchor };
                g.DrawLine(pen, center, end);

                using var dot = new SolidBrush(color);
                g.FillEllipse(dot, end.X - 4, end.Y - 4, 8, 8);
            });
            Invalidate();
        }

        public void DrawLine(Vector3D a, Vector3D b, Color color)
        {
            _drawActions.Add((g, bounds) =>
            {
                var center = new PointF(bounds.Width / 2f, bounds.Height / 2f);
                float scale = GetScale(bounds);
                var p1 = new PointF(center.X + (float)(a.X * scale), center.Y - (float)(a.Y * scale));
                var p2 = new PointF(center.X + (float)(b.X * scale), center.Y - (float)(b.Y * scale));
                using var pen = new Pen(color, 1.8f);
                g.DrawLine(pen, p1, p2);
            });
            Invalidate();
        }

        public void Clear()
        {
            _drawActions.Clear();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var bounds = ClientRectangle;

            // Background
            g.Clear(AppTheme.Surface);

            // Grid lines
            DrawGrid(g, bounds);

            // Axes
            DrawAxes(g, bounds);

            // User drawings
            foreach (var action in _drawActions)
                action(g, bounds);
        }

        private void DrawGrid(Graphics g, Rectangle bounds)
        {
            float scale = GetScale(bounds);
            var cx = bounds.Width / 2f;
            var cy = bounds.Height / 2f;

            using var gridPen = new Pen(Color.FromArgb(28, 35, 48), 1);
            for (float x = cx % scale; x < bounds.Width; x += scale)
                g.DrawLine(gridPen, x, 0, x, bounds.Height);
            for (float y = cy % scale; y < bounds.Height; y += scale)
                g.DrawLine(gridPen, 0, y, bounds.Width, y);
        }

        private void DrawAxes(Graphics g, Rectangle bounds)
        {
            float cx = bounds.Width / 2f;
            float cy = bounds.Height / 2f;
            float scale = GetScale(bounds);

            using var axisPen = new Pen(Color.FromArgb(45, 55, 72), 1.5f);
            g.DrawLine(axisPen, 0, cy, bounds.Width, cy);   // X
            g.DrawLine(axisPen, cx, 0, cx, bounds.Height);  // Y

            using var labelBrush = new SolidBrush(AppTheme.TextMuted);
            using var labelFont = new Font("Consolas", 7.5F);
            g.DrawString("X", labelFont, labelBrush, bounds.Width - 14, cy + 4);
            g.DrawString("Y", labelFont, labelBrush, cx + 4, 4);
            g.DrawString("0", labelFont, labelBrush, cx + 3, cy + 3);

            // Tick marks
            using var tickPen = new Pen(Color.FromArgb(50, 60, 80), 1);
            for (int i = 1; i * scale < Math.Max(bounds.Width, bounds.Height); i++)
            {
                g.DrawLine(tickPen, cx + i * scale, cy - 3, cx + i * scale, cy + 3);
                g.DrawLine(tickPen, cx - i * scale, cy - 3, cx - i * scale, cy + 3);
                g.DrawLine(tickPen, cx - 3, cy + i * scale, cx + 3, cy + i * scale);
                g.DrawLine(tickPen, cx - 3, cy - i * scale, cx + 3, cy - i * scale);
            }
        }

        private float GetScale(Rectangle bounds) => Math.Min(bounds.Width, bounds.Height) / 12f;
    }
}