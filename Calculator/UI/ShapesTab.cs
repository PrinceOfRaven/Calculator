using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Calculator.Shapes3D;
using Calculator.VectorAlgebra;

namespace MatrixCalculator.UI
{
    public class ShapesTab : TabPage
    {
        private ComboBox _cbShapeType;
        private DataGridView _dgvPoints;
        private Button _btnCalc;
        private TextBox _txtResult;
        private ChartPanel _chartPanel;

        public ShapesTab() : base("3D Фигуры")
        {
            BackColor = AppTheme.Background;
            AutoScroll = true;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ── Shape selector ─────────────────────────────────────────────────
            Controls.Add(AppTheme.MakeSectionLabel("Тип фигуры", 24, 20));

            _cbShapeType = new ComboBox
            {
                Location = new Point(24, 40),
                Width = 280,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = AppTheme.InputBg,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.MonoFont,
                FlatStyle = FlatStyle.Flat
            };
            _cbShapeType.Items.AddRange(new object[] {
                "Пирамида  (4 вершины)",
                "Куб       (8 вершин)"
            });
            _cbShapeType.SelectedIndex = 0;
            _cbShapeType.SelectedIndexChanged += (s, e) => BuildPointsGrid();

            // ── Points grid ────────────────────────────────────────────────────
            Controls.Add(AppTheme.MakeSectionLabel("Координаты вершин", 24, 82));

            _dgvPoints = new DataGridView
            {
                Location = new Point(24, 102),
                Width = 380,
                Height = 160,
                AllowUserToAddRows = false,
                RowHeadersVisible = true,
                ScrollBars = ScrollBars.Vertical
            };
            _dgvPoints.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Вершина", Width = 100, ReadOnly = true, SortMode = DataGridViewColumnSortMode.NotSortable });
            _dgvPoints.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "X", Width = 70, SortMode = DataGridViewColumnSortMode.NotSortable });
            _dgvPoints.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Y", Width = 70, SortMode = DataGridViewColumnSortMode.NotSortable });
            _dgvPoints.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Z", Width = 70, SortMode = DataGridViewColumnSortMode.NotSortable });
            AppTheme.StyleGrid(_dgvPoints);

            _btnCalc = AppTheme.MakePrimaryButton("▶  ПОСТРОИТЬ И РАССЧИТАТЬ", 24, 276, 380, 42);
            _btnCalc.Click += BtnCalc_Click;

            // ── Results ────────────────────────────────────────────────────────
            var resultCard = AppTheme.MakeCard(420, 40, 480, 280, "Параметры фигуры");
            _txtResult = AppTheme.MakeResultBox(12, 30, 454, 240);
            resultCard.Controls.Add(_txtResult);

            // ── Chart ──────────────────────────────────────────────────────────
            Controls.Add(AppTheme.MakeSectionLabel("Проекция (XY)", 24, 336));
            _chartPanel = new ChartPanel
            {
                Location = new Point(24, 356),
                Width = 876,
                Height = 280,
                BackColor = AppTheme.Surface
            };

            Controls.AddRange(new Control[] {
                _cbShapeType, _dgvPoints, _btnCalc, resultCard, _chartPanel
            });

            BuildPointsGrid();
        }

        private void BuildPointsGrid()
        {
            _dgvPoints.Rows.Clear();
            if (_cbShapeType.SelectedItem?.ToString()?.Contains("Пирамида") == true)
            {
                AddRow("A (основание)");
                AddRow("B (основание)");
                AddRow("C (основание)");
                AddRow("S (вершина)");
            }
            else
            {
                for (int i = 1; i <= 8; i++) AddRow($"V{i}");
            }
        }

        private void AddRow(string name)
        {
            int idx = _dgvPoints.Rows.Add(name, "0", "0", "0");
            _dgvPoints.Rows[idx].Cells[0].ReadOnly = true;
        }

        private void BtnCalc_Click(object? sender, EventArgs e)
        {
            try
            {
                var points = new List<Vector3D>();
                foreach (DataGridViewRow row in _dgvPoints.Rows)
                {
                    if (row.IsNewRow) continue;
                    double x = Parse(row.Cells[1]);
                    double y = Parse(row.Cells[2]);
                    double z = Parse(row.Cells[3]);
                    points.Add(new Vector3D(x, y, z));
                }

                string type = _cbShapeType.SelectedItem?.ToString() ?? "";
                string res = "";
                _chartPanel.Clear();

                if (type.Contains("Пирамида"))
                {
                    if (points.Count < 4) throw new Exception("Требуется 4 вершины.");
                    var pyr = new Pyramid(points[0], points[1], points[2], points[3]);

                    res  = "── Пирамида ──────────────────────────\n\n";
                    res += $"  Объём           =  {pyr.Volume:F6}\n";
                    res += $"  Площадь основ.  =  {pyr.BaseArea:F6}\n";
                    res += $"  Высота          =  {pyr.Height:F6}\n\n";
                    res += "── Площади граней ───────────────────\n\n";
                    res += $"  Основание ABC   =  {pyr.GetFaceArea(0, 1, 2):F6}\n";
                    res += $"  Грань ASB       =  {pyr.GetFaceArea(0, 3, 1):F6}\n";
                    res += $"  Грань BSC       =  {pyr.GetFaceArea(1, 3, 2):F6}\n";
                    res += $"  Грань ASC       =  {pyr.GetFaceArea(0, 3, 2):F6}\n";

                    DrawLine(pyr.A, pyr.B); DrawLine(pyr.B, pyr.C); DrawLine(pyr.C, pyr.A);
                    DrawLine(pyr.S, pyr.A); DrawLine(pyr.S, pyr.B); DrawLine(pyr.S, pyr.C);
                }
                else if (type.Contains("Куб"))
                {
                    if (points.Count < 8) throw new Exception("Требуется 8 вершин.");
                    var v1 = points[1] - points[0];
                    var v2 = points[3] - points[0];
                    var v3 = points[4] - points[0];
                    double vol = Math.Abs(Vector3D.TripleProduct(v1, v2, v3));
                    res  = "── Куб ───────────────────────────────\n\n";
                    res += $"  Объём (прибл.)  =  {vol:F6}\n\n";
                    res += "  Убедитесь, что вершины введены\n  в правильном порядке обхода.";

                    DrawLine(points[0], points[1]); DrawLine(points[1], points[2]);
                    DrawLine(points[2], points[3]); DrawLine(points[3], points[0]);
                    DrawLine(points[4], points[5]); DrawLine(points[5], points[6]);
                    DrawLine(points[6], points[7]); DrawLine(points[7], points[4]);
                    DrawLine(points[0], points[4]); DrawLine(points[1], points[5]);
                    DrawLine(points[2], points[6]); DrawLine(points[3], points[7]);
                }

                _txtResult.ForeColor = AppTheme.Accent;
                _txtResult.Text = res;
            }
            catch (Exception ex)
            {
                _txtResult.ForeColor = AppTheme.Danger;
                _txtResult.Text = "⚠  " + ex.Message;
            }
        }

        private double Parse(DataGridViewCell c) => double.Parse(c.Value?.ToString() ?? "0");
        private void DrawLine(Vector3D a, Vector3D b) => _chartPanel.DrawLine(a, b, AppTheme.Accent);
    }
}
