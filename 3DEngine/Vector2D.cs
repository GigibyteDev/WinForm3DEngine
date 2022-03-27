using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DEngine
{
    public class Vector2D
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2D()
        {

        }

        public Vector2D(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
