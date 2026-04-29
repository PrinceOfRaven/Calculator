using System;
using System.Drawing;
using System.Windows.Forms;
using Calculator.Core;

namespace MatrixCalculator.UI
{
    public class MatrixTab : TabPage
    {
        private NumericUpDown _numMatrixRowsA;
        private NumericUpDown _numMatrixColsA;
        private NumericUpDown _numMatrixRowsB;
        private NumericUpDown _numMatrixColsB;
        private DataGridView _dgvMatrixA;
        private DataGridView _dgvMatrixB;
        private ComboBox _cbMatrixOp;
        private Button _btnCalcMatrix;
        private TextBox _txtMatrixResult;

        public MatrixTab() : base("Матрицы")
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Размер матрицы A (строки и столбцы отдельно)
            var lblSizeA = new Label { Text = "Размер A:", Location = new Point(20, 20), Width = 75 };
            var lblRowsA = new Label { Text = "Строк:", Location = new Point(90, 20), Width = 50 };
            _numMatrixRowsA = new NumericUpDown { Location = new Point(140, 20), Width = 40, Minimum = 1, Maximum = 10, Value = 3 };
            var lblColsA = new Label { Text = "Столбцов:", Location = new Point(190, 20), Width = 75 };
            _numMatrixColsA = new NumericUpDown { Location = new Point(265, 20), Width = 40, Minimum = 1, Maximum = 10, Value = 3 };
            
            _numMatrixRowsA.ValueChanged += (s, e) => ResizeGridA((int)_numMatrixRowsA.Value, (int)_numMatrixColsA.Value);
            _numMatrixColsA.ValueChanged += (s, e) => ResizeGridA((int)_numMatrixRowsA.Value, (int)_numMatrixColsA.Value);

            // Размер матрицы B (строки и столбцы отдельно)
            var lblSizeB = new Label { Text = "Размер B:", Location = new Point(390, 20), Width = 75 };
            var lblRowsB = new Label { Text = "Строк:", Location = new Point(460, 20), Width = 50 };
            _numMatrixRowsB = new NumericUpDown { Location = new Point(510, 20), Width = 40, Minimum = 1, Maximum = 10, Value = 3 };
            var lblColsB = new Label { Text = "Столбцов:", Location = new Point(560, 20), Width = 75 };
            _numMatrixColsB = new NumericUpDown { Location = new Point(635, 20), Width = 40, Minimum = 1, Maximum = 10, Value = 3 };
            
            _numMatrixRowsB.ValueChanged += (s, e) => ResizeGridB((int)_numMatrixRowsB.Value, (int)_numMatrixColsB.Value);
            _numMatrixColsB.ValueChanged += (s, e) => ResizeGridB((int)_numMatrixRowsB.Value, (int)_numMatrixColsB.Value);

            _dgvMatrixA = CreateGrid(20, 60, 3, 3, "Матрица A");
            _dgvMatrixB = CreateGrid(400, 60, 3, 3, "Матрица B");

            _cbMatrixOp = new ComboBox
            {
                Location = new Point(20, 350),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cbMatrixOp.Items.AddRange(new object[] {
                "Сложение (A+B)", "Вычитание (A-B)", "Умножение (A*B)",
                "Транспонирование (A^T)", "Определитель (det A)",
                "Обратная матрица (A^-1)", "Ранг (Rank A)"
            });
            _cbMatrixOp.SelectedIndex = 0;

            _btnCalcMatrix = new Button { Text = "Рассчитать", Location = new Point(20, 390), Width = 250, Height = 40 };
            _btnCalcMatrix.Click += BtnCalcMatrix_Click;

            _txtMatrixResult = new TextBox
            {
                Location = new Point(20, 450),
                Width = 660,
                Height = 150,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            Controls.AddRange(new Control[] {
                lblSizeA, lblRowsA, _numMatrixRowsA, lblColsA, _numMatrixColsA, //btnInitA,
                lblSizeB, lblRowsB, _numMatrixRowsB, lblColsB, _numMatrixColsB, //btnInitB,
                _dgvMatrixA, _dgvMatrixB,
                _cbMatrixOp, _btnCalcMatrix, _txtMatrixResult
            });
        }

        private DataGridView CreateGrid(int x, int y, int rows, int cols, string name)
        {
            var dgv = new DataGridView
            {
                Location = new Point(x, y),
                Width = cols * 60 + 50,
                Height = rows * 30 + 40,
                AllowUserToAddRows = false,
                RowHeadersVisible = true,
                ColumnHeadersVisible = true
            };

            for (int i = 0; i < cols; i++)
            {
                var col = new DataGridViewTextBoxColumn
                {
                    HeaderText = (i < 3) ? $"{(char)('X' + i)}" : $"Col{i + 1}",
                    Width = 55,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };
                dgv.Columns.Add(col);
            }

            for (int i = 0; i < rows; i++)
                dgv.Rows.Add();

            return dgv;
        }

        private void ResizeGridA(int rows, int cols)
        {
            UpdateGridSize(_dgvMatrixA, rows, cols);
        }

        private void ResizeGridB(int rows, int cols)
        {
            UpdateGridSize(_dgvMatrixB, rows, cols);
        }

        private void UpdateGridSize(DataGridView dgv, int rows, int cols)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            for (int i = 0; i < cols; i++)
                dgv.Columns.Add(new DataGridViewTextBoxColumn { Width = 55, SortMode = DataGridViewColumnSortMode.NotSortable });
            for (int i = 0; i < rows; i++)
                dgv.Rows.Add();
        }

        private void BtnCalcMatrix_Click(object? sender, EventArgs e)
        {
            try
            {
                var mA = GetMatrixFromGrid(_dgvMatrixA);
                var mB = GetMatrixFromGrid(_dgvMatrixB);
                string op = _cbMatrixOp.SelectedItem?.ToString() ?? "";
                Matrix res;
                string msg = "";

                switch (op)
                {
                    case "Сложение (A+B)": res = mA + mB; msg = res.ToString(); break;
                    case "Вычитание (A-B)": res = mA - mB; msg = res.ToString(); break;
                    case "Умножение (A*B)": res = mA * mB; msg = res.ToString(); break;
                    case "Транспонирование (A^T)": res = mA.Transpose(); msg = res.ToString(); break;
                    case "Определитель (det A)": msg = $"det(A) = {mA.Determinant()}"; break;
                    case "Обратная матрица (A^-1)":
                        if (Math.Abs(mA.Determinant()) < 1e-9) msg = "Матрица вырождена!";
                        else { res = mA.Inverse(); msg = res.ToString(); }
                        break;
                    case "Ранг (Rank A)": msg = $"Rank(A) = {mA.Rank()}"; break;
                }
                _txtMatrixResult.Text = msg;
            }
            catch (Exception ex) { _txtMatrixResult.Text = "Ошибка: " + ex.Message; }
        }

        private Matrix GetMatrixFromGrid(DataGridView dgv)
        {
            int r = dgv.Rows.Count;
            int c = dgv.Columns.Count;
            var m = new Matrix(r, c);
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    m[i, j] = double.Parse(dgv.Rows[i].Cells[j].Value?.ToString() ?? "0");
            return m;
        }

        public event Func<Matrix, Matrix, string, string>? CalculationRequested;

        public void RaiseCalculation()
        {
            var mA = GetMatrixFromGrid(_dgvMatrixA);
            var mB = GetMatrixFromGrid(_dgvMatrixB);
            string op = _cbMatrixOp.SelectedItem?.ToString() ?? "";
            
            CalculationRequested?.Invoke(mA, mB, op);
        }
    }
}
