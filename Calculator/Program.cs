namespace Calculator
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MatrixCalculator.UI.MainForm());
        }
    }
}
