using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace TruckingGame
{
    class Texture
    {
		public readonly int Handle;

		public Texture(string texFilePath)
		{
			Handle = GL.GenTexture();

			GL.ActiveTexture(TextureUnit.Texture0);
			GL.BindTexture(TextureTarget.Texture2D, Handle);

			StbImage.stbi_set_flip_vertically_on_load(1);
			ImageResult image = ImageResult.FromStream(File.OpenRead(texFilePath), ColorComponents.RedGreenBlueAlpha);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
		}

		public void Use()
		{
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
