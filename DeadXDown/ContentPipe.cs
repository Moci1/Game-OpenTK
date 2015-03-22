using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace DeadXDown
{
	public class ContentPipe
	{
		/// <summary>
		/// Create texture with repeat and linear parameters.
		/// </summary>
		public static Texture2D CreateTexture2D(int width = 64, int height = 64) {
			int id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);
			Bitmap bmp = new Bitmap(width, height);
			for (int x = 0; x < bmp.Width; x++) {
				for (int y = 0; y < bmp.Width; y++) {
					bmp.SetPixel(x, y, Color.White);
				}
			}
			BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
			                               System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			bmp.UnlockBits(data);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			return new Texture2D(id, bmp.Width, bmp.Height);
		}
		public static Texture2D CreateTexture2D(TextureWrapMode textureWrapS, TextureWrapMode textureWrapT, TextureMinFilter textureMinFilter, TextureMagFilter textureMagFilter,
		                                        int width = 64, int height = 64) {
			int id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);
			Bitmap bmp = new Bitmap(width, height);
			for (int x = 0; x < bmp.Width; x++) {
				for (int y = 0; y < bmp.Width; y++) {
					bmp.SetPixel(x, y, Color.White);
				}
			}
			BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
			                               System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			bmp.UnlockBits(data);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)textureWrapS);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)textureWrapT);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)textureMinFilter);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)textureMagFilter);

			return new Texture2D(id, bmp.Width, bmp.Height);
		}
		public static Texture2D LoadTexture(string path) {
			if (!File.Exists("Content/" + path)) {
				throw new FileNotFoundException("File not found at 'Content/'" + path + "'");
			}

			int id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);

			Bitmap bmp = new Bitmap("Content/" + path);
			BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
			                               System.Drawing.Imaging.PixelFormat.Format32bppArgb);
			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
			bmp.UnlockBits(data);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Clamp);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Clamp);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			return new Texture2D(id, bmp.Width, bmp.Height);
		}
	}
}

