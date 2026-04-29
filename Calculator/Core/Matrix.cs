using System;

namespace Calculator.Core
{
    public class Matrix
    {
        private double[,] _data;
        public int Rows { get; private set; }
        public int Cols { get; private set; }

        public Matrix(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            _data = new double[rows, cols];
        }

        public Matrix(double[,] data)
        {
            Rows = data.GetLength(0);
            Cols = data.GetLength(1);
            _data = (double[,])data.Clone();
        }

        public double this[int row, int col]
        {
            get => _data[row, col];
            set => _data[row, col] = value;
        }

        public double[,] GetData() => (double[,])_data.Clone();

        //Сложение матриц
        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows || a.Cols != b.Cols)
                throw new ArgumentException("Размеры матриц должны совпадать");

            var result = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    result[i, j] = a[i, j] + b[i, j];

            return result;
        }

        //Вычитание матриц
        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.Rows != b.Rows || a.Cols != b.Cols)
                throw new ArgumentException("Размеры матриц должны совпадать");

            var result = new Matrix(a.Rows, a.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < a.Cols; j++)
                    result[i, j] = a[i, j] - b[i, j];

            return result;
        }


        //Умножение матриц
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.Cols != b.Rows)
                throw new ArgumentException("Количество столбцов первой матрицы должно равняться количеству строк второй");

            var result = new Matrix(a.Rows, b.Cols);
            for (int i = 0; i < a.Rows; i++)
                for (int j = 0; j < b.Cols; j++)
                    for (int k = 0; k < a.Cols; k++)
                        result[i, j] += a[i, k] * b[k, j];

            return result;
        }

        //Транспонирование матрицы
        public Matrix Transpose()
        {
            var result = new Matrix(Cols, Rows);
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    result[j, i] = _data[i, j];

            return result;
        }


        //Вычисление определителя матрицы (только для квадратных)
        public double Determinant()
        {
            if (Rows != Cols)
                throw new ArgumentException("Матрица должна быть квадратной");

            if (Rows == 1)
                return _data[0, 0];

            if (Rows == 2)
                return _data[0, 0] * _data[1, 1] - _data[0, 1] * _data[1, 0];

            // Метод Гаусса для вычисления определителя
            var temp = (double[,])_data.Clone();
            int n = Rows;
            double det = 1.0;

            for (int i = 0; i < n; i++)
            {
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(temp[k, i]) > Math.Abs(temp[maxRow, i]))
                        maxRow = k;
                }

                if (Math.Abs(temp[maxRow, i]) < 1e-10)
                    return 0;

                if (maxRow != i)
                {
                    for (int j = 0; j < n; j++)
                    {
                        double t = temp[i, j];
                        temp[i, j] = temp[maxRow, j];
                        temp[maxRow, j] = t;
                    }
                    det = -det;
                }

                det *= temp[i, i];

                for (int k = i + 1; k < n; k++)
                {
                    double factor = temp[k, i] / temp[i, i];
                    for (int j = i; j < n; j++)
                        temp[k, j] -= factor * temp[i, j];
                }
            }

            return det;
        }


        //Обратная матрица (только для невырожденных квадратных)
        public Matrix Inverse()
        {
            if (Rows != Cols)
                throw new ArgumentException("Матрица должна быть квадратной");

            double det = Determinant();
            if (Math.Abs(det) < 1e-10)
                throw new ArgumentException("Матрица вырождена (определитель = 0)");

            int n = Rows;
            var augmented = new double[n, 2 * n];

            // Создаем расширенную матрицу [A|I]
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    augmented[i, j] = _data[i, j];
                augmented[i, n + i] = 1.0;
            }

            // Метод Гаусса-Жордана
            for (int i = 0; i < n; i++)
            {
                // Поиск главного элемента
                int maxRow = i;
                for (int k = i + 1; k < n; k++)
                {
                    if (Math.Abs(augmented[k, i]) > Math.Abs(augmented[maxRow, i]))
                        maxRow = k;
                }

                // Обмен строк
                if (maxRow != i)
                {
                    for (int j = 0; j < 2 * n; j++)
                    {
                        double t = augmented[i, j];
                        augmented[i, j] = augmented[maxRow, j];
                        augmented[maxRow, j] = t;
                    }
                }

                double pivot = augmented[i, i];
                for (int j = 0; j < 2 * n; j++)
                    augmented[i, j] /= pivot;

                for (int k = 0; k < n; k++)
                {
                    if (k != i)
                    {
                        double factor = augmented[k, i];
                        for (int j = 0; j < 2 * n; j++)
                            augmented[k, j] -= factor * augmented[i, j];
                    }
                }
            }

            // Извлекаем обратную матрицу
            var result = new Matrix(n, n);
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    result[i, j] = augmented[i, n + j];

            return result;
        }


        //Вычисление ранга матрицы методом Гаусса
        public int Rank()
        {
            var temp = (double[,])_data.Clone();
            int m = Rows;
            int n = Cols;
            int rank = 0;

            for (int col = 0; col < n && rank < m; col++)
            {
                // Поиск главного элемента
                int maxRow = rank;
                for (int k = rank + 1; k < m; k++)
                {
                    if (Math.Abs(temp[k, col]) > Math.Abs(temp[maxRow, col]))
                        maxRow = k;
                }

                if (Math.Abs(temp[maxRow, col]) < 1e-10)
                    continue;

                // Обмен строк
                if (maxRow != rank)
                {
                    for (int j = 0; j < n; j++)
                    {
                        double t = temp[rank, j];
                        temp[rank, j] = temp[maxRow, j];
                        temp[maxRow, j] = t;
                    }
                }

                // Исключение
                for (int k = rank + 1; k < m; k++)
                {
                    double factor = temp[k, col] / temp[rank, col];
                    for (int j = col; j < n; j++)
                        temp[k, j] -= factor * temp[rank, j];
                }

                rank++;
            }

            return rank;
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                    sb.Append($"{_data[i, j],10:F4}");
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
