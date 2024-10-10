using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace TruckingGame
{
    public class Shader
    {
        private readonly Dictionary<string, int> _uniformLocs = [];
        private readonly int _shaderProgram;
        public int Program => _shaderProgram;

        public Shader(string vertexPath, string fragPath)
        {
            string vertexSource = File.ReadAllText(vertexPath);
            string fragmentSource = File.ReadAllText(fragPath);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);

            _shaderProgram = GL.CreateProgram();
            GL.AttachShader(_shaderProgram, vertexShader);
            GL.AttachShader(_shaderProgram, fragmentShader);

            GL.LinkProgram(_shaderProgram);

            GL.GetProgram(_shaderProgram, GetProgramParameterName.LinkStatus, out int success);

            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(_shaderProgram);
                Console.Error.WriteLine(infoLog);
                return;
            }

            CleanUp(vertexShader, fragmentShader);

            GL.GetProgram(_shaderProgram, GetProgramParameterName.ActiveUniforms, out int numUniforms);



            for (int i = 0; i < numUniforms; i++)
            {
                var key = GL.GetActiveUniform(_shaderProgram, i, out _, out _);

                var location = GL.GetUniformLocation(_shaderProgram, key);

                _uniformLocs.Add(key, location);

                Console.WriteLine($"{key} {location}");
            }
        }

        public void Use()
        {
            GL.UseProgram(_shaderProgram);
        }
        
        public void Delete()
        {
            GL.DeleteProgram(_shaderProgram);
        }

        public void SetInt(string uniform, int value)
        {
            Use();
            GL.Uniform1(_uniformLocs[uniform], value);
        }

        public void SetFloat(string uniform, float value)
        {
            Use();
            GL.Uniform1(_uniformLocs[uniform], value);
        }

        public void SetVector3(string uniform, Vector3 value)
        {
            Use();
            GL.Uniform3(_uniformLocs[uniform], value);
        }

        public void SetMatrix4(string uniform, Matrix4 value)
        {
            Use();
            GL.UniformMatrix4(_uniformLocs[uniform], false, ref value);
        }

        //public void SetSampler2D(string uniform, )

        private void CleanUp(int vert, int frag)
        {
            GL.DetachShader(_shaderProgram, vert);
            GL.DetachShader(_shaderProgram, frag);
            GL.DeleteShader(frag);
            GL.DeleteShader(vert);
        }
    }
}
