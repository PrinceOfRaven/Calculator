using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MatrixCalculator.UI
{
    public enum ButtonVariant { Primary, Secondary, Danger }

    /// <summary>Custom button with smooth hover animation and accent styling.</summary>
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
                ButtonVariant.Primary => AppTheme.Background,
                ButtonVariant.Secondary => AppTheme.Accent,
                ButtonVariant.Danger => AppTheme.Danger,
                _ => AppTheme.TextPrimary
            };
            // Fixed: use Surface instead of Transparent to prevent text overlap
            BackColor = AppTheme.Surface;
            SetStyle(ControlStyles.Opaque, true);

            _hoverTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _hoverTimer.Tick += (s, e) => Invalidate();
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

            // 1. ИСПРАВЛЕНИЕ: Сначала полностью заливаем фон цветом кнопки
            using (var bgBrush = new SolidBrush(BackColor))
            {
                g.FillRectangle(bgBrush, ClientRectangle);
            }

            // 2. Рисуем стили в зависимости от варианта кнопки
            if (_variant == ButtonVariant.Primary)
            {
                var baseColor = _hoverAlpha > 0 ? AppTheme.AccentSoft : AppTheme.Accent;
                using var fill = new SolidBrush(baseColor);
                g.FillRectangle(fill, rect);

                // Тонкий светлый блик сверху
                using var highlight = new LinearGradientBrush(
                    new Rectangle(0, 0, Width, Height / 2),
                    Color.FromArgb(40, Color.White), Color.Transparent,
                    LinearGradientMode.Vertical);
                g.FillRectangle(highlight, new Rectangle(0, 0, Width, Height / 2));
            }
            else if (_variant == ButtonVariant.Secondary)
            {
                using var border = new Pen(AppTheme.Accent, 1.5f);
                g.DrawRectangle(border, rect);

                if (_hoverAlpha > 0)
                {
                    using var fill = new SolidBrush(Color.FromArgb(28, AppTheme.Accent));
                    g.FillRectangle(fill, rect);
                }
            }
            else // Danger
            {
                using var border = new Pen(AppTheme.Danger, 1.5f);
                g.DrawRectangle(border, rect);

                if (_hoverAlpha > 0)
                {
                    using var fill = new SolidBrush(Color.FromArgb(35, AppTheme.Danger));
                    g.FillRectangle(fill, rect);
                }
            }

            // 3. Рисуем текст поверх всего
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

    /// <summary>Card panel with dark styling, optional title, and left accent stripe.</summary>
    public class CardPanel : Panel
    {
        private readonly string? _title;

        public CardPanel(string? title = null)
        {
            _title = title;
            BackColor = AppTheme.Surface;
            int topPad = string.IsNullOrEmpty(title) ? 14 : 40;
            Padding = new Padding(14, topPad, 14, 14);
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
            g.FillRectangle(accentBrush, 0, 0, 3, Height);

            if (!string.IsNullOrEmpty(_title))
            {
                // Title bar background
                using var headerBrush = new SolidBrush(AppTheme.SurfaceRaised);
                g.FillRectangle(headerBrush, 3, 0, Width - 3, 28);

                // Title text — readable size
                using var titleBrush = new SolidBrush(AppTheme.TextSecondary);
                using var titleFont = new Font("Segoe UI", 8.5F, FontStyle.Bold);
                g.DrawString(_title.ToUpper(), titleFont, titleBrush, new PointF(14, 7));
            }
        }
    }

    /// <summary>Horizontal separator with optional label.</summary>
    public class SectionDivider : Control
    {
        private readonly string _label;

        public SectionDivider(string label = "")
        {
            _label = label;
            Height = 24;
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            int midY = Height / 2;

            int lineStart = 0;
            if (!string.IsNullOrEmpty(_label))
            {
                using var lf = new Font("Segoe UI", 8.5F, FontStyle.Bold);
                var sz = g.MeasureString(_label.ToUpper(), lf);
                using var lb = new SolidBrush(AppTheme.TextMuted);
                g.DrawString(_label.ToUpper(), lf, lb, 0, (Height - sz.Height) / 2f);
                lineStart = (int)sz.Width + 10;
            }

            using var pen = new Pen(AppTheme.Border, 1);
            g.DrawLine(pen, lineStart, midY, Width, midY);
        }
    }
}