using Calculator.VectorAlgebra;
using System;

namespace Calculator.Shapes3D
{
    public class Pyramid
    {
        public Vector3D A { get; }
        public Vector3D B { get; }
        public Vector3D C { get; }
        public Vector3D S { get; }

        public Pyramid(Vector3D a, Vector3D b, Vector3D c, Vector3D s)
        {
            A = a; B = b; C = c; S = s;
        }

        public double Volume
        {
            get
            {
                // V = 1/6 * |(AB, AC, AS)|
                var ab = B - A;
                var ac = C - A;
                var as_ = S - A;
                return Math.Abs(Vector3D.TripleProduct(ab, ac, as_)) / 6.0;
            }
        }

        public double BaseArea
        {
            get
            {
                // Площадь треугольника ABC
                return GetFaceArea(0, 1, 2);
            }
        }

        public double Height
        {
            get
            {
                // H = 3 * V / S_base
                double sBase = BaseArea;
                if (sBase == 0) return 0;
                return 3 * Volume / sBase;
            }
        }

        public double GetFaceArea(int i1, int i2, int i3)
        {
            Vector3D[] pts = { A, B, C, S };
            var v1 = pts[i2] - pts[i1];
            var v2 = pts[i3] - pts[i1];
            return Vector3D.Cross(v1, v2).Length / 2.0;
        }
    }
}