using Calculator.VectorAlgebra;
using System;
using System.Collections.Generic;

namespace Calculator.Shapes3D
{
    // находится в разработке, пока неполный фунционал
    public class Cube
    {
        public List<Vector3D> Vertices { get; }

        public Cube(List<Vector3D> vertices)
        {
            if (vertices.Count != 8) throw new ArgumentException("Куб должен иметь 8 вершин");
            Vertices = vertices;
        }

        public double Volume
        {
            get
            {
                try
                {
                    var v1 = Vertices[1] - Vertices[0];
                    var v2 = Vertices[3] - Vertices[0];
                    var v3 = Vertices[4] - Vertices[0];
                    return Math.Abs(Vector3D.TripleProduct(v1, v2, v3));
                }
                catch
                {
                    return 0;
                }
            }
        }
    }
}