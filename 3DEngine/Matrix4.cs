using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DEngine
{
    public class Matrix4
    {
        public float[,] M { get; set; } = new float[4, 4];

        public Matrix4()
        {
            for (int x = 0; x < M.GetLength(0); x++)
            {
                for (int y = 0; y < M.GetLength(1); y++)
                {
                    M[x, y] = 0;
                }
            }
        }

        public static Vector3D MultiplyVector(Vector3D i, Matrix4 m)
        {
            Vector3D v = new Vector3D();

            v.X = i.X * m.M[0, 0] + i.Y * m.M[1, 0] + i.Z * m.M[2, 0] + i.W * m.M[3, 0];
            v.Y = i.X * m.M[0, 1] + i.Y * m.M[1, 1] + i.Z * m.M[2, 1] + i.W * m.M[3, 1];
            v.Z = i.X * m.M[0, 2] + i.Y * m.M[1, 2] + i.Z * m.M[2, 2] + i.W * m.M[3, 2];
            v.W = i.X * m.M[0, 3] + i.Y * m.M[1, 3] + i.Z * m.M[2, 3] + i.W * m.M[3, 3];

            return v;
        }

        public static Matrix4 MakeIdentity()
        {
            Matrix4 matrix = new Matrix4();

            matrix.M[0, 0] = 1.0f;
            matrix.M[1, 1] = 1.0f;
            matrix.M[2, 2] = 1.0f;
            matrix.M[3, 3] = 1.0f;

            return matrix;
        }

        public static Matrix4 MakeRotationX(float fAngleRad)
        {
            Matrix4 matrix = new Matrix4();

            matrix.M[0, 0] = 1.0f;
            matrix.M[1, 1] = (float)Math.Cos(fAngleRad);
            matrix.M[1, 2] = (float)Math.Sin(fAngleRad);
            matrix.M[2, 1] = -(float)Math.Sin(fAngleRad);
            matrix.M[2, 2] = (float)Math.Cos(fAngleRad);
            matrix.M[3, 3] = 1.0f;

            return matrix;
        }

        public static Matrix4 MakeRotationY(float fAngleRad)
        {
            Matrix4 matrix = new Matrix4();

            matrix.M[0, 0] = (float)Math.Cos(fAngleRad);
            matrix.M[0, 2] = (float)Math.Sin(fAngleRad);
            matrix.M[2, 0] = -(float)Math.Sin(fAngleRad);
            matrix.M[1, 1] = 1.0f;
            matrix.M[2, 2] = (float)Math.Cos(fAngleRad);
            matrix.M[3, 3] = 1.0f;

            return matrix;
        }

        public static Matrix4 MakeRotationZ(float fAngleRad)
        {
            Matrix4 matrix = new Matrix4();

            matrix.M[0, 0] = (float)Math.Cos(fAngleRad);
            matrix.M[0, 1] = (float)Math.Sin(fAngleRad);
            matrix.M[1, 0] = -(float)Math.Sin(fAngleRad);
            matrix.M[1, 1] = (float)Math.Cos(fAngleRad);
            matrix.M[2, 2] = 1.0f;
            matrix.M[3, 3] = 1.0f;

            return matrix;
        }

        public static Matrix4 MakeTranslation(float x, float y, float z)
        {
            Matrix4 matrix = new Matrix4();

            matrix.M[0, 0] = 1.0f;
            matrix.M[1, 1] = 1.0f;
            matrix.M[2, 2] = 1.0f;
            matrix.M[3, 3] = 1.0f;
            matrix.M[3, 0] = x;
            matrix.M[3, 1] = y;
            matrix.M[3, 2] = z;

            return matrix;
        }

        public static Matrix4 MakeProjection(float fFovDegrees, float fAspectRatio, float fNear, float fFar)
        {
            float fFovRad = 1.0f / (float)Math.Tan(fFovDegrees * 0.5f / 180.0f * 3.14159f);

            Matrix4 matrix = new Matrix4();

            matrix.M[0, 0] = fAspectRatio * fFovRad;
            matrix.M[1, 1] = fFovRad;
            matrix.M[2, 2] = fFar / (fFar - fNear);
            matrix.M[3, 2] = (-fFar * fNear) / (fFar - fNear);
            matrix.M[2, 3] = 1.0f;

            return matrix;
        }

        public static Matrix4 MultiplyMatrix(Matrix4 m1, Matrix4 m2)
        {
            Matrix4 matrix = new Matrix4();

            for (int c = 0; c < 4; c++)
                for (int r = 0; r < 4; r++)
                    matrix.M[r, c] = m1.M[r, 0] * m2.M[0, c] + m1.M[r, 1] * m2.M[1, c] + m1.M[r, 2] * m2.M[2, c] + m1.M[r, 3] * m2.M[3, c];

            return matrix;
        }

        public static Matrix4 PointAt(Vector3D pos, Vector3D target, Vector3D up)
        {
            Vector3D newForward = Vector3D.Sub(target, pos);
            newForward = Vector3D.Normalise(newForward);

            Vector3D a = Vector3D.Mul(newForward, Vector3D.DotProduct(up, newForward));
            Vector3D newUp = Vector3D.Sub(up, a);
            newUp = Vector3D.Normalise(newUp);

            Vector3D newRight = Vector3D.CrossProduct(newUp, newForward);

            Matrix4 matrix = new Matrix4();
            matrix.M[0, 0] = newRight.X;    matrix.M[0, 1] = newRight.Y;    matrix.M[0, 2] = newRight.Z;    matrix.M[0, 3] = 0.0f;
            matrix.M[1, 0] = newUp.X;       matrix.M[1, 1] = newUp.Y;       matrix.M[1, 2] = newUp.Z;       matrix.M[1, 3] = 0.0f;
            matrix.M[2, 0] = newForward.X;  matrix.M[2, 1] = newForward.Y;  matrix.M[2, 2] = newForward.Z;  matrix.M[2, 3] = 0.0f;
            matrix.M[3, 0] = pos.X;         matrix.M[3, 1] = pos.Y;         matrix.M[3, 2] = pos.Z;         matrix.M[3, 3] = 1.0f;
            return matrix;
        }

        public static Matrix4 QuickInverse(Matrix4 m) // Only for Rotation/Translation Matrices
        {
            Matrix4 matrix = new Matrix4();
            matrix.M[0, 0] = m.M[0, 0]; matrix.M[0, 1] = m.M[1, 0]; matrix.M[0, 2] = m.M[2, 0]; matrix.M[0, 3] = 0.0f;
            matrix.M[1, 0] = m.M[0, 1]; matrix.M[1, 1] = m.M[1, 1]; matrix.M[1, 2] = m.M[2, 1]; matrix.M[1, 3] = 0.0f;
            matrix.M[2, 0] = m.M[0, 2]; matrix.M[2, 1] = m.M[1, 2]; matrix.M[2, 2] = m.M[2, 2]; matrix.M[2, 3] = 0.0f;
            matrix.M[3, 0] = -(m.M[3, 0] * matrix.M[0, 0] + m.M[3, 1] * matrix.M[1, 0] + m.M[3, 2] * matrix.M[2, 0]);
            matrix.M[3, 1] = -(m.M[3, 0] * matrix.M[0, 1] + m.M[3, 1] * matrix.M[1, 1] + m.M[3, 2] * matrix.M[2, 1]);
            matrix.M[3, 2] = -(m.M[3, 0] * matrix.M[0, 2] + m.M[3, 1] * matrix.M[1, 2] + m.M[3, 2] * matrix.M[2, 2]);
            matrix.M[3, 3] = 1.0f;
            return matrix;
        }
    }
}
