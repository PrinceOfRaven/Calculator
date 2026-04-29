using System;
using System.Text;

namespace Calculator.Core
{
    public class SlaeSolver
    {
        public static string SolveGaussWithSteps(Matrix A, double[] b)
        {
            int rows = A.Rows, cols = A.Cols;
            var sb = new StringBuilder();

            if (rows != b.Length)
                throw new ArgumentException("Количество уравнений должно совпадать с размером вектора свободных членов");

            sb.AppendLine("=== Решение системы методом Гаусса ===\n");
            sb.AppendLine($"Система: {rows} уравнений, {cols} неизвестных\n");

            var aug = new double[rows, cols + 1];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++) aug[i, j] = A[i, j];
                aug[i, cols] = b[i];
            }

            sb.AppendLine("Расширенная матрица:");
            PrintMatrix(sb, aug, cols);
            sb.AppendLine();
            sb.AppendLine("--- Прямой ход ---\n");

            int pivotRow = 0;
            int[] pivotCols = new int[Math.Min(rows, cols)];
            int pivotCount = 0;

            for (int col = 0; col < cols && pivotRow < rows; col++)
            {
                int maxRow = pivotRow;
                for (int k = pivotRow + 1; k < rows; k++)
                    if (Math.Abs(aug[k, col]) > Math.Abs(aug[maxRow, col])) maxRow = k;

                if (Math.Abs(aug[maxRow, col]) < 1e-10) continue;

                if (maxRow != pivotRow)
                {
                    sb.AppendLine($"Меняем строки {pivotRow + 1} ↔ {maxRow + 1}");
                    for (int j = 0; j <= cols; j++) { double t = aug[pivotRow, j]; aug[pivotRow, j] = aug[maxRow, j]; aug[maxRow, j] = t; }
                    PrintMatrix(sb, aug, cols);
                    sb.AppendLine();
                }

                pivotCols[pivotCount++] = col;

                double pivot = aug[pivotRow, col];
                sb.AppendLine($"Делим строку {pivotRow + 1} на {pivot:F4}");
                for (int j = col; j <= cols; j++) aug[pivotRow, j] /= pivot;
                PrintMatrix(sb, aug, cols);
                sb.AppendLine();

                for (int k = 0; k < rows; k++)
                {
                    if (k != pivotRow && Math.Abs(aug[k, col]) > 1e-10)
                    {
                        double factor = aug[k, col];
                        sb.AppendLine($"Строка {k + 1} -= {factor:F4} × строка {pivotRow + 1}");
                        for (int j = col; j <= cols; j++) aug[k, j] -= factor * aug[pivotRow, j];
                        PrintMatrix(sb, aug, cols);
                        sb.AppendLine();
                    }
                }
                pivotRow++;
            }

            sb.AppendLine("--- Ступенчатый вид ---");
            PrintMatrix(sb, aug, cols);
            sb.AppendLine();

            for (int i = 0; i < rows; i++)
            {
                bool allZero = true;
                for (int j = 0; j < cols; j++)
                    if (Math.Abs(aug[i, j]) > 1e-10) { allZero = false; break; }
                if (allZero && Math.Abs(aug[i, cols]) > 1e-10)
                    throw new Exception("Система несовместна — решений нет");
            }

            int rank = pivotCount;
            int freeVars = cols - rank;

