using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace _3DEngine._3DEngineAttempt
{
    class MainWindow : Engine
    {
        public MainWindow() : base(new Vector2D(1280, 720), $"3D Engine", 60) { }
        Mesh mesh = new Mesh();
        Matrix4 matProj = new Matrix4();
        Vector3D vCamera = new Vector3D();
        Vector3D vLookDir = new Vector3D();

        float fYaw = 0.0f;

        public override void OnLoad()
        {
            mesh.LoadMeshFromFile(@"C:\Users\GigiPC\source\repos\3DEngineAttempt\3DEngineAttempt\Objects\GuruGuru.obj");
            Log.Info($"TRI COUNT: {mesh.Tris.Count()}");
            matProj = Matrix4.MakeProjection(90.0f, ScreenSize.Y / ScreenSize.X, 0.1f, 1000.0f);
        }

        public static float fElapsedTime = 0.03f;

        public override void OnUpdate(float fTheta)
        {
            DeleteTris();
            //fTheta = 1.0f;

            float speed = 4.0f * fElapsedTime;

            if (Window.keysPressed["space"])
                vCamera.Y += speed;
            if (Window.keysPressed["ctrl"])
                vCamera.Y -= speed;
            if (Window.keysPressed["left"])
                vCamera.X += speed;
            if (Window.keysPressed["right"])
                vCamera.X -= speed;

            Vector3D vForward = Vector3D.Mul(vLookDir, speed);
            if (Window.keysPressed["w"])
                vCamera = Vector3D.Add(vCamera, vForward);
            if (Window.keysPressed["s"])
                vCamera = Vector3D.Sub(vCamera, vForward);

            if (Window.keysPressed["a"])
                fYaw -= speed / 2;
            if (Window.keysPressed["d"])
                fYaw += speed / 2;

            Matrix4 matRotZ = new Matrix4();
            Matrix4 matRotX = new Matrix4();

            matRotZ = Matrix4.MakeRotationZ(fTheta);
            matRotX = Matrix4.MakeRotationX(fTheta * 0.5f);

            Matrix4 matTrans;
            matTrans = Matrix4.MakeTranslation(0.0f, 0.0f, 2.0f);

            Matrix4 matWorld;
            matWorld = Matrix4.MakeIdentity();

            //matWorld = Matrix4.MultiplyMatrix(matRotZ, matRotX);
            matWorld = Matrix4.MultiplyMatrix(matWorld, matTrans);

            Vector3D vUp = new Vector3D(0, 1, 0);
            Vector3D vTarget = new Vector3D(0, 0, 1);
            Matrix4 matCameraRot = Matrix4.MakeRotationY(fYaw);
            vLookDir = Matrix4.MultiplyVector(vTarget, matCameraRot);
            vTarget = Vector3D.Add(vCamera, vLookDir);

            Matrix4 matCamera = Matrix4.PointAt(vCamera, vTarget, vUp);

            Matrix4 matView = Matrix4.QuickInverse(matCamera);

            List<Triangle> AllTriangles = new List<Triangle>();

            foreach (Triangle tri in mesh.Tris)
            {
                Triangle triProjected = new Triangle();
                Triangle triTransformed = new Triangle();
                Triangle triViewed = new Triangle();

                triTransformed.P[0] = Matrix4.MultiplyVector(tri.P[0], matWorld);
                triTransformed.P[1] = Matrix4.MultiplyVector(tri.P[1], matWorld);
                triTransformed.P[2] = Matrix4.MultiplyVector(tri.P[2], matWorld);

                Vector3D normal = new Vector3D();
                Vector3D line1 = new Vector3D();
                Vector3D line2 = new Vector3D();

                line1 = Vector3D.Sub(triTransformed.P[1], triTransformed.P[0]);
                line2 = Vector3D.Sub(triTransformed.P[2], triTransformed.P[0]);

                normal = Vector3D.CrossProduct(line1, line2);

                normal = Vector3D.Normalise(normal);

                Vector3D vCameraRay = Vector3D.Sub(triTransformed.P[0], vCamera);

                if (Vector3D.DotProduct(normal, vCameraRay) < 0.0f)
                {
                    Vector3D lightDirection = new Vector3D(0.0f, 0.0f, -1.0f);
                    lightDirection = Vector3D.Normalise(lightDirection);

                    float dp = Math.Max(0.1f, Vector3D.DotProduct(lightDirection, normal));

                    triTransformed.TriColor = GetColor(dp);

                    triViewed.P[0] = Matrix4.MultiplyVector(triTransformed.P[0], matView);
                    triViewed.P[1] = Matrix4.MultiplyVector(triTransformed.P[1], matView);
                    triViewed.P[2] = Matrix4.MultiplyVector(triTransformed.P[2], matView);
                    triViewed.TriColor = triTransformed.TriColor;

                    int nClippedTriangles = 0;
                    Triangle[] clipped = new Triangle[2]
                    {
                        new Triangle(),
                        new Triangle()
                    };

                    nClippedTriangles = Vector3D.ClipAgainstPlane(new Vector3D(0.0f, 0.0f, 0.1f), new Vector3D(0.0f, 0.0f, 1.0f), triViewed, ref clipped[0], ref clipped[1]);
                    
                    for (int i = 0; i < nClippedTriangles; i++)
                    {
                        triProjected.P[0] = Matrix4.MultiplyVector(clipped[i].P[0], matProj);
                        triProjected.P[1] = Matrix4.MultiplyVector(clipped[i].P[1], matProj);
                        triProjected.P[2] = Matrix4.MultiplyVector(clipped[i].P[2], matProj);
                        triProjected.TriColor = clipped[i].TriColor;

                        triProjected.P[0] = Vector3D.Div(triProjected.P[0], triProjected.P[0].W);
                        triProjected.P[1] = Vector3D.Div(triProjected.P[1], triProjected.P[1].W);
                        triProjected.P[2] = Vector3D.Div(triProjected.P[2], triProjected.P[2].W);

                        triProjected.P[0].X *= -1.0f;
                        triProjected.P[1].X *= -1.0f;
                        triProjected.P[2].X *= -1.0f;
                        triProjected.P[0].Y *= -1.0f;
                        triProjected.P[1].Y *= -1.0f;
                        triProjected.P[2].Y *= -1.0f;

                        Vector3D vOffsetView = new Vector3D(1, 1, 0);

                        triProjected.P[0] = Vector3D.Add(triProjected.P[0], vOffsetView);
                        triProjected.P[1] = Vector3D.Add(triProjected.P[1], vOffsetView);
                        triProjected.P[2] = Vector3D.Add(triProjected.P[2], vOffsetView);

                        triProjected.P[0].X *= 0.5f * ScreenSize.X;
                        triProjected.P[0].Y *= 0.5f * ScreenSize.Y;
                        triProjected.P[1].X *= 0.5f * ScreenSize.X;
                        triProjected.P[1].Y *= 0.5f * ScreenSize.Y;
                        triProjected.P[2].X *= 0.5f * ScreenSize.X;
                        triProjected.P[2].Y *= 0.5f * ScreenSize.Y;

                        Vector3D[] tempPoints = new Vector3D[3]
                        {
                            new Vector3D(triProjected.P[0].X, triProjected.P[0].Y, triProjected.P[0].Z),
                            new Vector3D(triProjected.P[1].X, triProjected.P[1].Y, triProjected.P[1].Z),
                            new Vector3D(triProjected.P[2].X, triProjected.P[2].Y, triProjected.P[2].Z),
                        };

                        Triangle tempTriangle = new Triangle();

                        tempTriangle.P[0] = tempPoints[0];
                        tempTriangle.P[1] = tempPoints[1];
                        tempTriangle.P[2] = tempPoints[2];
                        tempTriangle.TriColor = triProjected.TriColor;

                        AllTriangles.Add(tempTriangle);
                    }
                }
            }

            AllTriangles.Sort((t1, t2) =>
            {
                float z1 = (t1.P[0].Z + t1.P[1].Z + t1.P[2].Z) / 3.0f;
                float z2 = (t2.P[0].Z + t2.P[1].Z + t2.P[2].Z) / 3.0f;
                if (z1 > z2)
                    return -1;
                else
                    return 1;
            }
            );

            foreach(Triangle triToRaster in AllTriangles)
            {
                Queue<Triangle> queueTriangles = new Queue<Triangle>();
                queueTriangles.Enqueue(triToRaster);
                int nNewTriangles = 1;

                for (int p = 0; p < 4; p++)
                {
                    int nTrisToAdd = 0;
                    while (nNewTriangles > 0)
                    {
                        Triangle[] clipped = new Triangle[2]
                {
                    new Triangle(),
                    new Triangle()
                };
                        Triangle test = queueTriangles.Dequeue();
                        nNewTriangles--;

                        switch (p)
                        {
                            case 0:
                                nTrisToAdd = Vector3D.ClipAgainstPlane(new Vector3D(0.0f, 0.0f, 0.0f), new Vector3D(0.0f, 1.0f, 0.0f), test, ref clipped[0], ref clipped[1]);
                                break;
                            case 1:
                                nTrisToAdd = Vector3D.ClipAgainstPlane(new Vector3D(0.0f, ScreenSize.Y - 1, 0.0f), new Vector3D(0.0f, -1.0f, 0.0f), test, ref clipped[0], ref clipped[1]);
                                break;
                            case 2:
                                nTrisToAdd = Vector3D.ClipAgainstPlane(new Vector3D(0.0f, 0.0f, 0.0f), new Vector3D(1.0f, 0.0f, 0.0f), test, ref clipped[0], ref clipped[1]);
                                break;
                            case 3:
                                nTrisToAdd = Vector3D.ClipAgainstPlane(new Vector3D(ScreenSize.X - 1, 0.0f, 0.0f), new Vector3D(-1.0f, 0.0f, 0.0f), test, ref clipped[0], ref clipped[1]);
                                break;
                        }

                        for (int w = 0; w < nTrisToAdd; w++)
                        {
                            queueTriangles.Enqueue(clipped[w]);
                        }
                    }
                    nNewTriangles = queueTriangles.Count();
                }

                foreach (Triangle t in queueTriangles)
                {
                    DrawTriangle(t.P[0].X, t.P[0].Y,
                                     t.P[1].X, t.P[1].Y,
                                     t.P[2].X, t.P[2].Y,
                                     t.TriColor
                                     );
                }
            }
        }

        public override void OnDraw(){ }

        public Brush GetColor(float lum)
        {
            Brush brush = new SolidBrush(Color.Black);
            int pixel_bw = (int)(31.0f * lum);

            Color darkColor = Color.DarkGray;
            Color color = Color.Gray;
            Color lightColor = Color.LightGray;

            switch (pixel_bw)
            {
                case 1:
                    brush = new HatchBrush(HatchStyle.Percent10, darkColor, Color.Black);
                    break;
                case 2:
                    brush = new HatchBrush(HatchStyle.Percent20, darkColor, Color.Black);
                    break;
                case 3:
                    brush = new HatchBrush(HatchStyle.Percent30, darkColor, Color.Black);
                    break;
                case 4:
                    brush = new HatchBrush(HatchStyle.Percent40, darkColor, Color.Black);
                    break;
                case 5:
                    brush = new HatchBrush(HatchStyle.Percent50, darkColor, Color.Black);
                    break;
                case 6:
                    brush = new HatchBrush(HatchStyle.Percent60, darkColor, Color.Black);
                    break;
                case 7:
                    brush = new HatchBrush(HatchStyle.Percent70, darkColor, Color.Black);
                    break;
                case 8:
                    brush = new HatchBrush(HatchStyle.Percent80, darkColor, Color.Black);
                    break;
                case 9:
                    brush = new HatchBrush(HatchStyle.Percent90, darkColor, Color.Black);
                    break;
                case 10:
                    brush = new SolidBrush(darkColor);
                    break;
                case 11:
                    brush = new HatchBrush(HatchStyle.Percent10, color, darkColor);
                    break;
                case 12:
                    brush = new HatchBrush(HatchStyle.Percent20, color, darkColor);
                    break;
                case 13:
                    brush = new HatchBrush(HatchStyle.Percent30, color, darkColor);
                    break;
                case 14:
                    brush = new HatchBrush(HatchStyle.Percent40, color, darkColor);
                    break;
                case 15:
                    brush = new HatchBrush(HatchStyle.Percent50, color, darkColor);
                    break;
                case 16:
                    brush = new HatchBrush(HatchStyle.Percent60, color, darkColor);
                    break;
                case 17:
                    brush = new HatchBrush(HatchStyle.Percent70, color, darkColor);
                    break;
                case 18:
                    brush = new HatchBrush(HatchStyle.Percent80, color, darkColor);
                    break;
                case 19:
                    brush = new HatchBrush(HatchStyle.Percent90, color, darkColor);
                    break;
                case 20:
                    brush = new SolidBrush(color);
                    break;
                case 21:
                    brush = new HatchBrush(HatchStyle.Percent10, lightColor, color);
                    break;
                case 22:
                    brush = new HatchBrush(HatchStyle.Percent20, lightColor, color);
                    break;
                case 23:
                    brush = new HatchBrush(HatchStyle.Percent30, lightColor, color);
                    break;
                case 24:
                    brush = new HatchBrush(HatchStyle.Percent40, lightColor, color);
                    break;
                case 25:
                    brush = new HatchBrush(HatchStyle.Percent50, lightColor, color);
                    break;
                case 26:
                    brush = new HatchBrush(HatchStyle.Percent60, lightColor, color);
                    break;
                case 27:
                    brush = new HatchBrush(HatchStyle.Percent70, lightColor, color);
                    break;
                case 28:
                    brush = new HatchBrush(HatchStyle.Percent80, lightColor, color);
                    break;
                case 29:
                    brush = new HatchBrush(HatchStyle.Percent90, lightColor, color);
                    break;
                case 30:
                    brush = new SolidBrush(lightColor);
                    break;
                default:
                    break;
            }
            return brush;
        }

    }
}
