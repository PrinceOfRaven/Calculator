using System;
using System.Drawing;
using System.Windows.Forms;
using Calculator.VectorAlgebra;

namespace MatrixCalculator.UI
{
    public class ChartPanel : Panel
    {
        private List<Action<Graphics>> _drawActions = new();

        public void DrawVector(Vector3D v, Color color)
        {
            _drawActions.Add(g =>
            {
                var center = new Point(Width / 2, Height / 2);
                // Простая проекция
                var end = new Point(center.X + (int)(v.X * 20), center.Y - (int)(v.Y * 20));
                g.DrawLine(new Pen(color, 2), center, end);
                g.FillEllipse(new SolidBrush(color), end.X - 3, end.Y - 3, 6, 6);
            });
            Invalidate();
        }

        public void DrawLine(Vector3D a, Vector3D b, Color color)
        {
            _drawActions.Add(g =>
            {
                var center = new Point(Width / 2, Height / 2);
                var p1 = new Point(center.X + (int)(a.X * 20), center.Y - (int)(a.Y * 20));
                var p2 = new Point(center.X + (int)(b.X * 20), center.Y - (int)(b.Y * 20));
                g.DrawLine(new Pen(color, 2), p1, p2);
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
            e.Graphics.Clear(Color.White);
            foreach (var action in _drawActions) action(e.Graphics);
        }
    }
}
