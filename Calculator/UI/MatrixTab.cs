using System;
using System.Drawing;
using System.Windows.Forms;
using Calculator.Core;

namespace MatrixCalculator.UI
{
    public class MatrixTab : TabPage
    {
        private NumericUpDown _numRowsA, _numColsA;
        private NumericUpDown _numRowsB, _numColsB;
        private DataGridView _dgvA;
        private DataGridView _dgvB;
        private ComboBox _cbOp;
        private Button _btnCalc;
        private TextBox _txtResult;
        private Panel _resultCard;

        // Layout constants for larger grids
        private const int GridTop = 90;
        private const int GridALeft = AppTheme.PadLeft;
        private const int GridBLeft = 540;
        private const int ControlsY = 320;

        public MatrixTab() : base("Матрицы")
        {
            BackColor = AppTheme.Background;
            AutoScroll = true;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ── Matrix A ───────────────────────────────────────────────────────
            var lblA = AppTheme.MakeSectionLabel("Матрица  A", GridALeft, 24);
            lblA.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblA.ForeColor = AppTheme.Accent;

            var lblRowsA = AppTheme.MakeFieldLabel("Строк:", GridALeft, 56);
            _numRowsA = AppTheme.MakeSpinner(GridALeft + 68, 50, 3);
            var lblColsA = AppTheme.MakeFieldLabel("Столбцов:", GridALeft + 148, 56);
            _numColsA = AppTheme.MakeSpinner(GridALeft + 238, 50, 3);

            _numRowsA.ValueChanged += (s, e) => ResizeGrid(_dgvA, (int)_numRowsA.Value, (int)_numColsA.Value);
            _numColsA.ValueChanged += (s, e) => ResizeGrid(_dgvA, (int)_numRowsA.Value, (int)_numColsA.Value);

            _dgvA = CreateGrid(GridALeft, GridTop, 3, 3);

            // ── Matrix B ───────────────────────────────────────────────────────
            var lblB = AppTheme.MakeSectionLabel("Матрица  B", GridBLeft, 24);
            lblB.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblB.ForeColor = Color.FromArgb(0, 180, 230);

            var lblRowsB = AppTheme.MakeFieldLabel("Строк:", GridBLeft, 56);
            _numRowsB = AppTheme.MakeSpinner(GridBLeft + 68, 50, 3);
            var lblColsB = AppTheme.MakeFieldLabel("Столбцов:", GridBLeft + 148, 56);
            _numColsB = AppTheme.MakeSpinner(GridBLeft + 238, 50, 3);

            _numRowsB.ValueChanged += (s, e) => ResizeGrid(_dgvB, (int)_numRowsB.Value, (int)_numColsB.Value);
            _numColsB.ValueChanged += (s, e) => ResizeGrid(_dgvB, (int)_numRowsB.Value, (int)_numColsB.Value);

            _dgvB = CreateGrid(GridBLeft, GridTop, 3, 3);

            // Operator label between grids
            var vsLabel = new Label
            {
                Text = "×",
                Font = new Font("Consolas", 28F, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, AppTheme.Accent),
                AutoSize = true,
                Location = new Point(472, GridTop + 80)
            };

            // ── Operation selector ─────────────────────────────────────────────
            var lblOp = AppTheme.MakeSectionLabel("Операция", GridALeft, ControlsY);

            _cbOp = new ComboBox
            {
                Location = new Point(GridALeft, ControlsY + 24),
                Width = 420,
                Height = AppTheme.InputH,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = AppTheme.InputBg,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.MonoFont,
                FlatStyle = FlatStyle.Flat
            };
            _cbOp.Items.AddRange(new object[] {
                "Сложение          A + B",
                "Вычитание         A − B",
                "Умножение         A × B",
                "Транспонирование  Aᵀ",
                "Определитель      det(A)",
                "Обратная матрица  A⁻¹",
                "Ранг              rank(A)"
            });
            _cbOp.SelectedIndex = 0;

            _btnCalc = AppTheme.MakePrimaryButton("▶  ВЫЧИСЛИТЬ", GridALeft, ControlsY + 68, 420, 44);
            _btnCalc.Click += BtnCalc_Click;

            // ── Tip labels ─────────────────────────────────────────────────────
            var tipLabel = new Label
            {
                Text = "Для Aᵀ, det(A), A⁻¹, rank(A) используется только матрица A",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = AppTheme.TextMuted,
                AutoSize = true,
                Location = new Point(GridALeft, ControlsY + 120)
            };

            // ── Result card ────────────────────────────────────────────────────
            _resultCard = AppTheme.MakeCard(GridALeft, ControlsY + 148, 960, 340, "Результат");
            _txtResult = AppTheme.MakeResultBox(14, 38, 928, 288);
            _resultCard.Controls.Add(_txtResult);

            Controls.AddRange(new Control[] {
                lblA, lblRowsA, _numRowsA, lblColsA, _numColsA,
                lblB, lblRowsB, _numRowsB, lblColsB, _numColsB,
                _dgvA, vsLabel, _dgvB,
                lblOp, _cbOp, _btnCalc, tipLabel, _resultCard
            });
        }

