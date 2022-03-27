using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _3DEngine
{
    public class Line2D
    {
        public Vector2D Start { get; set; }
        public Vector2D End { get; set; }
        public Color LineColor { get; set; }

        public Line2D(Vector2D start, Vector2D end, Color lineColor)
        {
            Start = start;
            End = end;
            LineColor = lineColor;

            Engine.CreateLine(this);
        }

        public void Delete()
        {
            Engine.DeleteLine(this);
        }
    }
}
