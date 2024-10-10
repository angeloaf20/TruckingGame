using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace TruckingGame
{
    public class Camera
    {
        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;

        private float _pitch;

        private float _yaw = -MathHelper.PiOver2;

        private readonly float _fov = MathHelper.DegreesToRadians(110);

        public Camera(Vector3 position, float aspectRatio)
        {
            Position = position;
            AspectRatio = aspectRatio;
        }

        public Vector3 Position { get; set; }

        public float AspectRatio { private get; set; }

        public Vector3 Front => _front;
        public Vector3 Up => _up;
        public Vector3 Right => _right;

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _front, _up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 1000f);
        }

        public void UpdateRotation(float mouseX, float mouseY)
        {
            float radMouseX = MathHelper.DegreesToRadians(mouseX);
            float radMouseY = MathHelper.DegreesToRadians(mouseY);

            _yaw += radMouseX;
            _pitch -= radMouseY;

            _pitch = MathHelper.Clamp(_pitch, MathHelper.DegreesToRadians(-89f), MathHelper.DegreesToRadians(89f));

            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            _front = Vector3.Normalize(_front);

            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        public void UpdateMovement(KeyboardState keyboard, float time)
        {
            if (keyboard.IsKeyDown(Keys.W))
            {
                Position += Front * 2 * time;
            }

            if (keyboard.IsKeyDown(Keys.S))
            {
                Position -= Front * 2 * time;
            }

            if (keyboard.IsKeyDown(Keys.A))
            {
                Position -= Right * 2 * time;
            }

            if (keyboard.IsKeyDown(Keys.D))
            {
                Position += Right * 2 * time;
            }

            if (keyboard.IsKeyDown(Keys.Space))
            {
                Position += Up * 2 * time;
            }
        }
    }
}
