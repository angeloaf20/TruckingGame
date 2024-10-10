using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System.Collections.Generic;

namespace TruckingGame
{
    class Texture
    {
		public readonly int Handle;

		public Texture(string texFilePath)
		{
			Handle = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, Handle);

			StbImage.stbi_set_flip_vertically_on_load(1);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            ImageResult image = ImageResult.FromStream(File.OpenRead(texFilePath), ColorComponents.RedGreenBlueAlpha);

            if (image.Data != null)
            {
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
        }

		public void Use(TextureUnit unit)
		{
            GL.ActiveTexture(unit);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }
    }
}
