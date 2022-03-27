using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace _3DEngine
{
    public class Vector3D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Vector3D()
        {
            X = 0;
            Y = 0;
            Z = 0;
            W = 1;
        }

        public Vector3D(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
            W = 1;
        }

        public static Vector3D Add(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        
        public static Vector3D Sub(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3D Mul(Vector3D v1, float k )
        {
            return new Vector3D(v1.X * k, v1.Y * k, v1.Z * k);
        }

        public static Vector3D Div(Vector3D v1, float k)
        {
            return new Vector3D(v1.X / k, v1.Y / k, v1.Z / k);
        }

        public static float DotProduct(Vector3D v1, Vector3D v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static float Length(Vector3D v)
        {
            return (float)Math.Sqrt(DotProduct(v, v));
        }

        public static Vector3D Normalise(Vector3D v)
        {
            float l = Length(v);
            return new Vector3D(v.X / l, v.Y / l, v.Z / l);
        }

        public static Vector3D CrossProduct(Vector3D v1, Vector3D v2)
        {
            Vector3D v = new Vector3D();

            v.X = v1.Y * v2.Z - v1.Z * v2.Y;
            v.Y = v1.Z * v2.X - v1.X * v2.Z;
            v.Z = v1.X * v2.Y - v1.Y * v2.X;

            return v;
        }

        public static Vector3D IntersectPlane(Vector3D planeP, Vector3D planeN, Vector3D lineStart, Vector3D lineEnd)
        {
            planeN = Normalise(planeN);
            float planeD = -DotProduct(planeN, planeP);
            float ad = DotProduct(lineStart, planeN);
            float bd = DotProduct(lineEnd, planeN);
            float t = (-planeD - ad) / (bd - ad);
            Vector3D lineStartToEnd = Sub(lineEnd, lineStart);
            Vector3D lineToIntersect = Mul(lineStartToEnd, t);
            return Add(lineStart, lineToIntersect);
        }

        public static int ClipAgainstPlane(Vector3D planeP, Vector3D planeN, Triangle inTri, ref Triangle outTri1, ref Triangle outTri2)
        {
            planeN = Normalise(planeN);

            Func<Vector3D, float> dist = p =>
            {
                return planeN.X * p.X + planeN.Y * p.Y + planeN.Z * p.Z - DotProduct(planeN, planeP);
            };

            Vector3D[] insidePoints = new Vector3D[3]
            {
                new Vector3D(),
                new Vector3D(),
                new Vector3D()
            }; int nInsidePointCount = 0;

            Vector3D[] outsidePoints = new Vector3D[3]
            {
                new Vector3D(),
                new Vector3D(),
                new Vector3D()
            }; int nOutsidePointCount = 0;

            float d0 = dist(inTri.P[0]);
            float d1 = dist(inTri.P[1]);
            float d2 = dist(inTri.P[2]);

            if (d0 >= 0) { insidePoints[nInsidePointCount++] = inTri.P[0]; }
            else { outsidePoints[nOutsidePointCount++] = inTri.P[0]; }

            if (d1 >= 0) { insidePoints[nInsidePointCount++] = inTri.P[1]; }
            else { outsidePoints[nOutsidePointCount++] = inTri.P[1]; }

            if (d2 >= 0) { insidePoints[nInsidePointCount++] = inTri.P[2]; }
            else { outsidePoints[nOutsidePointCount++] = inTri.P[2]; }

            if (nInsidePointCount == 0)
            {
                return 0;
            }

            if (nInsidePointCount == 3)
            {
                outTri1 = inTri;
                return 1;
            }

            if (nInsidePointCount == 1 && nOutsidePointCount == 2)
            {
                outTri1.TriColor = inTri.TriColor;

                //outTri1.TriColor = new SolidBrush(Color.Red);

                outTri1.P[0] = insidePoints[0];

                outTri1.P[1] = IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[0]);
                outTri1.P[2] = IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[1]);

                return 1;
            }

            if (nInsidePointCount == 2 && nOutsidePointCount == 1)
            {
                outTri1.TriColor = inTri.TriColor;
                outTri2.TriColor = inTri.TriColor;

               // outTri1.TriColor = new SolidBrush(Color.Blue);
                //outTri2.TriColor = new SolidBrush(Color.Green);

                outTri1.P[0] = insidePoints[0];
                outTri1.P[1] = insidePoints[1];
                outTri1.P[2] = IntersectPlane(planeP, planeN, insidePoints[0], outsidePoints[0]);

                outTri2.P[0] = insidePoints[1];
                outTri2.P[1] = outTri1.P[2];
                outTri2.P[2] = IntersectPlane(planeP, planeN, insidePoints[1], outsidePoints[0]);

                return 2;
            }

            return 0;
        }
    }
}
