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
        private Panel _gridContainer;

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
            // ── Заголовок ───────────────────────────────────────────────────
            var lblTitle = AppTheme.MakeSectionLabel("Система линейных алгебраических уравнений  [A]·x = b", AppTheme.PadLeft, 22);
            lblTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTitle.ForeColor = AppTheme.Accent;

            var lblRows = new Label { Text = "Уравнений", Left = AppTheme.PadLeft, Top = 64, AutoSize = true, ForeColor = AppTheme.TextMuted };
            var lblCols = new Label { Text = "Неизвестных", Left = AppTheme.PadLeft + 90, Top = 64, AutoSize = true, ForeColor = AppTheme.TextMuted };

            _numRows = AppTheme.MakeSpinner(AppTheme.PadLeft, 84, 5);
            _numCols = AppTheme.MakeSpinner(AppTheme.PadLeft + 90, 84, 5);

            _numRows.ValueChanged += UpdateGridSize;
            _numCols.ValueChanged += UpdateGridSize;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblRows);
            this.Controls.Add(lblCols);
            this.Controls.Add(_numRows);
            this.Controls.Add(_numCols);

            // ── Контейнер для таблиц (решает проблему с багами UI) ──────────
            _gridContainer = new Panel
            {
                Location = new Point(AppTheme.PadLeft, GridTop),
                Size = new Size(820, 260),
                AutoScroll = true,
                BackColor = AppTheme.Background
            };
            this.Controls.Add(_gridContainer);

            // Создаем таблицы через универсальный метод
            _dgvCoeffs = CreateStyledGrid();
            _dgvFree = CreateStyledGrid();

            _gridContainer.Controls.Add(_dgvCoeffs);
            _gridContainer.Controls.Add(_dgvFree);

            // ── Панель управления ──────────────────────────────────────────
            var lblMethod = new Label { Text = "Метод решения:", Left = AppTheme.PadLeft, Top = ControlsY, AutoSize = true };
            _cbMethod = new ComboBox
            {
                Location = new Point(AppTheme.PadLeft, ControlsY + 22),
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = AppTheme.SurfaceRaised,
                ForeColor = AppTheme.TextPrimary,
                FlatStyle = FlatStyle.Flat
            };

            _lblDiagNote = new Label
            {
                ForeColor = AppTheme.TextMuted,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                Location = new Point(AppTheme.PadLeft + 240, ControlsY + 26),
                AutoSize = true
            };

            _btnCalc = new StyledButton(ButtonVariant.Primary)
            {
                Text = "РЕШИТЬ СЛАУ",
                Location = new Point(AppTheme.PadLeft, ControlsY + 70),
                Size = new Size(180, 40)
            };
            _btnCalc.Click += BtnCalc_Click; // Твой метод

            _btnReset = new StyledButton(ButtonVariant.Secondary)
            {
                Text = "ОЧИСТИТЬ",
                Location = new Point(AppTheme.PadLeft + 195, ControlsY + 70),
                Size = new Size(120, 40)
            };
            _btnReset.Click += (s, e) => { ClearGrids(); };

            this.Controls.Add(lblMethod);
            this.Controls.Add(_cbMethod);
            this.Controls.Add(_lblDiagNote);
            this.Controls.Add(_btnCalc);
            this.Controls.Add(_btnReset);

            // ── Поле результата ────────────────────────────────────────────
            var lblRes = AppTheme.MakeSectionLabel("РЕЗУЛЬТАТ", AppTheme.PadLeft, ControlsY + 130);
            _txtResult = AppTheme.MakeResultBox(AppTheme.PadLeft, ControlsY + 155, 600, 180);

            this.Controls.Add(lblRes);
            this.Controls.Add(_txtResult);

            // Первичная отрисовка 5x5
            UpdateGridSize(null, null);
        }

        // Вспомогательный метод для создания сетки (вместо CreateCoeffGrid/CreateFreeGrid)
        private DataGridView CreateStyledGrid()
        {
            var dgv = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToOrderColumns = false,
                RowHeadersVisible = true,
                ColumnHeadersVisible = true,
                ScrollBars = ScrollBars.None, // Прокруткой заведует Panel
                BackgroundColor = AppTheme.Background,
                BorderStyle = BorderStyle.None
            };
            AppTheme.StyleGrid(dgv);
            return dgv;
        }

        private void UpdateGridSize(object? sender, EventArgs? e)
        {
            int rows = (int)_numRows.Value;
            int cols = (int)_numCols.Value;

            _dgvCoeffs.SuspendLayout();
            _dgvFree.SuspendLayout();

            // 1. Обновляем матрицу коэффициентов A
            _dgvCoeffs.ColumnCount = cols;
            for (int i = 0; i < cols; i++)
            {
                _dgvCoeffs.Columns[i].HeaderText = $"x{SubscriptDigit(i + 1)}";
                _dgvCoeffs.Columns[i].Width = 60;
                _dgvCoeffs.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            _dgvCoeffs.RowCount = rows;
            for (int i = 0; i < rows; i++)
            {
                _dgvCoeffs.Rows[i].HeaderCell.Value = $"eq{i + 1}";
            }

            // 2. Обновляем вектор свободных членов b
            _dgvFree.ColumnCount = 1;
            _dgvFree.Columns[0].HeaderText = "  b";
            _dgvFree.Columns[0].Width = 70;
            _dgvFree.RowCount = rows;
            for (int i = 0; i < rows; i++)
            {
                _dgvFree.Rows[i].HeaderCell.Value = $"b{i + 1}";
            }

            // 3. Расчет геометрии (чтобы панель прокрутки знала размеры)
            _dgvCoeffs.Height = (rows * _dgvCoeffs.RowTemplate.Height) + _dgvCoeffs.ColumnHeadersHeight + 2;
            _dgvCoeffs.Width = (cols * 60) + _dgvCoeffs.RowHeadersWidth + 2;

            _dgvFree.Height = _dgvCoeffs.Height;
            _dgvFree.Width = 70 + _dgvFree.RowHeadersWidth + 2;
            _dgvFree.Location = new Point(_dgvCoeffs.Right + 30, 0);

            _dgvCoeffs.ResumeLayout();
            _dgvFree.ResumeLayout();

            // Обновляем методы и надписи
            RebuildMethodList();
            UpdateDiagNote();
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
            bool sq = (int)_numRows.Value == (int)_numCols.Value;
            _cbMethod.Items.Clear();
            _cbMethod.Items.Add("Метод Гаусса (пошагово)");
            if (sq)
            {
                _cbMethod.Items.Add("Метод Крамера");
                _cbMethod.Items.Add("Матричный метод");
            }
            _cbMethod.SelectedIndex = 0;
        }

        private void ClearGrids()
        {
            foreach (DataGridViewRow row in _dgvCoeffs.Rows)
                foreach (DataGridViewCell cell in row.Cells) cell.Value = "0";
            foreach (DataGridViewRow row in _dgvFree.Rows)
                if (row.Cells.Count > 0) row.Cells[0].Value = "0";
            _txtResult.Clear();
        }

        /*private void UpdateGridSize(object sender, EventArgs e)
        {
            int rows = (int)_numRows.Value;
            int cols = (int)_numCols.Value;

            // Блокируем перерисовку для производительности
            _dgvCoeffs.SuspendLayout();
            _dgvFree.SuspendLayout();

            _dgvCoeffs.RowCount = rows;
            _dgvCoeffs.ColumnCount = cols;
            _dgvFree.RowCount = rows;
            _dgvFree.ColumnCount = 1;

            // Устанавливаем ширину колонок
            for (int i = 0; i < cols; i++) _dgvCoeffs.Columns[i].Width = 60;
            _dgvFree.Columns[0].Width = 70;

            // Рассчитываем реальный размер таблиц, чтобы панель "поняла", что нужна прокрутка
            _dgvCoeffs.Height = (rows * _dgvCoeffs.RowTemplate.Height) + _dgvCoeffs.ColumnHeadersHeight + 2;
            _dgvCoeffs.Width = (cols * 60) + _dgvCoeffs.RowHeadersWidth + 2;

            _dgvFree.Height = _dgvCoeffs.Height;
            _dgvFree.Left = _dgvCoeffs.Right + 20; // Вектор b всегда справа от матрицы A

            _dgvCoeffs.ResumeLayout();
            _dgvFree.ResumeLayout();

            UpdateDiagNote();
        }*/
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

        /*private void ClearGrids()
        {
            foreach (DataGridViewRow row in _dgvCoeffs.Rows)
                foreach (DataGridViewCell cell in row.Cells)
                    cell.Value = "0";
            foreach (DataGridViewRow row in _dgvFree.Rows)
                if (row.Cells.Count > 0) row.Cells[0].Value = "0";
            _txtResult.Text = "";
        }*/

        /*private void UpdateDiagNote()
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
        }*/

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