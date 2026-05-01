using System.Drawing;
using System.Windows.Forms;

namespace MatrixCalculator.UI
{
    /// <summary>
    /// Centralized design system — dark theme with high-contrast readable text and teal accent.
    /// </summary>
    public static class AppTheme
    {
        // ── Colors ─────────────────────────────────────────────────────────────
        public static readonly Color Background = Color.FromArgb(11, 13, 18);
        public static readonly Color Surface = Color.FromArgb(18, 22, 30);
        public static readonly Color SurfaceRaised = Color.FromArgb(26, 31, 43);
        public static readonly Color SurfaceHover = Color.FromArgb(32, 38, 52);
        public static readonly Color Border = Color.FromArgb(50, 60, 82);
        public static readonly Color BorderAccent = Color.FromArgb(0, 180, 155);
        public static readonly Color Accent = Color.FromArgb(0, 220, 190);
        public static readonly Color AccentDim = Color.FromArgb(0, 160, 138);
        public static readonly Color AccentSoft = Color.FromArgb(0, 255, 220);
        public static readonly Color Danger = Color.FromArgb(255, 90, 110);
        public static readonly Color Warning = Color.FromArgb(255, 195, 60);
        public static readonly Color Success = Color.FromArgb(80, 230, 150);

        // Higher contrast text for readability
        public static readonly Color TextPrimary = Color.FromArgb(240, 244, 255);
        public static readonly Color TextSecondary = Color.FromArgb(185, 195, 215);
        public static readonly Color TextMuted = Color.FromArgb(120, 135, 160);
        public static readonly Color TextDim = Color.FromArgb(70, 82, 105);

        public static readonly Color TabActive = Color.FromArgb(22, 28, 40);
        public static readonly Color InputBg = Color.FromArgb(14, 18, 26);
        public static readonly Color InputBorder = Color.FromArgb(42, 52, 70);

        // ── Fonts — larger and more readable ──────────────────────────────────
        public static readonly Font TitleFont = new Font("Consolas", 15F, FontStyle.Bold);
        public static readonly Font TabFont = new Font("Consolas", 10F, FontStyle.Regular);
        public static readonly Font BodyFont = new Font("Segoe UI", 10F);
        public static readonly Font LabelFont = new Font("Segoe UI", 9.5F, FontStyle.Regular);
        public static readonly Font LabelBold = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        public static readonly Font MonoFont = new Font("Consolas", 10F);
        public static readonly Font ResultFont = new Font("Consolas", 10F);
        public static readonly Font ButtonFont = new Font("Segoe UI", 10F, FontStyle.Bold);
        public static readonly Font SectionFont = new Font("Segoe UI", 9F, FontStyle.Bold);
        public static readonly Font SmallMono = new Font("Consolas", 9F);

        // ── Spacing constants ──────────────────────────────────────────────────
        public const int PadLeft = 28;
        public const int RowHeight = 32;   // grid row height
        public const int ColWidth = 72;   // grid column width
        public const int InputH = 30;   // standard input height

        // ── Helpers ────────────────────────────────────────────────────────────

        public static Label MakeSectionLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = SectionFont,
                ForeColor = TextSecondary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new System.Drawing.Point(x, y)
            };
        }

        public static Label MakeFieldLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = LabelFont,
                ForeColor = TextSecondary,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new System.Drawing.Point(x, y)
            };
        }

        public static void StyleGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = InputBg;
            dgv.GridColor = InputBorder;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            dgv.DefaultCellStyle.BackColor = InputBg;
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.Font = MonoFont;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 90, 78);
            dgv.DefaultCellStyle.SelectionForeColor = AccentSoft;
            dgv.DefaultCellStyle.Padding = new Padding(2, 0, 2, 0);

            dgv.ColumnHeadersDefaultCellStyle.BackColor = SurfaceRaised;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Accent;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersHeight = 32;

            dgv.RowHeadersDefaultCellStyle.BackColor = SurfaceRaised;
            dgv.RowHeadersDefaultCellStyle.ForeColor = TextMuted;
            dgv.RowHeadersDefaultCellStyle.Font = SmallMono;
            dgv.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.RowHeadersWidth = 40;

            dgv.EnableHeadersVisualStyles = false;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(17, 21, 30);
            dgv.RowTemplate.Height = RowHeight;
            dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
        }

        public static StyledButton MakePrimaryButton(string text, int x, int y, int w = 240, int h = 44)
        {
            return new StyledButton(ButtonVariant.Primary)
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Size = new Size(w, h)
            };
        }

        public static StyledButton MakeSecondaryButton(string text, int x, int y, int w = 160, int h = 36)
        {
            return new StyledButton(ButtonVariant.Secondary)
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Size = new Size(w, h)
            };
        }

        public static TextBox MakeResultBox(int x, int y, int w, int h)
        {
            return new TextBox
            {
                Location = new System.Drawing.Point(x, y),
                Size = new Size(w, h),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = InputBg,
                ForeColor = Accent,
                Font = ResultFont,
                BorderStyle = BorderStyle.None
            };
        }

        public static CardPanel MakeCard(int x, int y, int w, int h, string? title = null)
        {
            return new CardPanel(title)
            {
                Location = new System.Drawing.Point(x, y),
                Size = new Size(w, h)
            };
        }

        public static NumericUpDown MakeSpinner(int x, int y, int val, int min = 1, int max = 10)
        {
            return new NumericUpDown
            {
                Location = new System.Drawing.Point(x, y),
                Width = 62,
                Height = InputH,
                Minimum = min,
                Maximum = max,
                Value = val,
                BackColor = InputBg,
                ForeColor = TextPrimary,
                Font = MonoFont,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center
            };
        }
    }
}