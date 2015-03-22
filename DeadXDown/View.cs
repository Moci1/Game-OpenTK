using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace DeadXDown
{
	public class View
	{
		public Vector2 Position;
		public double Rotation;
		public double Zoom;
		public View(Vector2 startPos, double startZoom = 1.0, double startRotation = 0.0)
		{
			this.Position = startPos;
			this.Zoom = startZoom;
			this.Rotation = startRotation;
		}
		public void Update() {

		}
		public void ApplyTransform() {
			Matrix4 transform = Matrix4.Identity;
			transform = Matrix4.Mult(transform, Matrix4.CreateTranslation(-Position.X, -Position.Y, 0));
			transform = Matrix4.Mult(transform, Matrix4.CreateRotationZ(-(float)Rotation));
			transform = Matrix4.Mult(transform, Matrix4.CreateScale((float)Zoom, (float)Zoom, 1.0f));

			GL.MultMatrix(ref transform);
		}
	}
}

