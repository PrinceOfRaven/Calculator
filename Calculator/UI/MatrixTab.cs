using System;
using System.Drawing;
using System.Windows.Forms;
using Calculator.Core;

namespace MatrixCalculator.UI
{
    public class MatrixTab : TabPage
    {
        private NumericUpDown _numMatrixRowsA, _numMatrixColsA;
        private NumericUpDown _numMatrixRowsB, _numMatrixColsB;
        private DataGridView _dgvMatrixA;
        private DataGridView _dgvMatrixB;
        private ComboBox _cbMatrixOp;
        private Button _btnCalcMatrix;
        private TextBox _txtMatrixResult;
        private Panel _resultCard;

        public MatrixTab() : base("Матрицы")
        {
            BackColor = AppTheme.Background;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ── Matrix A controls ──────────────────────────────────────────────
            Controls.Add(AppTheme.MakeSectionLabel("Матрица A", 24, 20));

            var lblRowsA = new Label { Text = "Строк", Font = AppTheme.LabelFont, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(24, 48) };
            _numMatrixRowsA = MakeSpinner(80, 44, 3);
            var lblColsA = new Label { Text = "Столбцов", Font = AppTheme.LabelFont, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(150, 48) };
            _numMatrixColsA = MakeSpinner(230, 44, 3);

            _numMatrixRowsA.ValueChanged += (s, e) => ResizeGrid(_dgvMatrixA, (int)_numMatrixRowsA.Value, (int)_numMatrixColsA.Value);
            _numMatrixColsA.ValueChanged += (s, e) => ResizeGrid(_dgvMatrixA, (int)_numMatrixRowsA.Value, (int)_numMatrixColsA.Value);

            // ── Grid A ──────────────────────────────────────────────────────────
            _dgvMatrixA = CreateGrid(24, 80, 3, 3);

            // ── Matrix B controls ──────────────────────────────────────────────
            Controls.Add(AppTheme.MakeSectionLabel("Матрица B", 24, 260));

            var lblRowsB = new Label { Text = "Строк", Font = AppTheme.LabelFont, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(24, 288) };
            _numMatrixRowsB = MakeSpinner(80, 284, 3);
            var lblColsB = new Label { Text = "Столбцов", Font = AppTheme.LabelFont, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(150, 288) };
            _numMatrixColsB = MakeSpinner(230, 284, 3);

            _numMatrixRowsB.ValueChanged += (s, e) => ResizeGrid(_dgvMatrixB, (int)_numMatrixRowsB.Value, (int)_numMatrixColsB.Value);
            _numMatrixColsB.ValueChanged += (s, e) => ResizeGrid(_dgvMatrixB, (int)_numMatrixRowsB.Value, (int)_numMatrixColsB.Value);

            // ── Grid B ──────────────────────────────────────────────────────────
            _dgvMatrixB = CreateGrid(24, 320, 3, 3);

            // ── Operation selector ─────────────────────────────────────────────
            Controls.Add(AppTheme.MakeSectionLabel("Операция", 440, 20));

            _cbMatrixOp = new ComboBox
            {
                Location = new Point(440, 44),
                Width = 340,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = AppTheme.InputBg,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.MonoFont,
                FlatStyle = FlatStyle.Flat
            };
            _cbMatrixOp.Items.AddRange(new object[] {
                "Сложение        A + B",
                "Вычитание       A − B",
                "Умножение       A × B",
                "Транспонирование  Aᵀ",
                "Определитель    det(A)",
                "Обратная матрица A⁻¹",
                "Ранг            rank(A)"
            });
            _cbMatrixOp.SelectedIndex = 0;

            _btnCalcMatrix = AppTheme.MakePrimaryButton("▶  ВЫЧИСЛИТЬ", 440, 90, 340, 42);
            _btnCalcMatrix.Click += BtnCalcMatrix_Click;

            // ── Result ─────────────────────────────────────────────────────────
            _resultCard = AppTheme.MakeCard(440, 150, 780, 420, "Результат");
            _txtMatrixResult = AppTheme.MakeResultBox(12, 30, 754, 376);
            _resultCard.Controls.Add(_txtMatrixResult);

            Controls.AddRange(new Control[] {
                lblRowsA, _numMatrixRowsA, lblColsA, _numMatrixColsA,
                _dgvMatrixA,
                lblRowsB, _numMatrixRowsB, lblColsB, _numMatrixColsB,
                _dgvMatrixB,
                _cbMatrixOp, _btnCalcMatrix, _resultCard
            });
        }

