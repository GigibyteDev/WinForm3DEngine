using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Drawing.Drawing2D;

namespace _3DEngine
{

    public class Canvas : Form
    {
        public Canvas()
        {
            DoubleBuffered = true;
            KeyDown += new System.Windows.Forms.KeyEventHandler(Canvas_KeyDown);
            KeyUp += new System.Windows.Forms.KeyEventHandler(Canvas_KeyUp);
        }

        public Dictionary<string, bool> keysPressed { get; set; } = new Dictionary<string, bool>()
        {
            { "w", false},
            { "s", false },
            { "a", false },
            { "d", false },
            { "ctrl", false },
            { "space", false },
            { "left", false },
            { "right", false }
        };

        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    keysPressed["space"] = true;
                    break;
                case Keys.ControlKey:
                    keysPressed["ctrl"] = true;
                    break;
                case Keys.Left:
                    keysPressed["left"] = true;
                    break;
                case Keys.Right:
                    keysPressed["right"] = true;
                    break;
                case Keys.W:
                    keysPressed["w"] = true;
                    break;
                case Keys.S:
                    keysPressed["s"] = true;
                    break;
                case Keys.A:
                    keysPressed["a"] = true;
                    break;
                case Keys.D:
                    keysPressed["d"] = true;
                    break;
                default:
                    Log.Info($"KEY PRESSED\t{e.KeyCode}");
                    break;
            }
        }
        private void Canvas_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    keysPressed["space"] = false;
                    break;
                case Keys.ControlKey:
                    keysPressed["ctrl"] = false;
                    break;
                case Keys.Left:
                    keysPressed["left"] = false;
                    break;
                case Keys.Right:
                    keysPressed["right"] = false;
                    break;
                case Keys.W:
                    keysPressed["w"] = false;
                    break;
                case Keys.S:
                    keysPressed["s"] = false;
                    break;
                case Keys.A:
                    keysPressed["a"] = false;
                    break;
                case Keys.D:
                    keysPressed["d"] = false;
                    break;
                default:
                    //Log.Info($"KEY UNPRESSED\t{e.KeyCode}");
                    break;
            }
        }
    }

    public abstract class Engine
    {
        public Vector2D ScreenSize { get; set; } = new Vector2D(512, 512);
        public string Title;

        public Canvas Window = null;
        private Thread GameLoopThread = null;

        private float fElapsedTime = 0.01f;
        private float fTheta = 0.0f;
        private int FPS = 16;

        private static List<Line2D> AllLines = new List<Line2D>();
        private static List<Triangle2D> AllTriangles = new List<Triangle2D>();

        public Color BackgroundColor = Color.Black;

        public Engine(Vector2D screenSize, string title, int fps)
        {
            ScreenSize = screenSize;
            Title = title;

            Window = new Canvas();
            Window.Size = new Size((int)ScreenSize.X, (int)ScreenSize.Y);
            Window.Text = Title;
            Window.Paint += Renderer;

            FPS = (int)Math.Floor(1000.0f / fps);

            fElapsedTime = (1000 / fps) / 1000.0f;

            GameLoopThread = new Thread(GameLoop);
            GameLoopThread.Start();

            Application.Run(Window);
        }

        public static void CreateLine(Line2D line)
        {
            AllLines.Add(line);
        }

        public static void DrawTriangle(float x1, float y1, float x2, float y2, float x3, float y3, Brush color)
        {
            AllTriangles.Add(new Triangle2D(new Vector2D(x1, y1), new Vector2D(x2, y2), new Vector2D(x3, y3), color));
        }

        public static void DeleteTris()
        {
            AllTriangles.Clear();
        }

        public static void DeleteLines()
        {
            AllLines.Clear();
        }

        public static void DeleteLine(Line2D line)
        {
            AllLines.Remove(line);
        }

        private void GameLoop()
        {
            OnLoad();

            while (GameLoopThread.IsAlive)
            {
                try
                {
                    OnUpdate(GetTime());
                    Window.BeginInvoke((MethodInvoker)delegate { Window.Refresh(); });
                    OnDraw();
                    Thread.Sleep(FPS);
                }
                catch
                {
                    Log.Error("Display Window Lost...");
                    Thread.Sleep(500);
                }
            }
        }

        private void Renderer(object sender, PaintEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                g.Clear(BackgroundColor);

                
                foreach (Triangle2D triangle in AllTriangles)
                {
                    g.FillPolygon(triangle.FillColor, new PointF[] { new PointF(triangle.P[0].X, triangle.P[0].Y), new PointF(triangle.P[1].X, triangle.P[1].Y), new PointF(triangle.P[2].X, triangle.P[2].Y) });
                }
                
                
                foreach (Triangle2D triangle in AllTriangles)
                {
                    //g.DrawPolygon(new Pen(Color.Black), new PointF[] { new PointF(triangle.P[0].X, triangle.P[0].Y), new PointF(triangle.P[1].X, triangle.P[1].Y), new PointF(triangle.P[2].X, triangle.P[2].Y) });
                }
                
            }
            catch
            {
                //Log.Error("IDK LOL");
            }
        }

        private float GetTime()
        {
            return fTheta += fElapsedTime;
        }

        public abstract void OnLoad();
        public abstract void OnUpdate(float fElapsedTime);
        public abstract void OnDraw();
    }
}