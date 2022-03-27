using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace _3DEngine
{
    public class Mesh
    {
        public List<Triangle> Tris { get; set; } = new List<Triangle>();

        public void LoadMeshFromFile(string path)
        {
            if (File.Exists(path))
            {
                List<Vector3D> verts = new List<Vector3D>();

                List<string> lines = File.ReadAllLines(path).ToList();
                foreach(string line in lines)
                {
                    if (line != "")
                    {
                        if (line[0] == 'v' && line[1] == ' ')
                        {
                            string[] data = line.Split(' ');
                            verts.Add(new Vector3D(float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3])));
                        }
                        else if (line[0] == 'f' && line[1] == ' ')
                        {
                            string[] data = line.Split(' ');
                            int[] f = new int[3];
                            for(int i = 0; i < data.Count() - 1; i++)
                            {
                                f[i] = Convert.ToInt32(data[i + 1]);
                            }
                            Tris.Add(new Triangle(verts[f[0] - 1], verts[f[1] - 1], verts[f[2] - 1]));
                        }
                    }
                }
            }
        }
    }
}
