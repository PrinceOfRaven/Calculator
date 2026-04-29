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
        private Button _btnCalcTransition;
        private TextBox _txtBasisResult;
        private TextBox _txtVecX;
        private TextBox _txtVecY;
        private TextBox _txtVecZ;

        public BasisTab() : base("Базисы и переходы")
        {
            AutoScroll = true;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            var lblInfo = new Label
            {
                Text = "Введите базис E(по умолчанию единичный) и базис F. Будет найдена матрица перехода и координаты.",
                Location = new Point(20, 20),
                Width = 600,
                Height = 40
            };

            _dgvBasisE = CreateGrid(20, 70, 3, 3, "Базис E (Столбцы - векторы)");
            _dgvBasisF = CreateGrid(400, 70, 3, 3, "Базис F (Столбцы - векторы)");

            var lblVecCoords = new Label { Text = "Вектор для перевода (координаты в базисе E):", Location = new Point(20, 380), Width = 300 };
            _txtVecX = new TextBox { Location = new Point(20, 410), Width = 60, Text = "1" };
            _txtVecY = new TextBox { Location = new Point(90, 410), Width = 60, Text = "0" };
            _txtVecZ = new TextBox { Location = new Point(160, 410), Width = 60, Text = "0" };

            _btnCalcTransition = new Button { Text = "Найти матрицу перехода и новые координаты", Location = new Point(20, 460), Width = 350, Height = 40 };
            _btnCalcTransition.Click += BtnCalcTransition_Click;

            _txtBasisResult = new TextBox
            {
                Location = new Point(20, 520),
                Width = 700,
                Height = 200,
                Multiline = true,
                ReadOnly = true
            };

            Controls.AddRange(new Control[] {
                lblInfo, _dgvBasisE, _dgvBasisF, lblVecCoords, _txtVecX, _txtVecY, _txtVecZ,
                _btnCalcTransition, _txtBasisResult
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

        private void BtnCalcTransition_Click(object? sender, EventArgs e)
        {
            try
            {
                var E = GetMatrixFromGrid(_dgvBasisE);
                var F = GetMatrixFromGrid(_dgvBasisF);

                // Матрица перехода от E к F: P = E^-1 * F (если E единичный, то просто F)
                // Если вектор задан в старом (E), то X_new = P^-1 * X_old, где P - матрица из новых базисных векторов в столбцах.

                var P = F; // Считаем, что E - единичный для простоты, или P = E.Inverse() * F
                if (Math.Abs(E.Determinant() - 1.0) > 1e-9 && E.Determinant() != 0.0)
                {
                    // Общая формула: [F] = [E] * P => P = [E]^-1 * [F]
                    P = E.Inverse() * F;
                }

                double vx = double.Parse(_txtVecX.Text);
                double vy = double.Parse(_txtVecY.Text);
                double vz = double.Parse(_txtVecZ.Text);

                var oldCoords = new double[] { vx, vy, vz };
                // Новые координаты: X_new = P^-1 * X_old
                var P_inv = P.Inverse();
                
                // Умножение матрицы на вектор вручную (P_inv * oldCoords)
                var newCoords = new double[3];
                for (int i = 0; i < 3; i++)
                {
                    newCoords[i] = 0;
                    for (int j = 0; j < 3; j++)
                    {
                        newCoords[i] += P_inv[i, j] * oldCoords[j];
                    }
                }

                _txtBasisResult.Text = $"Матрица перехода P (от E к F):\n{P}\n\n" +
                                      $"Координаты вектора в новом базисе:\n" +
                                      $"x' = {newCoords[0]:F4}\ny' = {newCoords[1]:F4}\nz' = {newCoords[2]:F4}";
            }
            catch (Exception ex) { _txtBasisResult.Text = "Ошибка: " + ex.Message; }
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