        private NumericUpDown MakeSpinner(int x, int y, int val)
        {
            return new NumericUpDown
            {
                Location = new Point(x, y),
                Width = 52,
                Minimum = 1,
                Maximum = 10,
                Value = val,
                BackColor = AppTheme.InputBg,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.MonoFont,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private DataGridView CreateGrid(int x, int y, int rows, int cols)
        {
            var dgv = new DataGridView
            {
                Location = new Point(x, y),
                Width = cols * 62 + 40,
                Height = rows * 28 + 30,
                AllowUserToAddRows = false,
                RowHeadersVisible = true,
                ColumnHeadersVisible = true,
                ScrollBars = ScrollBars.None
            };

            for (int i = 0; i < cols; i++)
                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = $"{(char)('a' + i)}",
                    Width = 58,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

            for (int i = 0; i < rows; i++)
                dgv.Rows.Add();

            AppTheme.StyleGrid(dgv);
            return dgv;
        }

        private void ResizeGrid(DataGridView dgv, int rows, int cols)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            for (int i = 0; i < cols; i++)
                dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = $"{(char)('a' + i)}", Width = 58, SortMode = DataGridViewColumnSortMode.NotSortable });
            for (int i = 0; i < rows; i++)
                dgv.Rows.Add();

            dgv.Width = cols * 62 + 40;
            dgv.Height = rows * 28 + 30;
        }

        private void BtnCalcMatrix_Click(object? sender, EventArgs e)
        {
            try
            {
                var mA = GetMatrixFromGrid(_dgvMatrixA);
                var mB = GetMatrixFromGrid(_dgvMatrixB);
                string op = _cbMatrixOp.SelectedItem?.ToString()?.Trim() ?? "";
                Matrix res;
                string msg = "";

                if (op.StartsWith("Сложение"))       { res = mA + mB; msg = FormatMatrix(res); }
                else if (op.StartsWith("Вычитание"))  { res = mA - mB; msg = FormatMatrix(res); }
                else if (op.StartsWith("Умножение"))  { res = mA * mB; msg = FormatMatrix(res); }
                else if (op.StartsWith("Транспон"))   { msg = FormatMatrix(mA.Transpose()); }
                else if (op.StartsWith("Определитель")) { msg = $"det(A)  =  {mA.Determinant():F6}"; }
                else if (op.StartsWith("Обратная"))
                {
                    if (Math.Abs(mA.Determinant()) < 1e-9)
                        msg = "⚠  Матрица вырождена — обратной не существует.";
                    else msg = FormatMatrix(mA.Inverse());
                }
                else if (op.StartsWith("Ранг"))       { msg = $"rank(A)  =  {mA.Rank()}"; }

                _txtMatrixResult.ForeColor = AppTheme.Accent;
                _txtMatrixResult.Text = msg;
            }
            catch (Exception ex)
            {
                _txtMatrixResult.ForeColor = AppTheme.Danger;
                _txtMatrixResult.Text = "⚠  " + ex.Message;
            }
        }

        private string FormatMatrix(Matrix m)
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < m.Rows; i++)
            {
                sb.Append("│");
                for (int j = 0; j < m.Cols; j++)
                    sb.Append($"  {m[i, j],9:F4}");
                sb.AppendLine("  │");
            }
            return sb.ToString();
        }

        private Matrix GetMatrixFromGrid(DataGridView dgv)
        {
            int r = dgv.Rows.Count, c = dgv.Columns.Count;
            var m = new Matrix(r, c);
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    m[i, j] = double.Parse(dgv.Rows[i].Cells[j].Value?.ToString() ?? "0");
            return m;
        }

        public event Func<Matrix, Matrix, string, string>? CalculationRequested;
    }
}
