using System;
using Geometry.Shapes;

namespace InternalSection {
	public static class MathHelper
	{
		public static void ComplexToInt(float x, float y) {
			
		}
		/// <summary>
		/// (+-x+-y)^2
		/// </summary>
		public static void QuadraticEquation(float x, float constant, out float? x1, out float? x2) {
			float a = x * x;
			float b = 2f * x * constant;
			float c = constant * constant;
			float D = (float)Math.Sqrt(b * b - 4 * a * c);
			if (D >= 0) {
				x1 = (-b + D) / (2*a);
				x2 = (-b - D) / (2*a);
			}
			else {
				x1 = null; x2 = null;
			}
		}
        // a^2+2ab+c^2
		public static void QuadraticEquation(float x, float constant, out float a, out float b, out float c) {
			a = x * x;
			b = 2f * x * constant;
			c = constant * constant;
		}
		public static void QuadraticEquation(float a, float b, float c, out float? x1, out float? x2) {
			float asd = 4f * a * c, dsa = b * b;
			float D = (b * b) - (4f * a * c);
			if (D >= 0) {
				x1 = (-b + (float)Math.Sqrt(D)) / (2*a);
				x2 = (-b - (float)Math.Sqrt(D)) / (2*a);
			}
			else {
				x1 = null; x2 = null;
			}
		}
		/// <summary>
		/// 'a' és 'b' oldal által közbezárt gamma szög.
		/// </summary>
		public static float CosinesAngle(float a, float b, float c) {
			float gammaInv = (a*a + b*b - c*c) / (2f * a * b);
			if (gammaInv < -1)
				return (float)Math.PI;
			else if (gammaInv > 1)
				return 0f;
			else
				return (float)Math.Acos(gammaInv);
		}
		/// <summary>
		/// Harmadik oldal meghatározása.
		/// </summary>
		public static float CosinesSide(float a, float b, float angle) {
			return (float)Math.Sqrt(a*a + b*b - 2f * a * b * Math.Cos(angle));
		}
		public static float MinDistance(Line ln, Vector2 v) {
			Vector2 norm = new Vector2(-ln.Direction.Y, ln.Direction.X);
			float m = (float)Math.Atan(norm.Y / norm.X);
			float b = v.Y - m * v.X; // ráilleszti a v pontra az m meredekséget
			Line pointLine = new Line(m, b); // és csinál belőle egyenest
			Vector2 iPoint = Collision.Intersect(ln, pointLine); // már abszolút jó helyen van a pointLine
			float ads = Vector2.Distance(v - iPoint);
			return ads;
		}
		public static Line CalcNormal(Line ln1, Line ln2) {
			Vector2 v1, v2;
			if (ln1.Start == ln2.Start || ln1.End == ln2.End) {
				v1 = Vector2.Normailze(ln1.End - ln1.Start);
				v2 = Vector2.Normailze(ln2.End - ln2.Start);
			}
			else {
				v1 = Vector2.Normailze(ln1.Start - ln1.End);
				v2 = Vector2.Normailze(ln2.End - ln2.Start);
			}
			v1 = (v1 + v2)*40f;
			Vector2 start = Collision.Intersect(ln1, ln2);
			return new Line(start, start + v1);
		}
	}
}

