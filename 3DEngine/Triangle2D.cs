using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace _3DEngine
{
    class Triangle2D
    {
        public Vector2D[] P { get; set; } = new Vector2D[3];
        public Brush FillColor { get; set; }
        public Triangle2D(Vector2D x, Vector2D y, Vector2D z, Brush fillColor)
        {
            P[0] = x;
            P[1] = y;
            P[2] = z;
            FillColor = fillColor;
        }
    }
}
