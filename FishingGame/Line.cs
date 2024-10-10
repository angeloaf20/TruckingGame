using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace TruckingGame
{
    class Line
    {
        public Vector3 Min;
        public Vector3 Max;

        private readonly float[] _lineVertices;
        private int _lineVao;

        private Shader _lineShader;

        public Line(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;

            _lineVertices = [ Min.X, Min.Y, Min.Z, Max.X, Max.Y, Max.Z ];

            InitLine();
        }

        private void InitLine()
        {
            _lineVao = GL.GenVertexArray();
            int linesVbo = GL.GenBuffer();
            GL.BindVertexArray(_lineVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, linesVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _lineVertices.Length * sizeof(float), _lineVertices, BufferUsageHint.StaticDraw);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            _lineShader = new Shader("Shaders/Debug/DebugLine.vert", "Shaders/Debug/DebugLine.frag");
        }

        public void Draw(Camera cam)
        {
            _lineShader.Use();
            _lineShader.SetMatrix4("model", Matrix4.Identity);
            _lineShader.SetMatrix4("view", cam.GetViewMatrix());
            _lineShader.SetMatrix4("proj", cam.GetProjectionMatrix());

            GL.BindVertexArray(_lineVao);
            GL.DrawArrays(PrimitiveType.Lines, 0, _lineVertices.Length);
        }
    }
}
