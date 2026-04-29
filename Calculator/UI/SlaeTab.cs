using Calculator.Core;
using MatrixCalculator.UI;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MatrixCalculator.UI
{
    public class SlaeTab : TabPage
    {
        private NumericUpDown _numRows;
        private NumericUpDown _numCols;
        private DataGridView _dgvSlaeCoeffs;
        private DataGridView _dgvSlaeFree;
        private ComboBox _cbSlaeMethod;
        private Button _btnCalcSlae;
        private TextBox _txtSlaeResult;

        public SlaeTab() : base("СЛАУ")
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var lblRows = new Label { Text = "Кол-во строк:", Location = new Point(20, 20), Width = 100 };
            _numRows = new NumericUpDown { Location = new Point(120, 18), Width = 50, Minimum = 1, Maximum = 10, Value = 3 };

            var lblCols = new Label { Text = "Кол-во столбцов (неизвестных):", Location = new Point(180, 20), Width = 140 };
            _numCols = new NumericUpDown { Location = new Point(330, 18), Width = 50, Minimum = 1, Maximum = 10, Value = 3 };

            var btnInit = new Button { Text = "Создать", Location = new Point(390, 15), Width = 80, Height = 30 };
            btnInit.Click += (s, e) => ResizeGrids((int)_numRows.Value, (int)_numCols.Value);

            _dgvSlaeCoeffs = CreateGrid(20, 60, 3, 3, "Коэфф.");
            _dgvSlaeFree = CreateGrid(350, 60, 3, 1, "Свободные");

            _cbSlaeMethod = new ComboBox
            {
                Location = new Point(20, 350),
                Width = 350,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            UpdateMethodList();
            _cbSlaeMethod.SelectedIndexChanged += (s, e) => { };
            _cbSlaeMethod.SelectedIndex = 0;

            _btnCalcSlae = new Button { Text = "Решить", Location = new Point(20, 390), Width = 350, Height = 40 };
            _btnCalcSlae.Click += BtnCalcSlae_Click;

            _txtSlaeResult = new TextBox
            {
                Location = new Point(20, 450),
                Width = 660,
                Height = 150,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            Controls.AddRange(new Control[] {
                lblRows, _numRows, lblCols, _numCols, btnInit, _dgvSlaeCoeffs, _dgvSlaeFree,
                _cbSlaeMethod, _btnCalcSlae, _txtSlaeResult
            });
        }

        private void UpdateMethodList()
        {
            int rows = (int)_numRows.Value;
            int cols = (int)_numCols.Value;

            _cbSlaeMethod.Items.Clear();
            _cbSlaeMethod.Items.Add("Метод Гаусса (с шагами)");

            // Методы Крамера и матричный метод доступны только для квадратных систем
            if (rows == cols)
            {
                _cbSlaeMethod.Items.Add("Метод Крамера");
                _cbSlaeMethod.Items.Add("Матричный метод");
            }
            else
            {
                _cbSlaeMethod.Items.Add($"Метод Крамера (только для квадратных {cols}x{cols})");
                _cbSlaeMethod.Items.Add($"Матричный метод (только для квадратных {cols}x{cols})");
            }

            _cbSlaeMethod.SelectedIndex = 0;
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

        private void ResizeGrids(int rows, int cols)
        {
            UpdateGridSize(_dgvSlaeCoeffs, rows, cols);
            UpdateGridSize(_dgvSlaeFree, rows, 1);
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

        private void BtnCalcSlae_Click(object? sender, EventArgs e)
        {
            try
            {
                var A = GetMatrixFromGrid(_dgvSlaeCoeffs);
                var B = GetMatrixFromGrid(_dgvSlaeFree);
                var bVec = new double[B.Rows];
                for (int i = 0; i < B.Rows; i++) bVec[i] = B[i, 0];

                string method = _cbSlaeMethod.SelectedItem?.ToString() ?? "";
                string resultText = "";

                // Проверяем, выбран ли метод для неквадратной системы
                if ((method.Contains("только для квадратных")) && (_numRows.Value != _numCols.Value))
                {
                    _txtSlaeResult.Text = "Ошибка: выбранный метод применим только к квадратным системам. Используйте метод Гаусса.";
                    return;
                }

                if (method == "Метод Гаусса (с шагами)" || method.StartsWith("Метод Гаусса"))
                {
                    resultText = SlaeSolver.SolveGaussWithSteps(A, bVec);
                }
                else if (method == "Метод Крамера")
                {
                    var x = SlaeSolver.SolveCramer(A, bVec);
                    resultText = FormatSolution(x);
                }
                else if (method == "Матричный метод")
                {
                    var x = SlaeSolver.SolveInverse(A, bVec);
                    resultText = FormatSolution(x);
                }
                else
                {
                    // По умолчанию используем метод Гаусса
                    resultText = SlaeSolver.SolveGaussWithSteps(A, bVec);
                }

                _txtSlaeResult.Text = resultText;
            }
            catch (Exception ex) { _txtSlaeResult.Text = "Ошибка: " + ex.Message; }
        }

        private string FormatSolution(double[] x)
        {
            var res = "";
            for (int i = 0; i < x.Length; i++) res += $"x{i + 1} = {x[i]:F4}\n";
            return res;
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
    }
}