        private DataGridView CreateGrid(int x, int y, int rows, int cols)
        {
            var dgv = new DataGridView
            {
                Location = new Point(x, y),
                AllowUserToAddRows = false,
                RowHeadersVisible = true,
                ColumnHeadersVisible = true,
                ScrollBars = ScrollBars.None
            };

            for (int i = 0; i < cols; i++)
                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = $"col {i + 1}",
                    Width = AppTheme.ColWidth,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

            for (int i = 0; i < rows; i++)
            {
                dgv.Rows.Add();
                dgv.Rows[i].HeaderCell.Value = $"r{i + 1}";
            }

            ApplyGridSize(dgv, rows, cols);
            AppTheme.StyleGrid(dgv);
            return dgv;
        }

        private void ApplyGridSize(DataGridView dgv, int rows, int cols)
        {
            dgv.Width = cols * AppTheme.ColWidth + dgv.RowHeadersWidth + 4;
            dgv.Height = rows * AppTheme.RowHeight + dgv.ColumnHeadersHeight + 4;
        }

        private void ResizeGrid(DataGridView dgv, int rows, int cols)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            for (int i = 0; i < cols; i++)
                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = $"col {i + 1}",
                    Width = AppTheme.ColWidth,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });
            for (int i = 0; i < rows; i++)
            {
                dgv.Rows.Add();
                dgv.Rows[i].HeaderCell.Value = $"r{i + 1}";
            }
            ApplyGridSize(dgv, rows, cols);
        }

        private void BtnCalc_Click(object? sender, EventArgs e)
        {
            try
            {
                var mA = GetMatrix(_dgvA);
                var mB = GetMatrix(_dgvB);
                string op = _cbOp.SelectedItem?.ToString()?.Trim() ?? "";

                string msg;
                if (op.StartsWith("Сложение")) msg = FormatMatrix(mA + mB);
                else if (op.StartsWith("Вычитание")) msg = FormatMatrix(mA - mB);
                else if (op.StartsWith("Умножение")) msg = FormatMatrix(mA * mB);
                else if (op.StartsWith("Транспон")) msg = FormatMatrix(mA.Transpose());
                else if (op.StartsWith("Определитель")) msg = $"det(A)  =  {mA.Determinant():F8}";
                else if (op.StartsWith("Обратная"))
                {
                    if (Math.Abs(mA.Determinant()) < 1e-9)
                        msg = "⚠  Матрица вырождена — обратной не существует.";
                    else
                        msg = FormatMatrix(mA.Inverse());
                }
                else if (op.StartsWith("Ранг")) msg = $"rank(A)  =  {mA.Rank()}";
                else msg = "Выберите операцию";

                _txtResult.ForeColor = AppTheme.Accent;
                _txtResult.Text = msg;
            }
            catch (Exception ex)
            {
                _txtResult.ForeColor = AppTheme.Danger;
                _txtResult.Text = "⚠  " + ex.Message;
            }
        }

        private string FormatMatrix(Matrix m)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"  Размер: {m.Rows} × {m.Cols}\n");
            for (int i = 0; i < m.Rows; i++)
            {
                sb.Append("│");
                for (int j = 0; j < m.Cols; j++)
                    sb.Append($"  {m[i, j],10:F4}");
                sb.AppendLine("  │");
            }
            return sb.ToString();
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
