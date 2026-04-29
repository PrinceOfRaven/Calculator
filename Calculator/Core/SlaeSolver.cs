using Calculator.Core;
using System;
using System.Text;

namespace Calculator.Core
{
    //Класс для решения систем линейных алгебраических уравнений (СЛАУ)
    public class SlaeSolver
    {
        //Решение СЛАУ методом Гаусса с выводом шагов
        //Возвращает строку с описанием решения
        public static string SolveGaussWithSteps(Matrix A, double[] b)
        {
            int rows = A.Rows;
            int cols = A.Cols;
            var sb = new StringBuilder();

            if (rows != b.Length)
                throw new ArgumentException("Количество уравнений должно совпадать с размером вектора свободных членов");

            sb.AppendLine("=== Решение системы методом Гаусса ===\n");
            sb.AppendLine($"Исходная система: {rows} уравнений, {cols} неизвестных\n");

            // Создаем расширенную матрицу [A|b]
            var augmented = new double[rows, cols + 1];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    augmented[i, j] = A[i, j];
                augmented[i, cols] = b[i];
            }

            sb.AppendLine("Расширенная матрица:");
            PrintMatrix(sb, augmented, cols);
            sb.AppendLine();

            // Прямой ход метода Гаусса
            sb.AppendLine("--- Прямой ход ---\n");
            int pivotRow = 0;
            int[] pivotCols = new int[Math.Min(rows, cols)];
            int pivotCount = 0;

            for (int col = 0; col < cols && pivotRow < rows; col++)
            {
                // Поиск главного элемента
                int maxRow = pivotRow;
                for (int k = pivotRow + 1; k < rows; k++)
                {
                    if (Math.Abs(augmented[k, col]) > Math.Abs(augmented[maxRow, col]))
                        maxRow = k;
                }

                // Если ведущий элемент ≈ 0, пропускаем столбец
                if (Math.Abs(augmented[maxRow, col]) < 1e-10)
                    continue;

                // Обмен строк
                if (maxRow != pivotRow)
                {
                    sb.AppendLine($"Шаг {pivotCount + 1}: Меняем местами строки {pivotRow + 1} и {maxRow + 1}");
                    for (int j = 0; j <= cols; j++)
                    {
                        double t = augmented[pivotRow, j];
                        augmented[pivotRow, j] = augmented[maxRow, j];
                        augmented[maxRow, j] = t;
                    }
                    PrintMatrix(sb, augmented, cols);
                    sb.AppendLine();
                }

                pivotCols[pivotCount] = col;
                pivotCount++;

                // Нормализация строки
                double pivot = augmented[pivotRow, col];
                sb.AppendLine($"Шаг {pivotCount}: Делим строку {pivotRow + 1} на {pivot:F4}");
                for (int j = col; j <= cols; j++)
                    augmented[pivotRow, j] /= pivot;
                PrintMatrix(sb, augmented, cols);
                sb.AppendLine();

                // Исключение из других строк
                for (int k = 0; k < rows; k++)
                {
                    if (k != pivotRow && Math.Abs(augmented[k, col]) > 1e-10)
                    {
                        double factor = augmented[k, col];
                        sb.AppendLine($"Шаг: Из строки {k + 1} вычитаем строку {pivotRow + 1}, умноженную на {factor:F4}");
                        for (int j = col; j <= cols; j++)
                            augmented[k, j] -= factor * augmented[pivotRow, j];
                        PrintMatrix(sb, augmented, cols);
                        sb.AppendLine();
                    }
                }

                pivotRow++;
            }

            sb.AppendLine("--- Ступенчатый вид ---");
            PrintMatrix(sb, augmented, cols);
            sb.AppendLine();

            // Проверка на совместность
            for (int i = 0; i < rows; i++)
            {
                bool allZero = true;
                for (int j = 0; j < cols; j++)
                {
                    if (Math.Abs(augmented[i, j]) > 1e-10)
                    {
                        allZero = false;
                        break;
                    }
                }
                if (allZero && Math.Abs(augmented[i, cols]) > 1e-10)
                    throw new Exception("Система несовместна (решений нет)");
            }

