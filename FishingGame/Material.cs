using OpenTK.Mathematics;

namespace TruckingGame
{
    public struct Material(Vector3 ambient, Vector3 diffuse, Vector3 specular, float alpha, float shininess, int illumination, string textureMap)
    {
        public Vector3 Ambient = ambient;
        public Vector3 Diffuse = diffuse;
        public Vector3 Specular = specular;
        public float? Shininess = shininess;
        public int? Illumination = illumination;
        public string? TextureMap = textureMap;
    }
}
