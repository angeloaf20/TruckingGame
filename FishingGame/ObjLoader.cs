using OpenTK.Mathematics;

namespace TruckingGame
{
    /// <summary>
    /// Converts a triangulated mesh to a List of type Vertex
    /// </summary>
    /// <param name="objPath"></param>
    static class ObjLoader
    {
        public static List<Vertex> LoadMesh(string objPath)
        {
            List<Vertex> vertices = [];

            List<Material> materials = [];
            
            Dictionary<string, Material> materialNamePairs = [];

            string mtlPath = objPath.Replace("obj", "mtl");

            ReadMtlFile(mtlPath, ref materialNamePairs);

            ReadObjFile(objPath, ref vertices);

            // add an exception: 
            // 1. there was never any mtl file because the mesh/model has no materials
            // 2. mtl file has a different name
            // 3. mtl file is in a diff path
            // 4. mtl file was not included in project

            return vertices;
        }

        private static void ReadMtlFile(string mtlPath, ref Dictionary<string, Material> materials)
        {
            string currentMaterialName = "";

            try
            {
                StreamReader mtlFileReader = File.OpenText(mtlPath);

                while (!mtlFileReader.EndOfStream)
                {
                    string[] data = mtlFileReader.ReadLine()!.Split(' ');
                    Material currentMat = new Material();

                    if (data[0] == "newmtl")
                    {
                        currentMaterialName = data[1];
                    }

                    if (data[0] == "Ns")
                    {
                        if (float.TryParse(data[1], out float value)) currentMat.Shininess = value;
                    }

                    if (data[0] == "Ka")
                    {
                        if (float.TryParse(data[1], out float x) && float.TryParse(data[2], out float y) && float.TryParse(data[3], out float z))
                            currentMat.Ambient = new Vector3(x, y, z);

                        Console.WriteLine(currentMat.Ambient.X + " " + currentMat.Ambient.Y + " " + currentMat.Ambient.Z);
                    }

                    if (data[0] == "Ks")
                    {
                        if (float.TryParse(data[1], out float x) && float.TryParse(data[2], out float y) && float.TryParse(data[3], out float z))
                            currentMat.Specular = new Vector3(x, y, z);
                    }

                    if (data[0] == "Kd")
                    {
                        if (float.TryParse(data[1], out float x) && float.TryParse(data[2], out float y) && float.TryParse(data[3], out float z))
                            currentMat.Diffuse = new Vector3(x, y, z);
                    }

                    if (data[0] == "illum")
                    {
                        if (int.TryParse(data[1], out int value)) currentMat.Illumination = value;
                    }

                    if (data[0] == "map_Kd")
                    {
                        // make the file path local just in case (?)
                        //string[] newPath = data[1].Split('/');
                        currentMat.TextureMap = data[1];

                        materials.Add(currentMaterialName, currentMat);
                    }
                }
            }
            catch (FileNotFoundException fnfe)
            {
                Console.Error.WriteLine(fnfe.Message);
            }
        }

        private static void ReadObjFile(string objPath, ref List<Vertex> vertices)
        {
            List<Vector3> positions = [];
            List<Vector3> normals = [];
            List<Vector2> texCoords = [];

            try
            {
                StreamReader objFileReader = File.OpenText(objPath);

                while (!objFileReader.EndOfStream)
                {
                    GetDataFromLine(objFileReader.ReadLine(), ref positions, ref normals, ref texCoords, ref vertices);
                }

            }
            catch (FileNotFoundException fnfe)
            {
                Console.Error.WriteLine(fnfe.Message);
            }
        }

        private static void GetDataFromLine(string line, ref List<Vector3> positions, ref List<Vector3> normals, ref List<Vector2> texCoords, ref List<Vertex> vertices)
        {
            string currentMaterial = "";
            string[] data = line.Split(' ');

            if (data[0] == "v")
            {
                if (float.TryParse(data[1], out float x) && float.TryParse(data[2], out float y) && float.TryParse(data[3], out float z))
                {
                    positions.Add(new Vector3(x, y, z));
                }
            }

            if (data[0] == "vt")
            {
                if (float.TryParse(data[1], out float u) && float.TryParse(data[2], out float v))
                {
                    texCoords.Add(new Vector2(u, v));
                }
            }

            if (data[0] == "vn")
            {
                if (float.TryParse(data[1], out float x) && float.TryParse(data[2], out float y) && float.TryParse(data[3], out float z))
                {
                    normals.Add(new Vector3(x, y, z));
                }
            }

            

            if (data[0] == "f")
            {
                for (int i = 1; i < data.Length; i++)
                {
                    var point = data[i].Split('/');

                    for (int j = 0; j < point.Length; j += 3)
                    {
                        if (int.TryParse(point[j], out int x) && int.TryParse(point[j + 1], out int y) && int.TryParse(point[j + 2], out int z))
                        {
                            var pos = positions[x - 1];
                            var tex = texCoords[y - 1];
                            var norms = normals[z - 1];

                            var vertex = new Vertex(pos, norms, tex);
                            vertices.Add(vertex);
                        }
                    }
                }
            }
        }
    }
}
