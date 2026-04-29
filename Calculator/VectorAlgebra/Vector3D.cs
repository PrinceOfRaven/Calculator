using System;

namespace Calculator.VectorAlgebra
{
    //Класс для работы с векторами в 3D пространстве
    public class Vector3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3D(double x = 0, double y = 0, double z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        //Длина вектора
        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);


        //Вычитание векторов
        public static Vector3D operator -(Vector3D a, Vector3D b)
        {
            return new Vector3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        //Деление на скаляр
        public static Vector3D operator /(Vector3D v, double scalar)
        {
            if (Math.Abs(scalar) < 1e-10)
                throw new ArgumentException("Деление на ноль");
            return new Vector3D(v.X / scalar, v.Y / scalar, v.Z / scalar);
        }

        //Скалярное произведение векторов
        public static double Dot(Vector3D a, Vector3D b)
        {
            return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
        }

        //Векторное произведение векторов
        public static Vector3D Cross(Vector3D a, Vector3D b)
        {
            return new Vector3D(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X
            );
        }

        //Смешанное произведение трех векторов
        public static double TripleProduct(Vector3D a, Vector3D b, Vector3D c)
        {
            return Dot(a, Cross(b, c));
        }

        //Нормализация вектора (единичный вектор)
        public Vector3D Normalize()
        {
            double len = Length;
            if (len < 1e-10)
                throw new InvalidOperationException("Нельзя нормализовать нулевой вектор");
            return this / len;
        }

        //Угол между векторами (в радианах)
        public static double AngleBetween(Vector3D a, Vector3D b)
        {
            double dot = Dot(a, b);
            double lenA = a.Length;
            double lenB = b.Length;

            if (lenA < 1e-10 || lenB < 1e-10)
                throw new ArgumentException("Нулевой вектор не имеет направления");

            double cosAngle = dot / (lenA * lenB);
            // Ограничиваем значение от -1 до 1
            cosAngle = Math.Max(-1, Math.Min(1, cosAngle));
            return Math.Acos(cosAngle);
        }


        //Угол между векторами (в градусах)
        public static double AngleBetweenDegrees(Vector3D a, Vector3D b)
        {
            return AngleBetween(a, b) * 180.0 / Math.PI;
        }


        public override string ToString()
        {
            return $"({X:F4}, {Y:F4}, {Z:F4})";
        }

        public double[] ToArray()
        {
            return new double[] { X, Y, Z };
        }
    }
}
