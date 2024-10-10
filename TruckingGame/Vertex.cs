using OpenTK.Mathematics;

namespace TruckingGame
{
    public struct Vertex(Vector3 position, Vector3 normal, Vector2 texCoord)
    {
        public Vector3 Position = position;
        public Vector3 Normal = normal;
        public Vector2 TexCoord = texCoord;
    }
}
