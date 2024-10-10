using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace TruckingGame
{
    class Mesh
    {
        private int _vao;

        public float[] Vertices = [];
        public uint[] Indices = [];
        public Box3 BoundingBox;

        public Vector3 Position { get; set; } = Vector3.Zero;

        public Mesh(string objPath)
        {
            PrepareVertices(objPath);
            InitMesh();
        }

        private void PrepareVertices(string objPath)
        {
            Vertex[] initialVerts = [.. ObjLoader.LoadMesh(objPath)];

            List<float> floats = [];


            foreach (var vertex in initialVerts)
            {
                floats.Add(vertex.Position.X);
                floats.Add(vertex.Position.Y);
                floats.Add(vertex.Position.Z);

                floats.Add(vertex.Normal.X);
                floats.Add(vertex.Normal.Y);
                floats.Add(vertex.Normal.Z);

                floats.Add(vertex.TexCoord.X);
                floats.Add(vertex.TexCoord.Y);
            }

            Vertices = [.. floats.ToArray()];

            /*
            Vertices = [
                1.0f, 1.0f, 1.0f,       // right top forward
                1.0f, -1.0f, 1.0f,      // right bottom forward
                -1.0f, -1.0f, 1.0f,     // left bottom forward
                -1.0f, 1.0f, 1.0f,      // left top forward
                1.0f, 1.0f, -1.0f,      // right top backward
                1.0f, -1.0f, -1.0f,     // right bottom backward
                -1.0f, -1.0f, -1.0f,    // left bottom backward
                -1.0f, 1.0f, -1.0f,     // left top backward
            ];

            Indices = [
                0, 1, 2, //front
                2, 3, 0,

                0, 1, 4, //right
                1, 4, 5,

                4, 5, 6, //back
                6, 7, 4,

                2, 3, 7, //left
                2, 6, 7,

                0, 4, 7, //top
                0, 3, 7,

                1, 2, 5, //bottom
                2, 5, 6
            ];
            */
        }

        private void InitMesh()
        {
            BoundingBox = new Box3();
            (BoundingBox.Min, BoundingBox.Max) = CalculateBoundingBox();

            _vao = GL.GenVertexArray();
            int vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * sizeof(float), Vertices, BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 3 * sizeof(float));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 2 * sizeof(float));
        }

        private (Vector3, Vector3) CalculateBoundingBox()
        {
            Vector3 min = new(Vertices[0], Vertices[1], Vertices[2]);
            Vector3 max = new(Vertices[3], Vertices[4], Vertices[5]);

            for (int i = 5; i < Vertices.Length; i += 5)
            {
                min = Vector3.ComponentMin(min, new(Vertices[i], Vertices[i + 1], Vertices[i + 2]));
                max = Vector3.ComponentMax(max, new(Vertices[i], Vertices[i + 1], Vertices[i + 2]));
            }

            return (min, max);
        }

        public void Draw(Camera cam, Shader shader)
        {
            shader.Use();

            GL.BindVertexArray(_vao);

            Matrix4 model = Matrix4.CreateTranslation(Position);
            Matrix4 view = cam.GetViewMatrix();
            Matrix4 proj = cam.GetProjectionMatrix();

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("proj", proj);

            GL.DrawArrays(PrimitiveType.Triangles, 0, Vertices.Length);
            //GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