            // Определяем ранг и количество свободных переменных
            int rank = pivotCount;
            int freeVars = cols - rank;

            if (freeVars > 0)
            {
                sb.AppendLine($"=== Результат ===");
                sb.AppendLine($"Ранг матрицы системы: {rank}");
                sb.AppendLine($"Количество неизвестных: {cols}");
                sb.AppendLine($"Количество свободных переменных: {freeVars}");
                sb.AppendLine();
                sb.AppendLine("** Система имеет бесконечно много решений **");
                sb.AppendLine();

                // Формируем частное решение (свободные переменные = 0)
                double[] particularSolution = new double[cols];
                bool[] isFree = new bool[cols];

                // Помечаем свободные переменные
                for (int j = 0; j < cols; j++)
                {
                    isFree[j] = true;
                    for (int p = 0; p < pivotCount; p++)
                    {
                        if (pivotCols[p] == j)
                        {
                            isFree[j] = false;
                            break;
                        }
                    }
                }

                // Находим значения базисных переменных при свободных = 0
                for (int p = pivotCount - 1; p >= 0; p--)
                {
                    int row = p;
                    int col = pivotCols[p];

                    double sum = augmented[row, cols];
                    for (int j = col + 1; j < cols; j++)
                    {
                        if (!isFree[j])
                            sum -= augmented[row, j] * particularSolution[j];
                    }
                    particularSolution[col] = sum;
                }

                sb.AppendLine("Частное решение (при свободных переменных = 0):");
                for (int i = 0; i < cols; i++)
                {
                    string varName = $"x{i + 1}";
                    if (isFree[i])
                        sb.AppendLine($"{varName} = 0 (свободная)");
                    else
                        sb.AppendLine($"{varName} = {particularSolution[i]:F4}");
                }

                // Записываем общее решение
                sb.AppendLine();
                sb.AppendLine("Общее решение:");
                sb.AppendLine("Базисные переменные выражаются через свободные:");

                for (int p = pivotCount - 1; p >= 0; p--)
                {
                    int row = p;
                    int col = pivotCols[p];

                    var expr = new StringBuilder();
                    expr.Append($"x{col + 1} = {augmented[row, cols]:F4}");

                    for (int j = col + 1; j < cols; j++)
                    {
                        if (isFree[j])
                        {
                            double coeff = -augmented[row, j];
                            if (coeff >= 0)
                                expr.Append($" + {coeff:F4}*x{j + 1}");
                            else
                                expr.Append($" - {Math.Abs(coeff):F4}*x{j + 1}");
                        }
                    }
                    sb.AppendLine(expr.ToString());
                }
            }
            else
            {
                // Единственное решение
                double[] x = new double[cols];
                for (int i = 0; i < cols; i++)
                {
                    if (i < rows && Math.Abs(augmented[i, i]) > 1e-10)
                        x[i] = augmented[i, cols];
                    else
                        x[i] = 0;
                }

                sb.AppendLine("=== Результат ===");
                sb.AppendLine("Система имеет единственное решение:");
                for (int i = 0; i < cols; i++)
                    sb.AppendLine($"x{i + 1} = {x[i]:F4}");
            }

            return sb.ToString();
        }

        //Решение СЛАУ методом Гаусса (старая версия для совместимости)
        //Возвращает вектор решений или null если система несовместна
        public static double[] SolveGauss(Matrix A, double[] b)
        {
            int rows = A.Rows;
            int cols = A.Cols;

            if (rows != b.Length)
                throw new ArgumentException("Количество уравнений должно совпадать с размером вектора свободных членов");

            // Создаем расширенную матрицу [A|b]
            var augmented = new double[rows, cols + 1];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                    augmented[i, j] = A[i, j];
                augmented[i, cols] = b[i];
            }

