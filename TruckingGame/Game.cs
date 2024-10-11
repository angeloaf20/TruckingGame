using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace TruckingGame
{
    public class Game(int width, int height, string title) : GameWindow(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title })
    {
        private readonly float[] Vertices =
        {
             // Position          Normal
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f, // Front face
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f, // Back face
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f, // Left face
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f, // Right face
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f, // Bottom face
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f, // Top face
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f
        };

        //private readonly uint[] Indices = [
        //        0, 1, 2, //front
        //        2, 3, 0,

        //        0, 1, 4, //right
        //        1, 4, 5,

        //        4, 5, 6, //back
        //        6, 7, 4,

        //        2, 3, 7, //left
        //        2, 6, 7,

        //        0, 4, 7, //top
        //        0, 3, 7,

        //        1, 2, 5, //bottom
        //        2, 5, 6
        //    ];

        //private int lightVao; // mesh that "shines"
        //private int lightEbo;

        //private int objectVao; // mesh that the light hits
        //private int objectEbo;

        private Mesh lightMesh;
        private Mesh objectMesh;
        private Mesh suzanneMesh;

        private Line objectMeshBboxLine;
        private Line suzanneBboxLine;

        private readonly Camera _camera = new(Vector3.Zero, width / height);

        private Shader lightShader;
        private Shader objectShader;
        //private Shader defaultShader;

        private Texture truckTexture;

        private float objMove = -5;

        protected override void OnLoad()
        {
            Console.WriteLine("Loading assets");

            //GL.ClearColor(0.75f, 0.75f, 0.75f, 1.0f);
            GL.Enable(EnableCap.DepthTest);
            // GL.CullFace(CullFaceMode.Back);

            lightShader = new Shader("Shaders/VertShader.glsl", "Shaders/LightShader.glsl");
            objectShader = new Shader("Shaders/VertShader.glsl", "Shaders/ObjectShader.glsl");
            //defaultShader = new Shader("Shaders/VertShader.glsl", "Shaders/FragShader.glsl");

            lightMesh = new Mesh("Assets/cube.obj");

            objectMesh = new Mesh("Assets/Truck/truck.obj");
            truckTexture = new Texture("Assets/Truck/31DABC74.png");

            suzanneMesh = new Mesh("Assets/suzanne.obj");

            objectMesh.Position = new Vector3(-10, 0, 0);

            objectMeshBboxLine = new Line(objectMesh.BoundingBox.Min, objectMesh.BoundingBox.Max);
            suzanneBboxLine = new Line(suzanneMesh.BoundingBox.Min, suzanneMesh.BoundingBox.Max);
            
            CursorState = CursorState.Grabbed;

            _camera.Position = new Vector3(0, 0, 2);

            base.OnLoad();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            if (KeyboardState.IsKeyPressed(Keys.Escape))
            {
                Close();
            }

            if (KeyboardState.IsKeyDown(Keys.Right))
            {
                objMove += 0.005f;
                objectMesh.Position = new Vector3(objMove, 0, 0);
            }

            if (KeyboardState.IsKeyDown(Keys.Left))
            {
                objMove -= 0.005f;
                objectMesh.Position = new Vector3(objMove, 0, 0);
            }

            float mouseX = MouseState.Delta.X * 0.05f;
            float mouseY = MouseState.Delta.Y * 0.05f;

            _camera.UpdateRotation(mouseX, mouseY);
            _camera.UpdateMovement(KeyboardState, (float)args.Time);

            objectMesh.Update();

            if (objectMesh.BoundingBox.Contains(suzanneMesh.BoundingBox))
            {
                Console.WriteLine("Colliding");
            }

            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            lightShader.Use();
            lightMesh.Draw(_camera, lightShader);

            objectShader.Use();
            objectShader.SetVector3("lightColor", new Vector3(1f));
            objectShader.SetVector3("lightPos", new Vector3(-10f, 5f, 0f));
            objectShader.SetVector3("objColor", new Vector3(0.2f, 0.1f, 1.0f));
            objectShader.SetVector3("viewPos", _camera.Position);

            objectMesh.Draw(_camera, objectShader);

            suzanneMesh.Draw(_camera, objectShader);

            SwapBuffers();
            base.OnRenderFrame(args);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        protected override void OnUnload()
        {
            Console.WriteLine("Closing game");
            // _shader.Delete();
            base.OnUnload();
        }
    }
}