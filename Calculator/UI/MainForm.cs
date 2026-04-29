using Calculator.Core;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MatrixCalculator.UI
{
    public class MainForm : Form
    {
        private TabControl tabControl;
        private MatrixTab _matrixTab;
        private SlaeTab _slaeTab;
        private VectorTab _vectorTab;
        private BasisTab _basisTab;
        private ShapesTab _shapesTab;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Матричный Калькулятор v2.0";
            this.Size = new Size(1400, 950);
            this.StartPosition = FormStartPosition.CenterScreen;

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F)
            };

            _matrixTab = new MatrixTab();
            _slaeTab = new SlaeTab();
            _vectorTab = new VectorTab();
            _basisTab = new BasisTab();
            _shapesTab = new ShapesTab();

            tabControl.TabPages.AddRange(new TabPage[] { 
                _matrixTab, _slaeTab, _vectorTab, _basisTab, _shapesTab 
            });
            this.Controls.Add(tabControl);
        }
    }
}
