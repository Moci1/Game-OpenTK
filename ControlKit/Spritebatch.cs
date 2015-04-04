using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;

namespace glControlKit
{
	public class Spritebatch
	{// http://www.opentk.com/doc/graphics/how-to-render-text-using-opengl
		public Texture2D WhiteTexture { get; private set; }
		Color color;
		public Color ColorBrush {
			get {
				return color;
			}
			set {
				color = value;
				GL.Color4(color); // hogy van akko most az átlátszoság
			}
		}
		public float PenWidth { get; set; }

		/// <summary>
		/// Enable: Texture2D, Lighting, LineSmooth
		/// </summary>
		public Spritebatch() { // Disable Enable
			GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
			GL.Enable(EnableCap.Texture2D);
			GL.Disable(EnableCap.Lighting);
			GL.Enable(EnableCap.LineSmooth);
			WhiteTexture = ContentPipe.CreateTexture2D();
			GL.BindTexture(TextureTarget.Texture2D, WhiteTexture.ID);
		}

		/// <summary>
		/// Contains GL.Begin and GL.End functions.
		/// </summary>
		public void DrawLines(Vector2[] points) {
			GL.Disable(EnableCap.Blend);
			GL.LineWidth(PenWidth);
			GL.BindTexture(TextureTarget.Texture2D, WhiteTexture.ID);
			GL.Begin(PrimitiveType.Lines);
			for (int i = 0; i < points.Length; i++) {
				GL.TexCoord2(points[i]);
				GL.Vertex2(points[i]);
			}
			GL.End();
			GL.Enable(EnableCap.Blend);
		}
		public void DrawLines(PointF[] points) {
			GL.Disable(EnableCap.Blend);
			GL.LineWidth(PenWidth);
			GL.BindTexture(TextureTarget.Texture2D, WhiteTexture.ID);
			GL.Begin(PrimitiveType.Lines);
			for (int i = 0; i < points.Length; i++) {
				GL.TexCoord2(points[i].X, points[i].Y);
				GL.Vertex2(points[i].X, points[i].Y);
			}
			GL.End();
		}
		public static int LoadTexture(string filename)
		{
			if (String.IsNullOrEmpty(filename))
				throw new ArgumentException(filename);

			int id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);

			Bitmap bmp = new Bitmap(filename);
			BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
			              OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

			bmp.UnlockBits(bmp_data);

			// We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
			// On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
			// mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			return id;
		}
		public static int LoadTexture(string filename, TextureWrapMode wrapMode)
		{
			if (String.IsNullOrEmpty(filename))
				throw new ArgumentException(filename);

			int id = GL.GenTexture();
			GL.BindTexture(TextureTarget.Texture2D, id);

			Bitmap bmp = new Bitmap(filename);
			BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
			              OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);

			bmp.UnlockBits(bmp_data);

			// We haven't uploaded mipmaps, so disable mipmapping (otherwise the texture will not appear).
			// On newer video cards, we can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
			// mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
			GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

			return id;
		}
		public void DrawTexture(Texture2D texture, Vector2 pos, Vector2 scale, Vector2 origin) {
			GL.Enable(EnableCap.Blend);
			int[] argb = null;
			//GL.GetInteger(GetPName.CurrentColor, argb);
			Vector2[] vertices = new Vector2[4] {
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1)
			};
			GL.BindTexture(TextureTarget.Texture2D, texture.ID);
			GL.Begin(PrimitiveType.Quads);
			for (int i = 0; i < vertices.Length; i++) {
				GL.TexCoord2(vertices[i]);
				vertices[i].X *= texture.Width;
				vertices[i].Y *= texture.Height;
				vertices[i] -= origin;
				vertices[i] *= scale;
				vertices[i] += pos;
				GL.Vertex2(vertices[i]);
			}
			GL.End();
			//GL.Color3(Color.FromArgb(argb[0], argb[1], argb[2], argb[3]));
		}
		/// <summary>
		/// Contains the GL.LoadIdentity() and set projection.
		/// </summary>
		public void ResetScreen(float screenWidth, float screenHeight) {
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(-screenWidth / 2f, screenWidth / 2f, screenHeight / 2f, -screenHeight / 2f, 0f, 1f);
		}
		public void ResetView() {
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
//			GL.Ortho(-screenWidth / 2f, screenWidth / 2f, screenHeight / 2f, -screenHeight / 2f, 0f, 1f);
		}
		public void DrawPolygon(float x, float y, float radius, int segments)
		{
			// http://slabode.exofire.net/circle_draw.shtml

			GL.Disable(EnableCap.Blend);
			GL.LineWidth(PenWidth);
			GL.Begin(PrimitiveType.LineLoop);

			for (int i = 0; i < segments; i++)
			{
				float theta = (2.0f * (float)Math.PI * (float)i) / (float)segments;
				float xx = radius * (float)Math.Cos(theta);
				float yy = radius * (float)Math.Sin(theta);
				GL.Vertex2(x + xx, y + yy);
			}

			GL.End();
		}

		public void DrawLine(float x1, float y1, float x2, float y2)
		{
			GL.Disable(EnableCap.Blend);
			GL.LineWidth(PenWidth);
			GL.Begin(PrimitiveType.Lines);
			GL.Vertex2(x1, y1);
			GL.Vertex2(x2, y2);
			GL.End();
		}
		public void DrawLine(PointF x1, PointF x2)
		{
			GL.Disable(EnableCap.Blend);
			GL.LineWidth(PenWidth);
			GL.Begin(PrimitiveType.Lines);
			GL.Vertex2(x1.X, x1.Y);
			GL.Vertex2(x2.X, x2.Y);
			GL.End();
		}
		public void DrawBox(float x, float y, float size)
		{
			GL.Disable(EnableCap.Blend);
			GL.LineWidth(PenWidth);
			float half = (size / 2f);
			float x1 = x - half;
			float y1 = y - half;
			float x2 = x + half;
			float y2 = y + half;

			GL.Begin(PrimitiveType.Lines);
			GL.Vertex2(x1, y1); GL.Vertex2(x2, y1); // bottom
			GL.Vertex2(x1, y2); GL.Vertex2(x2, y2); // upper
			GL.Vertex2(x1, y1); GL.Vertex2(x1, y2); // left
			GL.Vertex2(x2, y1); GL.Vertex2(x2, y2); // right
			GL.End();
		}
	}
}

