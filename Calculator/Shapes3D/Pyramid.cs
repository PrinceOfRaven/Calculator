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
        { A = a; B = b; C = c; S = s; }

        public double Volume => Math.Abs(Vector3D.TripleProduct(B - A, C - A, S - A)) / 6.0;

        public double BaseArea => GetFaceArea(0, 1, 2);

        public double Height
        {
            get
            {
                double s = BaseArea;
                return s == 0 ? 0 : 3 * Volume / s;
            }
        }

        public double GetFaceArea(int i1, int i2, int i3)
        {
            Vector3D[] pts = { A, B, C, S };
            return Vector3D.Cross(pts[i2] - pts[i1], pts[i3] - pts[i1]).Length / 2.0;
        }
    }
}
