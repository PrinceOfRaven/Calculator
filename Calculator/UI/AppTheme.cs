using System.Drawing;
using System.Windows.Forms;

namespace MatrixCalculator.UI
{
    /// <summary>
    /// Centralized design system — dark industrial theme with cyan accent.
    /// All UI components reference these values for consistency.
    /// </summary>
    public static class AppTheme
    {
        // ── Colors ─────────────────────────────────────────────────────────────
        public static readonly Color Background  = Color.FromArgb(14, 16, 21);
        public static readonly Color Surface      = Color.FromArgb(22, 26, 34);
        public static readonly Color SurfaceRaised = Color.FromArgb(30, 35, 46);
        public static readonly Color Border       = Color.FromArgb(45, 52, 68);
        public static readonly Color Accent       = Color.FromArgb(0, 212, 180);     // teal-cyan
        public static readonly Color AccentDim    = Color.FromArgb(0, 140, 120);
        public static readonly Color AccentGlow   = Color.FromArgb(30, 0, 212, 180);
        public static readonly Color Danger       = Color.FromArgb(255, 85, 100);
        public static readonly Color Warning      = Color.FromArgb(255, 185, 50);
        public static readonly Color Success      = Color.FromArgb(80, 220, 140);

        public static readonly Color TextPrimary  = Color.FromArgb(230, 235, 245);
        public static readonly Color TextSecondary = Color.FromArgb(160, 170, 190);
        public static readonly Color TextMuted    = Color.FromArgb(90, 100, 120);

        public static readonly Color TabActive    = Color.FromArgb(26, 32, 44);
        public static readonly Color InputBg      = Color.FromArgb(18, 22, 30);

        // ── Fonts ──────────────────────────────────────────────────────────────
        public static readonly Font TitleFont  = new Font("Consolas", 13F, FontStyle.Bold);
        public static readonly Font TabFont    = new Font("Consolas", 9.5F, FontStyle.Regular);
        public static readonly Font BodyFont   = new Font("Segoe UI", 9.5F);
        public static readonly Font LabelFont  = new Font("Consolas", 8.5F);
        public static readonly Font MonoFont   = new Font("Consolas", 9.5F);
        public static readonly Font ResultFont = new Font("Consolas", 9.5F);
        public static readonly Font ButtonFont = new Font("Consolas", 9.5F, FontStyle.Bold);
        public static readonly Font SectionFont = new Font("Consolas", 8F, FontStyle.Bold);

        // ── Helpers ────────────────────────────────────────────────────────────

        /// <summary>Creates a styled section header label.</summary>
        public static Label MakeSectionLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text.ToUpper(),
                Font = SectionFont,
                ForeColor = TextMuted,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new System.Drawing.Point(x, y)
            };
        }

        /// <summary>Applies the dark theme to a DataGridView.</summary>
        public static void StyleGrid(DataGridView dgv)
        {
            dgv.BackgroundColor = InputBg;
            dgv.GridColor = Border;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            dgv.DefaultCellStyle.BackColor = InputBg;
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.Font = MonoFont;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 80, 70);
            dgv.DefaultCellStyle.SelectionForeColor = Accent;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Surface;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Accent;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Consolas", 8.5F, FontStyle.Bold);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgv.ColumnHeadersHeight = 28;

            dgv.RowHeadersDefaultCellStyle.BackColor = SurfaceRaised;
            dgv.RowHeadersDefaultCellStyle.ForeColor = TextMuted;
            dgv.RowHeadersDefaultCellStyle.Font = new Font("Consolas", 8F);
            dgv.RowHeadersWidth = 36;

            dgv.EnableHeadersVisualStyles = false;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(20, 24, 32);
            dgv.RowTemplate.Height = 26;
        }

        /// <summary>Creates a styled primary action button.</summary>
        public static StyledButton MakePrimaryButton(string text, int x, int y, int w = 220, int h = 40)
        {
            return new StyledButton(ButtonVariant.Primary)
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Size = new Size(w, h)
            };
        }

        /// <summary>Creates a styled secondary button.</summary>
        public static StyledButton MakeSecondaryButton(string text, int x, int y, int w = 160, int h = 36)
        {
            return new StyledButton(ButtonVariant.Secondary)
            {
                Text = text,
                Location = new System.Drawing.Point(x, y),
                Size = new Size(w, h)
            };
        }

        /// <summary>Creates a styled multiline result TextBox.</summary>
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

        /// <summary>Creates a card-like panel with border.</summary>
        public static CardPanel MakeCard(int x, int y, int w, int h, string? title = null)
        {
            return new CardPanel(title)
            {
                Location = new System.Drawing.Point(x, y),
                Size = new Size(w, h)
            };
        }
    }
}
