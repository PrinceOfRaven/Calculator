using System;
using System.Drawing;
using System.Windows.Forms;
using Calculator.VectorAlgebra;

namespace MatrixCalculator.UI
{
    public class VectorTab : TabPage
    {
        private TextBox[] _txtVecA = new TextBox[3];
        private TextBox[] _txtVecB = new TextBox[3];
        private TextBox[] _txtVecC = new TextBox[3];
        private ComboBox _cbVecOp;
        private Button _btnCalcVec;
        private Button[] _btnVecLen = new Button[3];
        private Button[] _btnVecNorm = new Button[3];
        private TextBox _txtVecResult;
        private ChartPanel _chartPanel;

        public VectorTab() : base("Векторная алгебра")
        {
            AutoScroll = true;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            int startY = 20;
            int groupHeight = 140;
            int groupWidth = 240;
            int gapX = 260;

            var grpA = new GroupBox { Text = "Вектор A", Location = new Point(20, startY), Width = groupWidth, Height = groupHeight };
            CreateVectorControls(grpA, _txtVecA, 0);

            var grpB = new GroupBox { Text = "Вектор B", Location = new Point(20 + gapX, startY), Width = groupWidth, Height = groupHeight };
            CreateVectorControls(grpB, _txtVecB, 1);

            var grpC = new GroupBox { Text = "Вектор C", Location = new Point(20 + gapX * 2, startY), Width = groupWidth, Height = groupHeight };
            CreateVectorControls(grpC, _txtVecC, 2);

            _cbVecOp = new ComboBox
            {
                Location = new Point(20, startY + groupHeight + 20),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cbVecOp.Items.AddRange(new object[] {
                "Скалярное произведение (A·B)",
                "Векторное произведение [A×B]",
                "Смешанное произведение (A,[B×C])",
                "Угол между векторами (A,B)",
                "Площадь параллелограмма",
                "Объем параллелепипеда"
            });
            _cbVecOp.SelectedIndex = 0;

            _btnCalcVec = new Button { Text = "Вычислить", Location = new Point(340, startY + groupHeight + 18), Width = 150, Height = 35 };
            _btnCalcVec.Click += BtnCalcVec_Click;

            _txtVecResult = new TextBox
            {
                Location = new Point(20, startY + groupHeight + 70),
                Width = 700,
                Height = 100,
                Multiline = true,
                ReadOnly = true
            };

            _chartPanel = new ChartPanel 
            { 
                Location = new Point(20, startY + groupHeight + 180), 
                Width = 700, 
                Height = 300, 
                BorderStyle = BorderStyle.FixedSingle 
            };

            Controls.AddRange(new Control[] {
                grpA, grpB, grpC, _cbVecOp, _btnCalcVec, _txtVecResult, _chartPanel
            });
        }

        private void CreateVectorControls(GroupBox grp, TextBox[] boxes, int vecIndex)
        {
            for (int i = 0; i < 3; i++)
            {
                var lbl = new Label { Text = $"{(char)('X' + i)}:", Location = new Point(15, 25 + i * 30), Width = 20 };
                boxes[i] = new TextBox { Location = new Point(40, 22 + i * 30), Width = 60, Text = "0" };
                grp.Controls.Add(lbl);
                grp.Controls.Add(boxes[i]);
            }

            var btnLen = new Button { Text = "Длина", Location = new Point(120, 25), Width = 100, Height = 30 };
            var btnNorm = new Button { Text = "Норм.", Location = new Point(120, 65), Width = 100, Height = 30 };

            btnLen.Click += (s, e) => CalcVecLength(vecIndex);
            btnNorm.Click += (s, e) => NormalizeVec(vecIndex);

            _btnVecLen[vecIndex] = btnLen;
            _btnVecNorm[vecIndex] = btnNorm;

            grp.Controls.Add(btnLen);
            grp.Controls.Add(btnNorm);
        }

        private void BtnCalcVec_Click(object? sender, EventArgs e)
        {
            try
            {
                var a = GetVec(_txtVecA);
                var b = GetVec(_txtVecB);
                var c = GetVec(_txtVecC);
                string op = _cbVecOp.SelectedItem?.ToString() ?? "";
                string res = "";

                if (op.Contains("Скалярное")) res = $"A·B = {Vector3D.Dot(a, b):F4}";
                else if (op.Contains("Векторное"))
                {
                    var v = Vector3D.Cross(a, b);
                    res = $"[A×B] = ({v.X:F2}, {v.Y:F2}, {v.Z:F2})";
                    _chartPanel.DrawVector(v, Color.Red);
                }
                else if (op.Contains("Смешанное")) res = $"V = {Vector3D.TripleProduct(a, b, c):F4}";
                else if (op.Contains("Угол")) res = $"Angle = {Vector3D.AngleBetween(a, b):F2} rad ({Vector3D.AngleBetweenDegrees(a, b):F2}°)";
                else if (op.Contains("Площадь")) res = $"Area = {Vector3D.Cross(a, b).Length:F4}";
                else if (op.Contains("Объем")) res = $"Vol = {Math.Abs(Vector3D.TripleProduct(a, b, c)):F4}";

                _txtVecResult.Text = res;
                _chartPanel.DrawVector(a, Color.Blue);
                _chartPanel.DrawVector(b, Color.Green);
            }
            catch (Exception ex) { _txtVecResult.Text = "Ошибка: " + ex.Message; }
        }

        private void CalcVecLength(int index)
        {
            TextBox[] box = index == 0 ? _txtVecA : (index == 1 ? _txtVecB : _txtVecC);
            var v = GetVec(box);
            MessageBox.Show($"Длина вектора {(index == 0 ? "A" : (index == 1 ? "B" : "C"))}: {v.Length:F4}", "Результат");
        }

        private void NormalizeVec(int index)
        {
            try
            {
                TextBox[] box = index == 0 ? _txtVecA : (index == 1 ? _txtVecB : _txtVecC);
                var v = GetVec(box);
                var n = v.Normalize();
                box[0].Text = n.X.ToString("F4");
                box[1].Text = n.Y.ToString("F4");
                box[2].Text = n.Z.ToString("F4");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private Vector3D GetVec(TextBox[] boxes)
        {
            return new Vector3D(
                double.Parse(boxes[0].Text),
                double.Parse(boxes[1].Text),
                double.Parse(boxes[2].Text)
            );
        }

        public ChartPanel ChartPanel => _chartPanel;
    }
}
