using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace TruckingGame
{
    class Mesh
    {
        private int _vao;

        public List<Vertex> Vertices;
        public uint[] Indices = [];
        public Box3 BoundingBox;

        private Matrix4 Transform = Matrix4.Identity;

        public Vector3 Position { get; set; } = Vector3.Zero;

        public Mesh(string objPath)
        {
            PrepareVertices(objPath);
            InitMesh();
        }

        private void PrepareVertices(string objPath)
        {
            Vertices = [.. ObjLoader.LoadMesh(objPath)];

            BoundingBox = new Box3();
            CalculateBoundingBox();

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
            _vao = GL.GenVertexArray();
            int vbo = GL.GenBuffer();

            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, Unsafe.SizeOf<Vertex>() * Vertices.Count, CollectionsMarshal.AsSpan<Vertex>(Vertices).ToArray(), BufferUsageHint.StaticDraw);

            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>(nameof(Vertex.Position)));

            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>(nameof(Vertex.Normal)));

            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Unsafe.SizeOf<Vertex>(), Marshal.OffsetOf<Vertex>(nameof(Vertex.TexCoord)));
        }

        private void CalculateBoundingBox()
        {
            Vector3 min = new(Vertices[0].Position.X, Vertices[0].Position.Y, Vertices[0].Position.Z);
            Vector3 max = new(Vertices[1].Position.X, Vertices[1].Position.Y, Vertices[1].Position.Z);

            foreach (Vertex v in Vertices)
            {
                min = Vector3.ComponentMin(min, v.Position);
                max = Vector3.ComponentMax(max, v.Position);
            }

            BoundingBox.Min = min;
            BoundingBox.Max = max;
        }

        private void UpdateBoundingBox()
        {
            Vector3 resultMin = new Vector3(
                    BoundingBox.Min.X * Transform.M11 + BoundingBox.Min.Y * Transform.M21 + BoundingBox.Min.Z * Transform.M31 + Transform.M41,
                    BoundingBox.Min.X * Transform.M12 + BoundingBox.Min.Y * Transform.M22 + BoundingBox.Min.Z * Transform.M32 + Transform.M42,
                    BoundingBox.Min.X * Transform.M13 + BoundingBox.Min.Y * Transform.M23 + BoundingBox.Min.Z * Transform.M33 + Transform.M43
                );

            Vector3 resultMax = new Vector3(
                    BoundingBox.Max.X * Transform.M11 + BoundingBox.Max.Y * Transform.M21 + BoundingBox.Max.Z * Transform.M31 + Transform.M41,
                    BoundingBox.Max.X * Transform.M12 + BoundingBox.Max.Y * Transform.M22 + BoundingBox.Max.Z * Transform.M32 + Transform.M42,
                    BoundingBox.Max.X * Transform.M13 + BoundingBox.Max.Y * Transform.M23 + BoundingBox.Max.Z * Transform.M33 + Transform.M43
                );

            BoundingBox.Min = resultMin;
            BoundingBox.Max = resultMax;
        }

        public void Update()
        {
            Transform = Matrix4.CreateTranslation(Position);

            UpdateBoundingBox();

        }

        public void Draw(Camera cam, Shader shader)
        {
            CalculateBoundingBox();

            shader.Use();

            GL.BindVertexArray(_vao);

            Matrix4 model = Transform;
            Matrix4 view = cam.GetViewMatrix();
            Matrix4 proj = cam.GetProjectionMatrix();

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("proj", proj);

  

            GL.DrawArrays(PrimitiveType.Triangles, 0, Vertices.Count);
            //GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}
