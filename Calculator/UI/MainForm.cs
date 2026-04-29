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
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AppTheme.Background;
            this.ForeColor = AppTheme.TextPrimary;
            this.Font = AppTheme.BodyFont;

            // Header
            _headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = AppTheme.Surface,
                Padding = new Padding(24, 0, 24, 0)
            };
            _headerPanel.Paint += HeaderPanel_Paint;

            var titleLabel = new Label
            {
                Text = "MATRIX CALCULATOR",
                Font = AppTheme.TitleFont,
                ForeColor = AppTheme.Accent,
                AutoSize = true,
                Location = new Point(28, 18)
            };

            var subtitleLabel = new Label
            {
                Text = "v2.0  ·  Linear Algebra Suite",
                Font = new Font("Consolas", 9F),
                ForeColor = AppTheme.TextMuted,
                AutoSize = true,
                Location = new Point(30, 40)
            };

            _headerPanel.Controls.Add(titleLabel);
            _headerPanel.Controls.Add(subtitleLabel);

            // TabControl
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = AppTheme.TabFont,
                DrawMode = TabDrawMode.OwnerDrawFixed,
                ItemSize = new Size(160, 42),
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
            // Bottom border accent line
            using var pen = new Pen(AppTheme.Accent, 2);
            g.DrawLine(pen, 0, _headerPanel.Height - 1, _headerPanel.Width, _headerPanel.Height - 1);

            // Decorative right-side geometry
            using var brush = new SolidBrush(Color.FromArgb(20, AppTheme.Accent));
            g.FillRectangle(brush, _headerPanel.Width - 200, 0, 200, _headerPanel.Height);
        }

        private void TabControl_DrawItem(object? sender, DrawItemEventArgs e)
        {
            var tab = tabControl.TabPages[e.Index];
            var bounds = tabControl.GetTabRect(e.Index);
            bool selected = tabControl.SelectedIndex == e.Index;

            using var bgBrush = new SolidBrush(selected ? AppTheme.TabActive : AppTheme.Surface);
            e.Graphics.FillRectangle(bgBrush, bounds);

            if (selected)
            {
                using var accentBrush = new SolidBrush(AppTheme.Accent);
                e.Graphics.FillRectangle(accentBrush, bounds.X, bounds.Bottom - 3, bounds.Width, 3);
            }

            var textColor = selected ? AppTheme.TextPrimary : AppTheme.TextMuted;
            using var textBrush = new SolidBrush(textColor);
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            e.Graphics.DrawString(tab.Text, AppTheme.TabFont, textBrush, bounds, sf);
        }
    }
}
