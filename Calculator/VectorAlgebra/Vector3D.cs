using System;

namespace Calculator.VectorAlgebra
{
    public class Vector3D
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Vector3D(double x = 0, double y = 0, double z = 0) { X = x; Y = y; Z = z; }

        public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

        public static Vector3D operator -(Vector3D a, Vector3D b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3D operator +(Vector3D a, Vector3D b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3D operator *(Vector3D v, double s) => new(v.X * s, v.Y * s, v.Z * s);
        public static Vector3D operator /(Vector3D v, double s)
        {
            if (Math.Abs(s) < 1e-10) throw new ArgumentException("Деление на ноль");
            return new(v.X / s, v.Y / s, v.Z / s);
        }

        public static double Dot(Vector3D a, Vector3D b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        public static Vector3D Cross(Vector3D a, Vector3D b) => new(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X);

        public static double TripleProduct(Vector3D a, Vector3D b, Vector3D c) => Dot(a, Cross(b, c));

        public Vector3D Normalize()
        {
            double len = Length;
            if (len < 1e-10) throw new InvalidOperationException("Нельзя нормализовать нулевой вектор");
            return this / len;
        }

        public static double AngleBetween(Vector3D a, Vector3D b)
        {
            double cos = Dot(a, b) / (a.Length * b.Length);
            return Math.Acos(Math.Max(-1, Math.Min(1, cos)));
        }

        public static double AngleBetweenDegrees(Vector3D a, Vector3D b) => AngleBetween(a, b) * 180.0 / Math.PI;

        public override string ToString() => $"({X:F4}, {Y:F4}, {Z:F4})";
        public double[] ToArray() => new[] { X, Y, Z };
    }
}