            if (freeVars > 0)
            {
                sb.AppendLine($"=== Результат ===");
                sb.AppendLine($"Ранг матрицы: {rank}  |  Неизвестных: {cols}  |  Свободных: {freeVars}");
                sb.AppendLine("\n** Система имеет бесконечно много решений **\n");

                bool[] isFree = new bool[cols];
                for (int j = 0; j < cols; j++)
                {
                    isFree[j] = true;
                    for (int p = 0; p < pivotCount; p++)
                        if (pivotCols[p] == j) { isFree[j] = false; break; }
                }

                double[] particular = new double[cols];
                for (int p = pivotCount - 1; p >= 0; p--)
                {
                    int r = p, c = pivotCols[p];
                    double sum = aug[r, cols];
                    for (int j = c + 1; j < cols; j++)
                        if (!isFree[j]) sum -= aug[r, j] * particular[j];
                    particular[c] = sum;
                }

                sb.AppendLine("Частное решение (свободные = 0):");
                for (int i = 0; i < cols; i++)
                    sb.AppendLine(isFree[i] ? $"  x{i + 1} = 0  (свободная)" : $"  x{i + 1} = {particular[i]:F4}");

                sb.AppendLine("\nОбщее решение:");
                for (int p = pivotCount - 1; p >= 0; p--)
                {
                    int r = p, c = pivotCols[p];
                    var expr = new StringBuilder($"  x{c + 1} = {aug[r, cols]:F4}");
                    for (int j = c + 1; j < cols; j++)
                    {
                        if (!isFree[j]) continue;
                        double coeff = -aug[r, j];
                        expr.Append(coeff >= 0 ? $" + {coeff:F4}·x{j + 1}" : $" - {Math.Abs(coeff):F4}·x{j + 1}");
                    }
                    sb.AppendLine(expr.ToString());
                }
            }
            else
            {
                double[] x = new double[cols];
                for (int i = 0; i < cols; i++)
                    x[i] = (i < rows && Math.Abs(aug[i, i]) > 1e-10) ? aug[i, cols] : 0;

                sb.AppendLine("=== Результат: единственное решение ===");
                for (int i = 0; i < cols; i++)
                    sb.AppendLine($"  x{i + 1} = {x[i]:F6}");
            }

            return sb.ToString();
        }

        public static double[] SolveGauss(Matrix A, double[] b)
        {
            int rows = A.Rows, cols = A.Cols;
            var aug = new double[rows, cols + 1];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++) aug[i, j] = A[i, j];
                aug[i, cols] = b[i];
            }

            for (int i = 0; i < Math.Min(rows, cols); i++)
            {
                int maxRow = i;
                for (int k = i + 1; k < rows; k++)
                    if (Math.Abs(aug[k, i]) > Math.Abs(aug[maxRow, i])) maxRow = k;
                if (maxRow != i)
                    for (int j = 0; j <= cols; j++) { double t = aug[i, j]; aug[i, j] = aug[maxRow, j]; aug[maxRow, j] = t; }
                if (Math.Abs(aug[i, i]) < 1e-10) continue;
                for (int j = i; j <= cols; j++) aug[i, j] /= aug[i, i];
                for (int k = 0; k < rows; k++)
                {
                    if (k == i || Math.Abs(aug[k, i]) < 1e-10) continue;
                    double f = aug[k, i];
                    for (int j = i; j <= cols; j++) aug[k, j] -= f * aug[i, j];
                }
            }

            for (int i = 0; i < rows; i++)
            {
                bool z = true;
                for (int j = 0; j < cols; j++) if (Math.Abs(aug[i, j]) > 1e-10) { z = false; break; }
                if (z && Math.Abs(aug[i, cols]) > 1e-10) throw new Exception("Система несовместна");
            }

            var x = new double[cols];
            for (int i = 0; i < cols; i++)
                x[i] = (i < rows && Math.Abs(aug[i, i]) > 1e-10) ? aug[i, cols] : 0;
            return x;
        }

        public static double[] SolveCramer(Matrix A, double[] b)
        {
            int n = A.Rows;
            if (n != A.Cols) throw new ArgumentException("Метод Крамера применим только к квадратным системам");
            double detA = A.Determinant();
            if (Math.Abs(detA) < 1e-10) throw new Exception("det(A) = 0, метод Крамера неприменим");

            var x = new double[n];
            for (int i = 0; i < n; i++)
            {
                var data = A.GetData();
                for (int row = 0; row < n; row++) data[row, i] = b[row];
                x[i] = new Matrix(data).Determinant() / detA;
            }
            return x;
        }

        public static double[] SolveInverse(Matrix A, double[] b)
        {
            if (A.Rows != A.Cols) throw new ArgumentException("Матрица должна быть квадратной");
            var inv = A.Inverse();
            var x = new double[A.Rows];
            for (int i = 0; i < A.Rows; i++)
                for (int j = 0; j < A.Rows; j++)
                    x[i] += inv[i, j] * b[j];
            return x;
        }

        private static void PrintMatrix(StringBuilder sb, double[,] m, int cols)
        {
            int rows = m.GetLength(0);
            for (int i = 0; i < rows; i++)
            {
                sb.Append("│ ");
                for (int j = 0; j < cols; j++) sb.Append($"{m[i, j],9:F4} ");
                sb.AppendLine($"│ {m[i, cols],9:F4}");
            }
        }
    }
}
