using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace _3DEngine
{
    public class Triangle
    {
        public Vector3D[] P { get; set; } = new Vector3D[3];
        public Brush TriColor { get; set; }
        public Triangle(Vector3D X, Vector3D Y, Vector3D Z)
        {
            P[0] = X;
            P[1] = Y;
            P[2] = Z;
        }

        public Triangle(Vector3D X, Vector3D Y, Vector3D Z, Brush triColor)
        {
            P[0] = X;
            P[1] = Y;
            P[2] = Z;
            TriColor = triColor;
        }

        public Triangle()
        {
            P[0] = new Vector3D();
            P[1] = new Vector3D();
            P[2] = new Vector3D();
        }

    }
}
