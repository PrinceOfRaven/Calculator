using Calculator.Core;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MatrixCalculator.UI
{
    public class MainForm : Form
    {
        private TabControl tabControl;
        private MatrixTab _matrixTab;
        private SlaeTab _slaeTab;
        private VectorTab _vectorTab;
        private BasisTab _basisTab;
        private Panel _headerPanel;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Matrix Calculator";
            this.Size = new Size(1440, 980);
            this.MinimumSize = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AppTheme.Background;
            this.ForeColor = AppTheme.TextPrimary;
            this.Font = AppTheme.BodyFont;

            // ── Header ─────────────────────────────────────────────────────────
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = AppTheme.Surface,
                Padding = new Padding(32, 0, 32, 0)
            };
            _headerPanel.Paint += HeaderPanel_Paint;

            var titleLabel = new Label
            {
                Text = "MATRIX CALCULATOR",
                Font = AppTheme.TitleFont,
                ForeColor = AppTheme.Accent,
                AutoSize = true,
                Location = new Point(32, 14)
            };

            var subtitleLabel = new Label
            {
                Text = "v2.1  ·  Linear Algebra Suite",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = AppTheme.TextMuted,
                AutoSize = true,
                Location = new Point(34, 42)
            };

            _headerPanel.Controls.Add(titleLabel);
            _headerPanel.Controls.Add(subtitleLabel);

            // ── TabControl ─────────────────────────────────────────────────────
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = AppTheme.TabFont,
                DrawMode = TabDrawMode.OwnerDrawFixed,
                ItemSize = new Size(180, 46),
                SizeMode = TabSizeMode.Fixed,
                Padding = new Point(0, 0),
                Appearance = TabAppearance.FlatButtons
            };
            tabControl.DrawItem += TabControl_DrawItem;
            tabControl.SelectedIndexChanged += (s, e) => tabControl.Invalidate();

            _matrixTab = new MatrixTab();
            _slaeTab = new SlaeTab();
            _vectorTab = new VectorTab();
            _basisTab = new BasisTab();

            tabControl.TabPages.AddRange(new TabPage[] {
                _matrixTab, _slaeTab, _vectorTab, _basisTab
            });

            var mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = AppTheme.Background };
            mainPanel.Controls.Add(tabControl);

            this.Controls.Add(mainPanel);
            this.Controls.Add(_headerPanel);
        }

        private void HeaderPanel_Paint(object? sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            // Bottom accent line
            using var pen = new Pen(AppTheme.Accent, 2);
            g.DrawLine(pen, 0, _headerPanel.Height - 1, _headerPanel.Width, _headerPanel.Height - 1);

            // Decorative right-side glow block
            using var brush = new LinearGradientBrush(
                new Rectangle(_headerPanel.Width - 300, 0, 300, _headerPanel.Height),
                Color.Transparent, Color.FromArgb(14, AppTheme.Accent),
                LinearGradientMode.Horizontal);
            g.FillRectangle(brush, _headerPanel.Width - 300, 0, 300, _headerPanel.Height);

            // Small grid decoration (top-right corner)
            using var dotBrush = new SolidBrush(Color.FromArgb(18, AppTheme.Accent));
            int spacing = 16;
            for (int x = _headerPanel.Width - 180; x < _headerPanel.Width - 20; x += spacing)
                for (int y = 12; y < _headerPanel.Height - 12; y += spacing)
                    g.FillRectangle(dotBrush, x, y, 2, 2);
        }

        private void TabControl_DrawItem(object? sender, DrawItemEventArgs e)
        {
            var tab = tabControl.TabPages[e.Index];
            var bounds = tabControl.GetTabRect(e.Index);
            bool sel = tabControl.SelectedIndex == e.Index;

            // Background
            using var bgBrush = new SolidBrush(sel ? AppTheme.TabActive : AppTheme.Surface);
            e.Graphics.FillRectangle(bgBrush, bounds);

            // Subtle separator between tabs
            if (!sel)
            {
                using var sepPen = new Pen(AppTheme.Border, 1);
                e.Graphics.DrawLine(sepPen, bounds.Right - 1, bounds.Y + 8, bounds.Right - 1, bounds.Bottom - 8);
            }

            // Bottom accent bar on selected tab
            if (sel)
            {
                using var accentBrush = new SolidBrush(AppTheme.Accent);
                e.Graphics.FillRectangle(accentBrush, bounds.X, bounds.Bottom - 3, bounds.Width, 3);

                // Side indicator
                using var leftBrush = new SolidBrush(AppTheme.Accent);
                e.Graphics.FillRectangle(leftBrush, bounds.X, bounds.Y, 2, bounds.Height);
            }

            // Tab text
            var textColor = sel ? AppTheme.TextPrimary : AppTheme.TextMuted;
            using var textBrush = new SolidBrush(textColor);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            var font = sel ? new Font("Segoe UI", 10F, FontStyle.Bold) : AppTheme.TabFont;
            e.Graphics.DrawString(tab.Text, font, textBrush, bounds, sf);
        }
    }
}