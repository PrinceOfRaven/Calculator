using Calculator.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MatrixCalculator.UI
{
    public class SlaeTab : TabPage
    {
        // Size controls — rows and cols are now SEPARATE (key change)
        private NumericUpDown _numRows;   // equations (vertical)
        private NumericUpDown _numCols;   // unknowns (horizontal)

        private DataGridView _dgvCoeffs;
        private DataGridView _dgvFree;
        private ComboBox _cbMethod;
        private Button _btnCalc;
        private Button _btnReset;
        private TextBox _txtResult;
        private Label _lblDiagNote;

        private const int GridTop = 110;
        private const int ControlsY = 380;

        public SlaeTab() : base("СЛАУ")
        {
            BackColor = AppTheme.Background;
            AutoScroll = true;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ── Title & description ────────────────────────────────────────────
            var lblTitle = AppTheme.MakeSectionLabel("Система линейных алгебраических уравнений  [A]·x = b", AppTheme.PadLeft, 22);
            lblTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTitle.ForeColor = AppTheme.Accent;

            // ── Size controls ──────────────────────────────────────────────────
            //    Rows = equations (vertical), Cols = unknowns (horizontal)
            //    These are deliberately kept independent.

            var sizeCard = new Panel
            {
                Location = new Point(AppTheme.PadLeft, 54),
                Size = new Size(800, 46),
                BackColor = Color.Transparent
            };

            var lblRows = AppTheme.MakeFieldLabel("Уравнений (строк):", 0, 12);
            _numRows = AppTheme.MakeSpinner(170, 8, 3);

            var lblCols = AppTheme.MakeFieldLabel("Неизвестных (столбцов):", 250, 12);
            _numCols = AppTheme.MakeSpinner(450, 8, 3);

            // Diagonal info badge — shows when it's a square system
            _lblDiagNote = new Label
            {
                Text = "✓ Квадратная — все методы доступны",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = AppTheme.Success,
                AutoSize = true,
                Location = new Point(730, 14)
            };

            var btnApply = AppTheme.MakeSecondaryButton("ПРИМЕНИТЬ", 524, 6, 120, 32);
            btnApply.Click += (s, e) => ApplySize();

            _numRows.ValueChanged += (s, e) => UpdateDiagNote();
            _numCols.ValueChanged += (s, e) => UpdateDiagNote();

            sizeCard.Controls.AddRange(new Control[] {
                lblRows, _numRows, lblCols, _numCols, _lblDiagNote, btnApply
            });

            // ── Section labels ─────────────────────────────────────────────────
            var lblCoeffs = AppTheme.MakeSectionLabel("Коэффициенты  [A]", AppTheme.PadLeft, GridTop - 22);
            var lblFree = AppTheme.MakeSectionLabel("Свободные  [b]", 430, GridTop - 22);

            // ── Grids ──────────────────────────────────────────────────────────
            _dgvCoeffs = CreateCoeffGrid(AppTheme.PadLeft, GridTop, 3, 3);
            _dgvFree = CreateFreeGrid(430, GridTop, 3);

            // ── Method selector ────────────────────────────────────────────────
            var lblMethod = AppTheme.MakeSectionLabel("Метод решения", AppTheme.PadLeft, ControlsY);

            _cbMethod = new ComboBox
            {
                Location = new Point(AppTheme.PadLeft, ControlsY + 26),
                Width = 500,
                Height = AppTheme.InputH,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = AppTheme.InputBg,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.MonoFont,
                FlatStyle = FlatStyle.Flat
            };
            RebuildMethodList();

            // ── Buttons ────────────────────────────────────────────────────────
            _btnCalc = AppTheme.MakePrimaryButton("▶  РЕШИТЬ СИСТЕМУ", AppTheme.PadLeft, ControlsY + 72, 350, 44);
            _btnReset = AppTheme.MakeSecondaryButton("✕  ОЧИСТИТЬ", AppTheme.PadLeft + 366, ControlsY + 72, 170, 44);

            _btnCalc.Click += BtnCalc_Click;
            _btnReset.Click += (s, e) => ClearGrids();

            // ── Method hint ────────────────────────────────────────────────────
            var lblHint = new Label
            {
                Text = "Метод Крамера и матричный метод работают только для квадратных систем (m = n)",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = AppTheme.TextMuted,
                AutoSize = true,
                Location = new Point(AppTheme.PadLeft, ControlsY + 124)
            };

            // ── Result card ────────────────────────────────────────────────────
            var resultCard = AppTheme.MakeCard(AppTheme.PadLeft, ControlsY + 150, 960, 380, "Решение");
            _txtResult = AppTheme.MakeResultBox(14, 38, 928, 328);
            resultCard.Controls.Add(_txtResult);

            Controls.AddRange(new Control[] {
                lblTitle, sizeCard, lblCoeffs, lblFree,
                _dgvCoeffs, _dgvFree, lblMethod, _cbMethod,
                _btnCalc, _btnReset, lblHint, resultCard
            });
        }

        // ── Grid factories ─────────────────────────────────────────────────────

        private DataGridView CreateCoeffGrid(int x, int y, int rows, int cols)
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
                    HeaderText = $"x{SubscriptDigit(i + 1)}",
                    Width = AppTheme.ColWidth,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

            for (int i = 0; i < rows; i++)
            {
                dgv.Rows.Add();
                dgv.Rows[i].HeaderCell.Value = $"eq{i + 1}";
            }

            ApplyGridSize(dgv, rows, cols);
            AppTheme.StyleGrid(dgv);
            return dgv;
        }

        private DataGridView CreateFreeGrid(int x, int y, int rows)
        {
            var dgv = new DataGridView
            {
                Location = new Point(x, y),
                AllowUserToAddRows = false,
                RowHeadersVisible = true,
                ColumnHeadersVisible = true,
                ScrollBars = ScrollBars.None
            };

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "  b",
                Width = AppTheme.ColWidth + 10,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            for (int i = 0; i < rows; i++)
            {
                dgv.Rows.Add();
                dgv.Rows[i].HeaderCell.Value = $"b{i + 1}";
            }

            dgv.Width = AppTheme.ColWidth + 10 + dgv.RowHeadersWidth + 4;
            dgv.Height = rows * AppTheme.RowHeight + dgv.ColumnHeadersHeight + 4;
            AppTheme.StyleGrid(dgv);
            return dgv;
        }

        private void ApplyGridSize(DataGridView dgv, int rows, int cols)
        {
            dgv.Width = cols * AppTheme.ColWidth + dgv.RowHeadersWidth + 4;
            dgv.Height = rows * AppTheme.RowHeight + dgv.ColumnHeadersHeight + 4;
        }

        // ── Size apply ─────────────────────────────────────────────────────────

        private void ApplySize()
        {
            int rows = (int)_numRows.Value;
            int cols = (int)_numCols.Value;

            // Rebuild coefficient grid
            _dgvCoeffs.Rows.Clear();
            _dgvCoeffs.Columns.Clear();
            for (int i = 0; i < cols; i++)
                _dgvCoeffs.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = $"x{SubscriptDigit(i + 1)}",
                    Width = AppTheme.ColWidth,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });
            for (int i = 0; i < rows; i++)
            {
                _dgvCoeffs.Rows.Add();
                _dgvCoeffs.Rows[i].HeaderCell.Value = $"eq{i + 1}";
            }
            ApplyGridSize(_dgvCoeffs, rows, cols);

            // Rebuild free vector grid
            _dgvFree.Rows.Clear();
            for (int i = 0; i < rows; i++)
            {
                _dgvFree.Rows.Add();
                _dgvFree.Rows[i].HeaderCell.Value = $"b{i + 1}";
            }
            _dgvFree.Height = rows * AppTheme.RowHeight + _dgvFree.ColumnHeadersHeight + 4;

            // Reposition free grid to right of coeff grid
            _dgvFree.Location = new Point(_dgvCoeffs.Right + 24, _dgvCoeffs.Top);

            RebuildMethodList();
            UpdateDiagNote();

            _txtResult.Text = "";
            _txtResult.ForeColor = AppTheme.Accent;
        }

        private void ClearGrids()
        {
            foreach (DataGridViewRow row in _dgvCoeffs.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Value = "0";
            foreach (DataGridViewRow row in _dgvFree.Rows)
                if (row.Cells.Count > 0) row.Cells[0].Value = "0";
            _txtResult.Text = "";
        }

        private void UpdateDiagNote()
        {
            bool square = (int)_numRows.Value == (int)_numCols.Value;
            _lblDiagNote.Text = square
                ? "✓ Квадратная — все методы доступны"
                : $"ℹ Прямоугольная ({_numRows.Value}×{_numCols.Value}) — только метод Гаусса";
            _lblDiagNote.ForeColor = square ? AppTheme.Success : AppTheme.Warning;
        }

        private void RebuildMethodList()
        {
            int rows = (int)_numRows.Value;
            int cols = (int)_numCols.Value;
            bool sq = rows == cols;

            _cbMethod.Items.Clear();
            _cbMethod.Items.Add("Метод Гаусса  (с пошаговым выводом)");
            if (sq)
            {
                _cbMethod.Items.Add("Метод Крамера");
                _cbMethod.Items.Add("Матричный метод  (A⁻¹ · b)");
            }
            _cbMethod.SelectedIndex = 0;
        }

        // ── Calculation ────────────────────────────────────────────────────────

        private void BtnCalc_Click(object? sender, EventArgs e)
        {
            try
            {
                var A = GetMatrix(_dgvCoeffs);
                var bVec = GetFreeVector();

                string method = _cbMethod.SelectedItem?.ToString() ?? "";
                string result;

                result = method switch
                {
                    var m when m.StartsWith("Метод Гаусса") => SlaeSolver.SolveGaussWithSteps(A, bVec),
                    "Метод Крамера" => FormatSolution(SlaeSolver.SolveCramer(A, bVec), "Метод Крамера"),
                    var m when m.StartsWith("Матричный") => FormatSolution(SlaeSolver.SolveInverse(A, bVec), "Матричный метод"),
                    _ => SlaeSolver.SolveGaussWithSteps(A, bVec)
                };

                _txtResult.ForeColor = AppTheme.Accent;
                _txtResult.Text = result;
            }
            catch (Exception ex)
            {
                _txtResult.ForeColor = AppTheme.Danger;
                _txtResult.Text = "⚠  " + ex.Message;
            }
        }

        private string FormatSolution(double[] x, string methodName)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Метод: {methodName}");
            sb.AppendLine(new string('─', 40));
            sb.AppendLine();
            for (int i = 0; i < x.Length; i++)
                sb.AppendLine($"  x{SubscriptDigit(i + 1)}  =  {x[i]:F8}");
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

        private double[] GetFreeVector()
        {
            int r = _dgvFree.Rows.Count;
            var vec = new double[r];
            for (int i = 0; i < r; i++)
                vec[i] = double.Parse(_dgvFree.Rows[i].Cells[0].Value?.ToString() ?? "0");
            return vec;
        }

        // Convert digit to Unicode subscript for pretty headers
        private static string SubscriptDigit(int n)
        {
            var map = new[] { "₀", "₁", "₂", "₃", "₄", "₅", "₆", "₇", "₈", "₉" };
            return n < 10 ? map[n] : n.ToString();
        }
    }
}