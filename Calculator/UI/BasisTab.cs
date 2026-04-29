using System;
using System.Drawing;
using System.Windows.Forms;
using Calculator.Core;
using Calculator.VectorAlgebra;

namespace MatrixCalculator.UI
{
    public class BasisTab : TabPage
    {
        private DataGridView _dgvBasisE;
        private DataGridView _dgvBasisF;
        private Button _btnCalc;
        private TextBox _txtResult;
        private TextBox _txtVecX, _txtVecY, _txtVecZ;

        public BasisTab() : base("Базисы")
        {
            BackColor = AppTheme.Background;
            AutoScroll = true;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Description
            var lblInfo = new Label
            {
                Text = "Матрица перехода между базисами и перевод координат вектора.",
                Font = AppTheme.LabelFont,
                ForeColor = AppTheme.TextMuted,
                AutoSize = true,
                Location = new Point(24, 20)
            };

            // ── Basis grids ────────────────────────────────────────────────────
            Controls.Add(AppTheme.MakeSectionLabel("Базис E  (столбцы — векторы)", 24, 46));
            Controls.Add(AppTheme.MakeSectionLabel("Базис F  (столбцы — векторы)", 410, 46));

            _dgvBasisE = CreateGrid(24, 66);
            _dgvBasisF = CreateGrid(410, 66);

            // Arrow indicator
            var arrow = new Label
            {
                Text = "→",
                Font = new Font("Consolas", 24F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, AppTheme.Accent),
                AutoSize = true,
                Location = new Point(348, 130)
            };

            // ── Vector input ───────────────────────────────────────────────────
            Controls.Add(AppTheme.MakeSectionLabel("Вектор в базисе E", 24, 270));

            var vecCard = AppTheme.MakeCard(24, 290, 360, 80);
            var lblX = new Label { Text = "X", Font = new Font("Consolas", 9F, FontStyle.Bold), ForeColor = AppTheme.Accent, AutoSize = true, Location = new Point(14, 28) };
            _txtVecX = MakeInput(36, 24, "1");
            var lblY = new Label { Text = "Y", Font = new Font("Consolas", 9F, FontStyle.Bold), ForeColor = AppTheme.Accent, AutoSize = true, Location = new Point(120, 28) };
            _txtVecY = MakeInput(142, 24, "0");
            var lblZ = new Label { Text = "Z", Font = new Font("Consolas", 9F, FontStyle.Bold), ForeColor = AppTheme.Accent, AutoSize = true, Location = new Point(226, 28) };
            _txtVecZ = MakeInput(248, 24, "0");
            vecCard.Controls.AddRange(new Control[] { lblX, _txtVecX, lblY, _txtVecY, lblZ, _txtVecZ });

            _btnCalc = AppTheme.MakePrimaryButton("▶  НАЙТИ МАТРИЦУ ПЕРЕХОДА", 24, 384, 360, 42);
            _btnCalc.Click += BtnCalc_Click;

            // ── Result ─────────────────────────────────────────────────────────
            var resultCard = AppTheme.MakeCard(24, 442, 760, 280, "Результат");
            _txtResult = AppTheme.MakeResultBox(12, 30, 734, 240);
            resultCard.Controls.Add(_txtResult);

            Controls.AddRange(new Control[] {
                lblInfo, _dgvBasisE, arrow, _dgvBasisF,
                vecCard, _btnCalc, resultCard
            });
        }

        private DataGridView CreateGrid(int x, int y)
        {
            var dgv = new DataGridView
            {
                Location = new Point(x, y),
                Width = 3 * 62 + 42,
                Height = 3 * 28 + 30,
                AllowUserToAddRows = false,
                RowHeadersVisible = true,
                ScrollBars = ScrollBars.None
            };

            for (int i = 0; i < 3; i++)
                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = $"e{i + 1}",
                    Width = 58,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

            for (int i = 0; i < 3; i++) dgv.Rows.Add();
            AppTheme.StyleGrid(dgv);
            return dgv;
        }

        private TextBox MakeInput(int x, int y, string val)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Width = 72,
                Text = val,
                BackColor = AppTheme.InputBg,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.MonoFont,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void BtnCalc_Click(object? sender, EventArgs e)
        {
            try
            {
                var E = GetMatrix(_dgvBasisE);
                var F = GetMatrix(_dgvBasisF);

                Matrix P;
                if (Math.Abs(E.Determinant() - 1.0) > 1e-9 && Math.Abs(E.Determinant()) > 1e-9)
                    P = E.Inverse() * F;
                else
                    P = F;

                double vx = double.Parse(_txtVecX.Text);
                double vy = double.Parse(_txtVecY.Text);
                double vz = double.Parse(_txtVecZ.Text);
                var old = new double[] { vx, vy, vz };

                var P_inv = P.Inverse();
                var newCoords = new double[3];
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        newCoords[i] += P_inv[i, j] * old[j];

                var sb = new System.Text.StringBuilder();
                sb.AppendLine("Матрица перехода P  (от E к F):\n");
                for (int i = 0; i < P.Rows; i++)
                {
                    sb.Append("│");
                    for (int j = 0; j < P.Cols; j++)
                        sb.Append($"  {P[i, j],9:F4}");
                    sb.AppendLine("  │");
                }
                sb.AppendLine($"\nКоординаты вектора в новом базисе F:\n");
                sb.AppendLine($"  x'  =  {newCoords[0]:F6}");
                sb.AppendLine($"  y'  =  {newCoords[1]:F6}");
                sb.AppendLine($"  z'  =  {newCoords[2]:F6}");

                _txtResult.ForeColor = AppTheme.Accent;
                _txtResult.Text = sb.ToString();
            }
            catch (Exception ex)
            {
                _txtResult.ForeColor = AppTheme.Danger;
                _txtResult.Text = "⚠  " + ex.Message;
            }
        }

        private Matrix GetMatrix(DataGridView dgv)
        {
            int r = dgv.Rows.Count, c = dgv.Columns.Count;
            var m = new Matrix(r, c);
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    m[i, j] = double.Parse(dgv.Rows[i].Cells[j].Value?.ToString() ?? "0");
            return m;
        }
    }
}
