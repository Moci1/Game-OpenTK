using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace DeadXDown
{
	public class Spritebatch
	{// http://www.opentk.com/doc/graphics/how-to-render-text-using-opengl
		public Texture2D WhiteTexture { get; private set; }

		/// <summary>
		/// Enable: Texture2D, Lighting, LineSmooth
		/// </summary>
		public Spritebatch() { // Disable Enable
			GL.Enable(EnableCap.Texture2D);
			GL.Disable(EnableCap.Lighting);
			GL.Enable(EnableCap.LineSmooth);
			WhiteTexture = ContentPipe.CreateTexture2D();
			GL.BindTexture(TextureTarget.Texture2D, WhiteTexture.ID);
		}

		/// <summary>
		/// Contains GL.Begin and GL.End functions.
		/// </summary>
		public void DrawLines(float width, Vector2[] points) {
			GL.LineWidth(width);
			GL.BindTexture(TextureTarget.Texture2D, WhiteTexture.ID);
			GL.Begin(PrimitiveType.Lines);
			for (int i = 0; i < points.Length; i++) {
				GL.TexCoord2(points[i]);
				GL.Vertex2(points[i]);
			}
			GL.End();
			GL.LineWidth(1f);
		}
		public void DrawTexture(Texture2D texture, Vector2 pos, Vector2 scale, Color color, Vector2 origin) {
			int[] argb = null;
			//GL.GetInteger(GetPName.CurrentColor, argb);
			Vector2[] vertices = new Vector2[4] {
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(1, 1),
				new Vector2(0, 1)
			};
			GL.BindTexture(TextureTarget.Texture2D, texture.ID);
			GL.Color3(color);
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
		public void Begin(int screenWidth, int screenHeight) {
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			GL.Ortho(-screenWidth / 2f, screenWidth / 2f, screenHeight / 2f, -screenHeight / 2f, 0f, 1f);
		}

		public void DrawCircle(float x, float y, float radius, int segments, float width)
		{
			// http://slabode.exofire.net/circle_draw.shtml

			GL.LineWidth(width);
			GL.Begin(PrimitiveType.LineLoop);

			for (int i = 0; i < segments; i++)
			{
				float theta = (2.0f * (float)Math.PI * (float)i) / (float)segments;
				float xx = radius * (float)Math.Cos(theta);
				float yy = radius * (float)Math.Sin(theta);
				GL.Vertex2(x + xx, y + yy);
			}

			GL.End();
			GL.LineWidth(1f);
		}

		public void DrawLine(float x1, float y1, float x2, float y2, float width)
		{
			GL.LineWidth(width);
			GL.Begin(PrimitiveType.Lines);
			GL.Vertex2(x1, y1);
			GL.Vertex2(x2, y2);
			GL.End();
			GL.LineWidth(1f);
		}

		public void DrawBox(float x, float y, float size, float width)
		{
			GL.LineWidth(width);
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
			GL.LineWidth(1f);
		}
	}
}

