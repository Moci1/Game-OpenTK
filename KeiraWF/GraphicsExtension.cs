using System;
using System.Drawing;
using Geometry.Shapes;
using InternalSection;

namespace Keira {
	public static class GraphicsExtension
	{
		public static float fff  = 1f;
		public static void DrawArrow(this Graphics g, Vector2 a, Vector2 b, float intensity) {
			Pen p = new Pen(Brushes.Red, 3);
			Vector2 c = b+(b-a)*intensity;
			g.DrawLine(p, (Point)(a), (Point)(c));
			g.FillEllipse(p.Brush, c.X-3,c.Y-3,6,6);
		}
		public static void DrawCircle(this Graphics g, Pen p, Circle circle) {
			RectangleF rectF = new RectangleF(circle.center.X - circle.radius, circle.center.Y- circle.radius,
			                                  circle.radius * 2f, circle.radius * 2f);
			//g.DrawRectangle(p, rectF.X, rectF.Y, rectF.Width, rectF.Height);
			g.DrawEllipse(p, rectF);
		}
		public static void FillCircle(this Graphics g, Brush b, Circle circle) {
			RectangleF rectF = new RectangleF(circle.center.X - circle.radius, circle.center.Y- circle.radius,
			                                  circle.radius * 2f, circle.radius * 2f);
			//g.DrawRectangle(p, rectF.X, rectF.Y, rectF.Width, rectF.Height);
			g.FillEllipse(b, rectF);
		}
		public static RectangleF DrawCircleInRect(this Graphics g, Pen p, Circle circle) {
			RectangleF rectF = new RectangleF(circle.center.X - circle.radius, circle.center.Y- circle.radius,
			                                  circle.radius * 2f, circle.radius * 2f);
			g.DrawEllipse(p, rectF);
			return rectF;
		}
	}
}

