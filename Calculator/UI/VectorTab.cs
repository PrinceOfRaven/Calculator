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
        private ComboBox _cbOp;
        private Button _btnCalc;
        private TextBox _txtResult;
        private ChartPanel _chart;

        public VectorTab() : base("Векторы")
        {
            BackColor = AppTheme.Background;
            AutoScroll = true;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ── Header ─────────────────────────────────────────────────────────
            var lblTitle = AppTheme.MakeSectionLabel("Операции с трёхмерными векторами", AppTheme.PadLeft, 22);
            lblTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblTitle.ForeColor = AppTheme.Accent;

            // ── Vector cards ───────────────────────────────────────────────────
            var cardA = BuildVectorCard("Вектор  A", AppTheme.PadLeft, 60, _txtVecA, 0, Color.FromArgb(0, 200, 255));
            var cardB = BuildVectorCard("Вектор  B", AppTheme.PadLeft + 260, 60, _txtVecB, 1, AppTheme.Accent);
            var cardC = BuildVectorCard("Вектор  C", AppTheme.PadLeft + 520, 60, _txtVecC, 2, Color.FromArgb(180, 140, 255));

            // ── Operation selector ─────────────────────────────────────────────
            var lblOp = AppTheme.MakeSectionLabel("Операция", AppTheme.PadLeft, 280);

            _cbOp = new ComboBox
            {
                Location = new Point(AppTheme.PadLeft, 304),
                Width = 520,
                Height = AppTheme.InputH,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = AppTheme.InputBg,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.MonoFont,
                FlatStyle = FlatStyle.Flat
            };
            _cbOp.Items.AddRange(new object[] {
                "Скалярное произведение    A · B",
                "Векторное произведение    A × B",
                "Смешанное произведение    (A, [B×C])",
                "Угол между векторами      ∠(A, B)",
                "Площадь параллелограмма   |A × B|",
                "Объём параллелепипеда     |A · (B×C)|"
            });
            _cbOp.SelectedIndex = 0;

            _btnCalc = AppTheme.MakePrimaryButton("▶  ВЫЧИСЛИТЬ", AppTheme.PadLeft, 356, 520, 44);
            _btnCalc.Click += BtnCalc_Click;

            // ── Result card ────────────────────────────────────────────────────
            var resultCard = AppTheme.MakeCard(AppTheme.PadLeft, 416, 760, 130, "Результат");
            _txtResult = AppTheme.MakeResultBox(14, 38, 728, 78);
            resultCard.Controls.Add(_txtResult);

            // ── Visualization ──────────────────────────────────────────────────
            var lblViz = AppTheme.MakeSectionLabel("Визуализация  (проекция на плоскость XY)", AppTheme.PadLeft, 562);

            _chart = new ChartPanel
            {
                Location = new Point(AppTheme.PadLeft, 586),
                Width = 960,
                Height = 340,
                BackColor = AppTheme.Surface
            };

            Controls.AddRange(new Control[] {
                lblTitle, cardA, cardB, cardC,
                lblOp, _cbOp, _btnCalc, resultCard, lblViz, _chart
            });
        }

        private CardPanel BuildVectorCard(string title, int x, int y, TextBox[] boxes, int vecIndex, Color accentColor)
        {
            var card = new CardPanel(title)
            {
                Location = new Point(x, y),
                Size = new Size(242, 210)
            };

            string[] axes = { "X", "Y", "Z" };
            for (int i = 0; i < 3; i++)
            {
                var lbl = new Label
                {
                    Text = axes[i],
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = accentColor,
                    AutoSize = true,
                    Location = new Point(16, 46 + i * 42)
                };
                boxes[i] = new TextBox
                {
                    Location = new Point(46, 42 + i * 42),
                    Width = 140,
                    Height = AppTheme.InputH,
                    Text = "0",
                    BackColor = AppTheme.InputBg,
                    ForeColor = AppTheme.TextPrimary,
                    Font = AppTheme.MonoFont,
                    BorderStyle = BorderStyle.FixedSingle,
                    TextAlign = HorizontalAlignment.Center
                };
                card.Controls.Add(lbl);
                card.Controls.Add(boxes[i]);
            }

            // Action buttons
            var btnLen = new StyledButton(ButtonVariant.Secondary)
            {
                Text = "Длина  |v|",
                Location = new Point(14, 172),
                Size = new Size(100, 28),
                Font = new Font("Segoe UI", 8.5F)
            };
            var btnNorm = new StyledButton(ButtonVariant.Secondary)
            {
                Text = "Норм.",
                Location = new Point(122, 172),
                Size = new Size(64, 28),
                Font = new Font("Segoe UI", 8.5F)
            };
            btnLen.Click += (s, e) => ShowLength(vecIndex);
            btnNorm.Click += (s, e) => Normalize(vecIndex);

            card.Controls.Add(btnLen);
            card.Controls.Add(btnNorm);
            return card;
        }

        private void BtnCalc_Click(object? sender, EventArgs e)
        {
            try
            {
                var a = GetVec(_txtVecA);
                var b = GetVec(_txtVecB);
                var c = GetVec(_txtVecC);
                string op = _cbOp.SelectedItem?.ToString() ?? "";
                string res = "";

                _chart.Clear();

                if (op.Contains("Скалярное"))
                {
                    res = $"A · B  =  {Vector3D.Dot(a, b):F8}";
                }
                else if (op.Contains("Векторное"))
                {
                    var cross = Vector3D.Cross(a, b);
                    res = $"A × B  =  {cross}\n\nДлина: {cross.Length:F8}";
                    _chart.DrawVector(cross, Color.FromArgb(255, 100, 100));
                }
                else if (op.Contains("Смешанное"))
                {
                    res = $"(A, [B×C])  =  {Vector3D.TripleProduct(a, b, c):F8}";
                }
                else if (op.Contains("Угол"))
                {
                    double rad = Vector3D.AngleBetween(a, b);
                    double deg = Vector3D.AngleBetweenDegrees(a, b);
                    res = $"∠(A,B)  =  {rad:F6} рад  =  {deg:F3}°";
                }
                else if (op.Contains("Площадь"))
                {
                    double area = Vector3D.Cross(a, b).Length;
                    res = $"Площадь параллелограмма:\n|A × B|  =  {area:F8}";
                }
                else if (op.Contains("Объём"))
                {
                    double vol = Math.Abs(Vector3D.TripleProduct(a, b, c));
                    res = $"Объём параллелепипеда:\n|A · (B×C)|  =  {vol:F8}";
                }

                _txtResult.ForeColor = AppTheme.Accent;
                _txtResult.Text = res;

                // Draw input vectors on chart
                _chart.DrawVector(a, Color.FromArgb(0, 200, 255));
                _chart.DrawVector(b, AppTheme.Accent);
                _chart.DrawVector(c, Color.FromArgb(180, 140, 255));
            }
            catch (Exception ex)
            {
                _txtResult.ForeColor = AppTheme.Danger;
                _txtResult.Text = "⚠  " + ex.Message;
            }
        }

        private void ShowLength(int index)
        {
            var boxes = index == 0 ? _txtVecA : index == 1 ? _txtVecB : _txtVecC;
            var v = GetVec(boxes);
            string name = (char)('A' + index) + "";
            MessageBox.Show(
                $"|{name}|  =  {v.Length:F8}",
                $"Длина вектора {name}",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void Normalize(int index)
        {
            try
            {
                var boxes = index == 0 ? _txtVecA : index == 1 ? _txtVecB : _txtVecC;
                var n = GetVec(boxes).Normalize();
                boxes[0].Text = n.X.ToString("F8");
                boxes[1].Text = n.Y.ToString("F8");
                boxes[2].Text = n.Z.ToString("F8");
            }
            catch (Exception ex)
            {
                MessageBox.Show("⚠  " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Vector3D GetVec(TextBox[] b) =>
            new Vector3D(
                double.Parse(b[0].Text),
                double.Parse(b[1].Text),
                double.Parse(b[2].Text)
            );

        public ChartPanel ChartPanel => _chart;
    }
}