            // Прямой ход метода Гаусса
            for (int i = 0; i < Math.Min(rows, cols); i++)
            {
                // Поиск главного элемента
                int maxRow = i;
                for (int k = i + 1; k < rows; k++)
                {
                    if (Math.Abs(augmented[k, i]) > Math.Abs(augmented[maxRow, i]))
                        maxRow = k;
                }

                // Обмен строк
                if (maxRow != i)
                {
                    for (int j = 0; j <= cols; j++)
                    {
                        double t = augmented[i, j];
                        augmented[i, j] = augmented[maxRow, j];
                        augmented[maxRow, j] = t;
                    }
                }

                // Если ведущий элемент = 0, пропускаем
                if (Math.Abs(augmented[i, i]) < 1e-10)
                    continue;

                // Нормализация строки
                for (int j = i; j <= cols; j++)
                    augmented[i, j] /= augmented[i, i];

                // Исключение из других строк
                for (int k = 0; k < rows; k++)
                {
                    if (k != i && Math.Abs(augmented[k, i]) > 1e-10)
                    {
                        double factor = augmented[k, i];
                        for (int j = i; j <= cols; j++)
                            augmented[k, j] -= factor * augmented[i, j];
                    }
                }
            }

            // Проверка на совместность
            for (int i = 0; i < rows; i++)
            {
                bool allZero = true;
                for (int j = 0; j < cols; j++)
                {
                    if (Math.Abs(augmented[i, j]) > 1e-10)
                    {
                        allZero = false;
                        break;
                    }
                }
                if (allZero && Math.Abs(augmented[i, cols]) > 1e-10)
                    throw new Exception("Система несовместна (решений нет)");
            }

            // Формируем результат
            double[] x = new double[cols];
            for (int i = 0; i < cols; i++)
            {
                if (i < rows && Math.Abs(augmented[i, i]) > 1e-10)
                    x[i] = augmented[i, cols];
                else
                    x[i] = 0; // Свободная переменная
            }

            return x;
        }

        //Решение СЛАУ методом Крамера (только для квадратных систем с ненулевым определителем)
        public static double[] SolveCramer(Matrix A, double[] b)
        {
            int n = A.Rows;
            if (n != A.Cols)
                throw new ArgumentException("Метод Крамера применим только к квадратным системам");
            if (n != b.Length)
                throw new ArgumentException("Размер вектора свободных членов не совпадает с размером матрицы");

            double detA = A.Determinant();
            if (Math.Abs(detA) < 1e-10)
                throw new Exception("Определитель матрицы равен нулю, метод Крамера неприменим");

            double[] x = new double[n];
            for (int i = 0; i < n; i++)
            {
                // Создаем матрицу, где i-й столбец заменен на вектор b
                var tempData = A.GetData();
                for (int row = 0; row < n; row++)
                    tempData[row, i] = b[row];

                var tempMatrix = new Matrix(tempData);
                x[i] = tempMatrix.Determinant() / detA;
            }

            return x;
        }


        //Решение СЛАУ через обратную матрицу (только для квадратных систем)
        public static double[] SolveInverse(Matrix A, double[] b)
        {
            int n = A.Rows;
            if (n != A.Cols)
                throw new ArgumentException("Матрица должна быть квадратной");
            if (n != b.Length)
                throw new ArgumentException("Размер вектора свободных членов не совпадает с размером матрицы");

            var Ainv = A.Inverse();
            double[] x = new double[n];

            for (int i = 0; i < n; i++)
            {
                x[i] = 0;
                for (int j = 0; j < n; j++)
                    x[i] += Ainv[i, j] * b[j];
            }

            return x;
        }

        //Вспомогательный метод для печати матрицы
        private static void PrintMatrix(StringBuilder sb, double[,] matrix, int cols)
        {
            int rows = matrix.GetLength(0);
            for (int i = 0; i < rows; i++)
            {
                sb.Append("| ");
                for (int j = 0; j < cols; j++)
                {
                    sb.Append($"{matrix[i, j],10:F4} ");
                }
                sb.Append($"| {matrix[i, cols],10:F4}");
                sb.AppendLine();
            }
        }
    }
}