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
        private TextBox _txtVecResult;
        private ChartPanel _chartPanel;

        public VectorTab() : base("Векторы")
        {
            BackColor = AppTheme.Background;
            AutoScroll = true;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Controls.Add(AppTheme.MakeSectionLabel("Входные векторы", 24, 20));

            // ── Vector input cards ─────────────────────────────────────────────
            var cardA = BuildVectorCard("Вектор  A", 24, 44, _txtVecA, 0, Color.FromArgb(0, 180, 230));
            var cardB = BuildVectorCard("Вектор  B", 230, 44, _txtVecB, 1, AppTheme.Accent);
            var cardC = BuildVectorCard("Вектор  C", 436, 44, _txtVecC, 2, Color.FromArgb(160, 120, 255));

            // ── Operation ─────────────────────────────────────────────────────
            Controls.Add(AppTheme.MakeSectionLabel("Операция", 24, 230));

            _cbVecOp = new ComboBox
            {
                Location = new Point(24, 250),
                Width = 380,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = AppTheme.InputBg,
                ForeColor = AppTheme.TextPrimary,
                Font = AppTheme.MonoFont,
                FlatStyle = FlatStyle.Flat
            };
            _cbVecOp.Items.AddRange(new object[] {
                "Скалярное произведение   A · B",
                "Векторное произведение   A × B",
                "Смешанное произведение   (A,[B×C])",
                "Угол между векторами     ∠(A,B)",
                "Площадь параллелограмма  |A×B|",
                "Объём параллелепипеда    |A·(B×C)|"
            });
            _cbVecOp.SelectedIndex = 0;

            _btnCalcVec = AppTheme.MakePrimaryButton("▶  ВЫЧИСЛИТЬ", 24, 300, 380, 42);
            _btnCalcVec.Click += BtnCalcVec_Click;

            // ── Result ─────────────────────────────────────────────────────────
            var resultCard = AppTheme.MakeCard(24, 360, 680, 110, "Результат");
            _txtVecResult = AppTheme.MakeResultBox(12, 30, 654, 72);
            resultCard.Controls.Add(_txtVecResult);

            // ── Chart ──────────────────────────────────────────────────────────
            Controls.Add(AppTheme.MakeSectionLabel("Визуализация (проекция XY)", 24, 486));
            _chartPanel = new ChartPanel
            {
                Location = new Point(24, 506),
                Width = 680,
                Height = 280,
                BackColor = AppTheme.Surface
            };

            Controls.AddRange(new Control[] {
                cardA, cardB, cardC,
                _cbVecOp, _btnCalcVec, resultCard, _chartPanel
            });
        }

        private CardPanel BuildVectorCard(string title, int x, int y, TextBox[] boxes, int vecIndex, Color accentColor)
        {
            var card = new CardPanel(title)
            {
                Location = new Point(x, y),
                Size = new Size(196, 180)
            };

            string[] axes = { "X", "Y", "Z" };
            for (int i = 0; i < 3; i++)
            {
                var lbl = new Label
                {
                    Text = axes[i],
                    Font = new Font("Consolas", 9F, FontStyle.Bold),
                    ForeColor = accentColor,
                    AutoSize = true,
                    Location = new Point(12, 38 + i * 38)
                };
                boxes[i] = new TextBox
                {
                    Location = new Point(36, 34 + i * 38),
                    Width = 100,
                    Text = "0",
                    BackColor = AppTheme.InputBg,
                    ForeColor = AppTheme.TextPrimary,
                    Font = AppTheme.MonoFont,
                    BorderStyle = BorderStyle.FixedSingle
                };
                card.Controls.Add(lbl);
                card.Controls.Add(boxes[i]);
            }

            var btnLen = new StyledButton(ButtonVariant.Secondary)
            {
                Text = "|v|",
                Location = new Point(12, 148),
                Size = new Size(56, 24),
                Font = new Font("Consolas", 8F)
            };
            var btnNorm = new StyledButton(ButtonVariant.Secondary)
            {
                Text = "norm",
                Location = new Point(76, 148),
                Size = new Size(60, 24),
                Font = new Font("Consolas", 8F)
            };
            btnLen.Click += (s, e) => ShowLength(vecIndex);
            btnNorm.Click += (s, e) => Normalize(vecIndex);

            card.Controls.Add(btnLen);
            card.Controls.Add(btnNorm);
            return card;
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

                _chartPanel.Clear();

                if (op.Contains("Скалярное"))      res = $"A · B  =  {Vector3D.Dot(a, b):F6}";
                else if (op.Contains("Векторное"))
                {
                    var v = Vector3D.Cross(a, b);
                    res = $"A × B  =  {v}";
                    _chartPanel.DrawVector(v, Color.FromArgb(255, 80, 80));
                }
                else if (op.Contains("Смешанное")) res = $"(A,[B×C])  =  {Vector3D.TripleProduct(a, b, c):F6}";
                else if (op.Contains("Угол"))      res = $"∠(A,B)  =  {Vector3D.AngleBetween(a, b):F4} рад  ({Vector3D.AngleBetweenDegrees(a, b):F2}°)";
                else if (op.Contains("Площадь"))   res = $"|A×B|  =  {Vector3D.Cross(a, b).Length:F6}";
                else if (op.Contains("Объём"))     res = $"|A·(B×C)|  =  {Math.Abs(Vector3D.TripleProduct(a, b, c)):F6}";

                _txtVecResult.ForeColor = AppTheme.Accent;
                _txtVecResult.Text = res;

                _chartPanel.DrawVector(a, Color.FromArgb(0, 180, 230));
                _chartPanel.DrawVector(b, AppTheme.Accent);
                _chartPanel.DrawVector(c, Color.FromArgb(160, 120, 255));
            }
            catch (Exception ex)
            {
                _txtVecResult.ForeColor = AppTheme.Danger;
                _txtVecResult.Text = "⚠  " + ex.Message;
            }
        }

        private void ShowLength(int index)
        {
            var v = GetVec(index == 0 ? _txtVecA : index == 1 ? _txtVecB : _txtVecC);
            MessageBox.Show($"|{(char)('A' + index)}|  =  {v.Length:F6}", "Длина вектора",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Normalize(int index)
        {
            try
            {
                var boxes = index == 0 ? _txtVecA : index == 1 ? _txtVecB : _txtVecC;
                var n = GetVec(boxes).Normalize();
                boxes[0].Text = n.X.ToString("F6");
                boxes[1].Text = n.Y.ToString("F6");
                boxes[2].Text = n.Z.ToString("F6");
            }
            catch (Exception ex) { MessageBox.Show("⚠  " + ex.Message); }
        }

        private Vector3D GetVec(TextBox[] b) =>
            new Vector3D(double.Parse(b[0].Text), double.Parse(b[1].Text), double.Parse(b[2].Text));

        public ChartPanel ChartPanel => _chartPanel;
    }
}
