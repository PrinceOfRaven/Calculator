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
        private DataGridView _dgvShapePoints;
        private Button _btnCalcShape;
        private TextBox _txtShapeResult;
        private ChartPanel _chartShapePanel;

        public ShapesTab() : base("3D Фигуры")
        {
            AutoScroll = true;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _cbShapeType = new ComboBox
            {
                Location = new Point(20, 20),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cbShapeType.Items.AddRange(new object[] { "Пирамида (4 точки)", "Куб (8 точек)" });
            _cbShapeType.SelectedIndex = 0;
            _cbShapeType.SelectedIndexChanged += CbShapeType_SelectedIndexChanged;

            var lblInstr = new Label
            {
                Text = "Введите координаты вершин фигуры. Для пирамиды: A, B, C (основание), S (вершина).",
                Location = new Point(20, 60),
                Width = 600,
                Height = 40
            };

            _dgvShapePoints = new DataGridView
            {
                Location = new Point(20, 110),
                Width = 400,
                Height = 250,
                AllowUserToAddRows = false,
                RowHeadersVisible = true
            };

            _dgvShapePoints.Columns.Add("Name", "Вершина");
            _dgvShapePoints.Columns.Add("X", "X");
            _dgvShapePoints.Columns.Add("Y", "Y");
            _dgvShapePoints.Columns.Add("Z", "Z");

            foreach (DataGridViewColumn col in _dgvShapePoints.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            _btnCalcShape = new Button { Text = "Построить и рассчитать", Location = new Point(20, 380), Width = 250, Height = 40 };
            _btnCalcShape.Click += BtnCalcShape_Click;

            _txtShapeResult = new TextBox
            {
                Location = new Point(450, 110),
                Width = 400,
                Height = 250,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            _chartShapePanel = new ChartPanel 
            { 
                Location = new Point(20, 440), 
                Width = 830, 
                Height = 300, 
                BorderStyle = BorderStyle.FixedSingle 
            };

            Controls.AddRange(new Control[] {
                _cbShapeType, lblInstr, _dgvShapePoints, _btnCalcShape, _txtShapeResult, _chartShapePanel
            });

            CbShapeType_SelectedIndexChanged(null, null);
        }

        private void CbShapeType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            _dgvShapePoints.Rows.Clear();
            if (_cbShapeType.SelectedItem?.ToString() == "Пирамида (4 точки)")
            {
                AddShapeRow("A (Основание)");
                AddShapeRow("B (Основание)");
                AddShapeRow("C (Основание)");
                AddShapeRow("S (Вершина)");
            }
            else
            {
                string[] names = { "1", "2", "3", "4", "5", "6", "7", "8" };
                foreach (var name in names) AddShapeRow($"Вершина {name}");
            }
        }

        private void AddShapeRow(string name)
        {
            int idx = _dgvShapePoints.Rows.Add(name, "0", "0", "0");
            for (int i = 1; i <= 3; i++) _dgvShapePoints.Rows[idx].Cells[i].ReadOnly = false;
        }

        private void BtnCalcShape_Click(object? sender, EventArgs e)
        {
            try
            {
                var points = new List<Vector3D>();
                foreach (DataGridViewRow row in _dgvShapePoints.Rows)
                {
                    if (row.IsNewRow) continue;
                    double x = double.Parse(row.Cells["X"].Value?.ToString() ?? "0");
                    double y = double.Parse(row.Cells["Y"].Value?.ToString() ?? "0");
                    double z = double.Parse(row.Cells["Z"].Value?.ToString() ?? "0");
                    points.Add(new Vector3D(x, y, z));
                }

                string type = _cbShapeType.SelectedItem?.ToString() ?? "";
                string res = "";

                _chartShapePanel.Clear();

                if (type.Contains("Пирамида"))
                {
                    if (points.Count < 4) throw new Exception("Нужно 4 точки!");
                    var pyr = new Pyramid(points[0], points[1], points[2], points[3]);

                    res = "=== ПИРАМИДА ===\n\n";
                    res += $"Объем:                  {pyr.Volume:F4}\n";
                    res += $"Площадь основания:      {pyr.BaseArea:F4}\n";
                    res += $"Высота:                 {pyr.Height:F4}\n";

                    // Грани
                    res += "\nГрани:\n";
                    res += $"  Основание (ABC):      {pyr.GetFaceArea(0, 1, 2):F4}\n";
                    res += $"  Грань ASB:            {pyr.GetFaceArea(0, 3, 1):F4}\n";
                    res += $"  Грань BSC:            {pyr.GetFaceArea(1, 3, 2):F4}\n";
                    res += $"  Грань ASC:            {pyr.GetFaceArea(0, 3, 2):F4}\n";

                    // Отрисовка ребер
                    DrawLine(pyr.A, pyr.B); DrawLine(pyr.B, pyr.C); DrawLine(pyr.C, pyr.A);
                    DrawLine(pyr.S, pyr.A); DrawLine(pyr.S, pyr.B); DrawLine(pyr.S, pyr.C);
                }
                else if (type.Contains("Куб"))
                {
                    if (points.Count < 8) throw new Exception("Нужно 8 точек!");
                    var v1 = points[1] - points[0];
                    var v2 = points[3] - points[0];
                    var v3 = points[4] - points[0];

                    double vol = Math.Abs(Vector3D.TripleProduct(v1, v2, v3));
                    res = $"Приближенный объем (по первым ребрам): {vol:F4}\n";
                    res += "Для точного расчета убедитесь, что вершины введены последовательно.";

                    // Отрисовка каркаса
                    DrawLine(points[0], points[1]); DrawLine(points[1], points[2]);
                    DrawLine(points[2], points[3]); DrawLine(points[3], points[0]);
                    DrawLine(points[4], points[5]); DrawLine(points[5], points[6]);
                    DrawLine(points[6], points[7]); DrawLine(points[7], points[4]);
                    DrawLine(points[0], points[4]); DrawLine(points[1], points[5]);
                    DrawLine(points[2], points[6]); DrawLine(points[3], points[7]);
                }

                _txtShapeResult.Text = res;
            }
            catch (Exception ex) { _txtShapeResult.Text = "Ошибка: " + ex.Message; }
        }

        private void DrawLine(Vector3D a, Vector3D b)
        {
            _chartShapePanel.DrawLine(a, b, Color.Black);
        }
    }
}
