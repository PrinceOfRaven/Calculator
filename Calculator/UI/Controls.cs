using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MatrixCalculator.UI
{
    public enum ButtonVariant { Primary, Secondary, Danger }

    /// <summary>Custom button with hover animation and accent styling.</summary>
    public class StyledButton : Button
    {
        private readonly ButtonVariant _variant;
        private float _hoverAlpha = 0f;
        private System.Windows.Forms.Timer? _hoverTimer;

        public StyledButton(ButtonVariant variant = ButtonVariant.Primary)
        {
            _variant = variant;
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Font = AppTheme.ButtonFont;
            ForeColor = _variant switch
            {
                ButtonVariant.Primary   => AppTheme.Background,
                ButtonVariant.Secondary => AppTheme.Accent,
                ButtonVariant.Danger    => AppTheme.Danger,
                _                       => AppTheme.TextPrimary
            };
            BackColor = Color.Transparent;

            _hoverTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _hoverTimer.Tick += HoverTimer_Tick;
        }

        private void HoverTimer_Tick(object? sender, EventArgs e)
        {
            Invalidate();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hoverAlpha = 1f;
            _hoverTimer?.Start();
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoverAlpha = 0f;
            _hoverTimer?.Stop();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);

            if (_variant == ButtonVariant.Primary)
            {
                // Base fill
                var baseColor = _hoverAlpha > 0 ? AppTheme.Accent : AppTheme.AccentDim;
                using var fill = new SolidBrush(baseColor);
                g.FillRectangle(fill, rect);

                // Highlight overlay
                if (_hoverAlpha > 0)
                {
                    using var glow = new SolidBrush(Color.FromArgb(30, Color.White));
                    g.FillRectangle(glow, rect);
                }
            }
            else if (_variant == ButtonVariant.Secondary)
            {
                using var border = new Pen(AppTheme.Accent, 1.5f);
                g.DrawRectangle(border, rect);

                if (_hoverAlpha > 0)
                {
                    using var fill = new SolidBrush(Color.FromArgb(25, AppTheme.Accent));
                    g.FillRectangle(fill, rect);
                }
            }
            else
            {
                using var border = new Pen(AppTheme.Danger, 1.5f);
                g.DrawRectangle(border, rect);
                if (_hoverAlpha > 0)
                {
                    using var fill = new SolidBrush(Color.FromArgb(30, AppTheme.Danger));
                    g.FillRectangle(fill, rect);
                }
            }

            // Text
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            using var textBrush = new SolidBrush(ForeColor);
            g.DrawString(Text, Font, textBrush, ClientRectangle, sf);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _hoverTimer?.Dispose();
            base.Dispose(disposing);
        }
    }

    /// <summary>Panel with a dark card look and optional title header.</summary>
    public class CardPanel : Panel
    {
        private readonly string? _title;

        public CardPanel(string? title = null)
        {
            _title = title;
            BackColor = AppTheme.Surface;
            Padding = new Padding(string.IsNullOrEmpty(title) ? 12 : 36, 12, 12, 12);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            // Border
            using var borderPen = new Pen(AppTheme.Border, 1);
            g.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);

            // Left accent stripe
            using var accentBrush = new SolidBrush(AppTheme.Accent);
            g.FillRectangle(accentBrush, 0, 0, 2, Height);

            if (!string.IsNullOrEmpty(_title))
            {
                // Title bar
                using var headerBrush = new SolidBrush(AppTheme.SurfaceRaised);
                g.FillRectangle(headerBrush, 2, 0, Width - 2, 24);

                using var titleBrush = new SolidBrush(AppTheme.TextMuted);
                g.DrawString(_title.ToUpper(), AppTheme.SectionFont, titleBrush, new PointF(10, 6));
            }
        }
    }
}
