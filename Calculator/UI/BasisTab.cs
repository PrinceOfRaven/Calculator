using System;
using System.Drawing;
using System.Windows.Forms;
using Calculator.Core;
using Calculator.VectorAlgebra;

namespace MatrixCalculator.UI
{
    public class BasisTab : TabPage
    {
        private DataGridView _dgvE;
        private DataGridView _dgvF;
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
            // ── Header ─────────────────────────────────────────────────────────
            var lblTitle = AppTheme.MakeSectionLabel("Преобразование координат между базисами", AppTheme.PadLeft, 22);
            lblTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTitle.ForeColor = AppTheme.Accent;

            var lblDesc = new Label
            {
                Text = "Столбцы матриц — базисные векторы. Вычисляется матрица перехода P и новые координаты вектора.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = AppTheme.TextMuted,
                AutoSize = true,
                Location = new Point(AppTheme.PadLeft, 48)
            };

            // ── Basis grids ────────────────────────────────────────────────────
            var lblE = AppTheme.MakeSectionLabel("Базис  E  (старый)", AppTheme.PadLeft, 82);
            var lblF = AppTheme.MakeSectionLabel("Базис  F  (новый)", 480, 82);

            _dgvE = CreateBasisGrid(AppTheme.PadLeft, 106, "E");
            _dgvF = CreateBasisGrid(480, 106, "F");

            // Arrow indicator
            var arrow = new Label
            {
                Text = "→",
                Font = new Font("Consolas", 32F, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, AppTheme.Accent),
                AutoSize = true,
                Location = new Point(400, 180)
            };

            // ── Vector input ───────────────────────────────────────────────────
            var lblVec = AppTheme.MakeSectionLabel("Вектор в базисе E", AppTheme.PadLeft, 332);

            var vecCard = AppTheme.MakeCard(AppTheme.PadLeft, 356, 460, 90);

            var lblX = new Label
            {
                Text = "X",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = AppTheme.Accent,
                AutoSize = true,
                Location = new Point(20, 48)
            };
            _txtVecX = MakeInput(50, 44, "1");

            var lblY = new Label
            {
                Text = "Y",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = AppTheme.Accent,
                AutoSize = true,
                Location = new Point(170, 48)
            };
            _txtVecY = MakeInput(200, 44, "0");

            var lblZ = new Label
            {
                Text = "Z",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = AppTheme.Accent,
                AutoSize = true,
                Location = new Point(320, 48)
            };
            _txtVecZ = MakeInput(350, 44, "0");

            vecCard.Controls.AddRange(new Control[] { lblX, _txtVecX, lblY, _txtVecY, lblZ, _txtVecZ });

            _btnCalc = AppTheme.MakePrimaryButton("▶  НАЙТИ МАТРИЦУ ПЕРЕХОДА", AppTheme.PadLeft, 462, 460, 44);
            _btnCalc.Click += BtnCalc_Click;

            // ── Result ─────────────────────────────────────────────────────────
            var resultCard = AppTheme.MakeCard(AppTheme.PadLeft, 522, 860, 360, "Результат");
            _txtResult = AppTheme.MakeResultBox(14, 38, 828, 308);
            resultCard.Controls.Add(_txtResult);

            Controls.AddRange(new Control[] {
                lblTitle, lblDesc, lblE, lblF, _dgvE, arrow, _dgvF,
                lblVec, vecCard, _btnCalc, resultCard
            });
        }

        private DataGridView CreateBasisGrid(int x, int y, string baseName)
        {
            var dgv = new DataGridView
            {
                Location = new Point(x, y),
                AllowUserToAddRows = false,
                RowHeadersVisible = true,
                ColumnHeadersVisible = true,
                ScrollBars = ScrollBars.None
            };

            for (int i = 0; i < 3; i++)
                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = $"{baseName.ToLower()}{i + 1}",
                    Width = AppTheme.ColWidth,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

            for (int i = 0; i < 3; i++)
            {
                dgv.Rows.Add();
                dgv.Rows[i].HeaderCell.Value = new[] { "X", "Y", "Z" }[i];
            }

            dgv.Width = 3 * AppTheme.ColWidth + dgv.RowHeadersWidth + 4;
            dgv.Height = 3 * AppTheme.RowHeight + dgv.ColumnHeadersHeight + 4;
            AppTheme.StyleGrid(dgv);

            // Set identity matrix as default
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    dgv.Rows[i].Cells[j].Value = (i == j) ? "1" : "0";

            return dgv;
        }

        private TextBox MakeInput(int x, int y, string val)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Width = 100,
                Height = AppTheme.InputH,
                Text = val,
                BackColor = AppTheme.InputBg,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.MonoFont,
                BorderStyle = BorderStyle.FixedSingle,
                TextAlign = HorizontalAlignment.Center
            };
        }

        private void BtnCalc_Click(object? sender, EventArgs e)
        {
            try
            {
                var E = GetMatrix(_dgvE);
                var F = GetMatrix(_dgvF);

                // Compute transition matrix P from E to F
                Matrix P;
                if (Math.Abs(E.Determinant()) < 1e-9)
                    throw new Exception("Базис E вырожден (определитель = 0)");

                P = E.Inverse() * F;

                // Get vector coordinates in basis E
                double vx = double.Parse(_txtVecX.Text);
                double vy = double.Parse(_txtVecY.Text);
                double vz = double.Parse(_txtVecZ.Text);
                var oldCoords = new double[] { vx, vy, vz };

                // Transform to basis F using P^(-1)
                var P_inv = P.Inverse();
                var newCoords = new double[3];
                for (int i = 0; i < 3; i++)
                    for (int j = 0; j < 3; j++)
                        newCoords[i] += P_inv[i, j] * oldCoords[j];

                // Format output
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("Матрица перехода  P  (от базиса E к базису F):");
                sb.AppendLine(new string('─', 50));
                sb.AppendLine();
                for (int i = 0; i < P.Rows; i++)
                {
                    sb.Append("│");
                    for (int j = 0; j < P.Cols; j++)
                        sb.Append($"  {P[i, j],10:F6}");
                    sb.AppendLine("  │");
                }
                sb.AppendLine();
                sb.AppendLine($"Определитель:  det(P) = {P.Determinant():F8}");
                sb.AppendLine();
                sb.AppendLine(new string('─', 50));
                sb.AppendLine();
                sb.AppendLine($"Координаты вектора  v = ({vx:F4}, {vy:F4}, {vz:F4})  в базисе E");
                sb.AppendLine();
                sb.AppendLine("Координаты в новом базисе F:");
                sb.AppendLine();
                sb.AppendLine($"  x'  =  {newCoords[0]:F8}");
                sb.AppendLine($"  y'  =  {newCoords[1]:F8}");
                sb.AppendLine($"  z'  =  {newCoords[2]:F8}");

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