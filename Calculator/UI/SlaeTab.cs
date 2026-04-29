using Calculator.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MatrixCalculator.UI
{
    public class SlaeTab : TabPage
    {
        private NumericUpDown _numRows;
        private NumericUpDown _numCols;
        private DataGridView _dgvCoeffs;
        private DataGridView _dgvFree;
        private ComboBox _cbMethod;
        private Button _btnCalc;
        private TextBox _txtResult;

        public SlaeTab() : base("СЛАУ")
        {
            BackColor = AppTheme.Background;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ── Size controls ──────────────────────────────────────────────────
            Controls.Add(AppTheme.MakeSectionLabel("Система уравнений", 24, 20));

            var lblRows = new Label { Text = "Уравнений", Font = AppTheme.LabelFont, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(24, 44) };
            _numRows = MakeSpinner(110, 40, 3);
            var lblCols = new Label { Text = "Неизвестных", Font = AppTheme.LabelFont, ForeColor = AppTheme.TextMuted, AutoSize = true, Location = new Point(178, 44) };
            _numCols = MakeSpinner(272, 40, 3);

            var btnInit = AppTheme.MakeSecondaryButton("СОЗДАТЬ", 338, 37, 110, 30);
            btnInit.Click += (s, e) => ResizeGrids((int)_numRows.Value, (int)_numCols.Value);

            // ── Grids ──────────────────────────────────────────────────────────
            Controls.Add(AppTheme.MakeSectionLabel("Коэффициенты  [A]", 24, 84));
            Controls.Add(AppTheme.MakeSectionLabel("Свободные  [b]", 340, 84));

            _dgvCoeffs = CreateGrid(24, 104, 3, 3);
            _dgvFree   = CreateGrid(340, 104, 3, 1);

            // ── Method ─────────────────────────────────────────────────────────
            Controls.Add(AppTheme.MakeSectionLabel("Метод решения", 24, 316));

            _cbMethod = new ComboBox
            {
                Location = new Point(24, 336),
                Width = 400,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = AppTheme.InputBg,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.MonoFont,
                FlatStyle = FlatStyle.Flat
            };
            UpdateMethodList();
            _cbMethod.SelectedIndex = 0;

            _btnCalc = AppTheme.MakePrimaryButton("▶  РЕШИТЬ СИСТЕМУ", 24, 386, 400, 42);
            _btnCalc.Click += BtnCalc_Click;

            // ── Result ─────────────────────────────────────────────────────────
            var resultCard = AppTheme.MakeCard(24, 446, 840, 340, "Решение");
            _txtResult = AppTheme.MakeResultBox(12, 30, 814, 300);
            resultCard.Controls.Add(_txtResult);

            Controls.AddRange(new Control[] {
                lblRows, _numRows, lblCols, _numCols, btnInit,
                _dgvCoeffs, _dgvFree, _cbMethod, _btnCalc, resultCard
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
                Width = cols * 62 + 42,
                Height = rows * 28 + 30,
                AllowUserToAddRows = false,
                RowHeadersVisible = true,
                ScrollBars = ScrollBars.None
            };

            for (int i = 0; i < cols; i++)
                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = cols == 1 ? "b" : $"x{i + 1}",
                    Width = 58,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });

            for (int i = 0; i < rows; i++) dgv.Rows.Add();
            AppTheme.StyleGrid(dgv);
            return dgv;
        }

        private void UpdateMethodList()
        {
            int rows = (int)_numRows.Value;
            int cols = (int)_numCols.Value;
            _cbMethod.Items.Clear();
            _cbMethod.Items.Add("Метод Гаусса  (с шагами)");
            if (rows == cols)
            {
                _cbMethod.Items.Add("Метод Крамера");
                _cbMethod.Items.Add("Матричный метод  (A⁻¹·b)");
            }
            else
            {
                _cbMethod.Items.Add($"Метод Крамера  [только {cols}×{cols}]");
                _cbMethod.Items.Add($"Матричный метод  [только {cols}×{cols}]");
            }
            _cbMethod.SelectedIndex = 0;
        }

        private void ResizeGrids(int rows, int cols)
        {
            RebuildGrid(_dgvCoeffs, rows, cols, false);
            RebuildGrid(_dgvFree, rows, 1, true);
            UpdateMethodList();
        }

        private void RebuildGrid(DataGridView dgv, int rows, int cols, bool isFree)
        {
            dgv.Rows.Clear();
            dgv.Columns.Clear();
            for (int i = 0; i < cols; i++)
                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = isFree ? "b" : $"x{i + 1}",
                    Width = 58,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });
            for (int i = 0; i < rows; i++) dgv.Rows.Add();
            dgv.Width  = cols * 62 + 42;
            dgv.Height = rows * 28 + 30;
        }

        private void BtnCalc_Click(object? sender, EventArgs e)
        {
            try
            {
                var A = GetMatrix(_dgvCoeffs);
                var B = GetMatrix(_dgvFree);
                var b = new double[B.Rows];
                for (int i = 0; i < B.Rows; i++) b[i] = B[i, 0];

                string method = _cbMethod.SelectedItem?.ToString()?.Trim() ?? "";

                if (method.Contains('[') && _numRows.Value != _numCols.Value)
                {
                    ShowError("Этот метод применим только к квадратным системам. Используйте метод Гаусса.");
                    return;
                }

                string result = method switch
                {
                    var m when m.StartsWith("Метод Гаусса") => SlaeSolver.SolveGaussWithSteps(A, b),
                    "Метод Крамера"                          => FormatSolution(SlaeSolver.SolveCramer(A, b)),
                    var m when m.StartsWith("Матричный")     => FormatSolution(SlaeSolver.SolveInverse(A, b)),
                    _                                         => SlaeSolver.SolveGaussWithSteps(A, b)
                };

                _txtResult.ForeColor = AppTheme.Accent;
                _txtResult.Text = result;
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private string FormatSolution(double[] x)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Решение системы:\n");
            for (int i = 0; i < x.Length; i++)
                sb.AppendLine($"  x{i + 1}  =  {x[i]:F6}");
            return sb.ToString();
        }

        private void ShowError(string msg)
        {
            _txtResult.ForeColor = AppTheme.Danger;
            _txtResult.Text = "⚠  " + msg;
        }

        private Calculator.Core.Matrix GetMatrix(DataGridView dgv)
        {
            int r = dgv.Rows.Count, c = dgv.Columns.Count;
            var m = new Calculator.Core.Matrix(r, c);
            for (int i = 0; i < r; i++)
                for (int j = 0; j < c; j++)
                    m[i, j] = double.Parse(dgv.Rows[i].Cells[j].Value?.ToString() ?? "0");
            return m;
        }
    }
}
