using System;

namespace InternalSection {
	public struct Vector2 {
		public float X, Y;
		
		public Vector2(float x, float y) {
			X = x; Y = y;
		}
		public int Quadrant {
			get {
				if (X >= 0 && Y >= 0)
					return 1;
				if (X <= 0 && Y >= 0)
					return 2;
				if (X <= 0 && Y <= 0)
					return 3;
				if (X >= 0 && Y <= 0)
					return 4;
				return -1;
			}
		}
		public static Vector2 MinSideBySide(Vector2 source, Vector2 x, Vector2 y)
		{
			Vector2 v1 = x - source;
			Vector2 v2 = y - source;

			float a1 = Vector2.Angle(Vector2.Normailze(v1));
			float a2 = Vector2.Angle(Vector2.Normailze(v2));
			float mod = Math.Abs((a1 - a2) % 360f);

			if (mod < 10f || mod > 349f) {
				if (Vector2.Distance(v1) < Vector2.Distance(v2))
					return x;
				else
					return y;
			}
			return Vector2.Zero;
		}
		public static float Distance(Vector2 v) {
			return (float)Math.Sqrt(v.X*v.X+v.Y*v.Y);
		}
		public static Vector2 Normailze(Vector2 v) {
			float d = Distance(v);
			if (d != 0) {
				v.X /= d;
				v.Y /= d;
			}
			return v;
		}
		public static float Angle(Vector2 v) {
			double radian = 0.0;
			if (v.X > 0 && v.Y >= 0) 
				radian = Math.Atan(v.Y / v.X);
			else if (v.X <= 0 && v.Y >= 0)
				radian = Math.PI - Math.Abs(Math.Atan(v.Y / v.X));
			else if (v.X <= 0 && v.Y <= 0)
				radian = Math.PI + Math.Abs(Math.Atan(v.Y / v.X));
			else if (v.X > 0 && v.Y < 0)
				radian = Math.PI * 2.0 - Math.Abs(Math.Atan(v.Y / v.X));

			return (float)(radian * (180.0 / Math.PI));
		}
		public static float Radian(Vector2 v) {
			double radian = 0.0;
			if (v.X > 0 && v.Y >= 0) 
				radian = Math.Atan(v.Y / v.X);
			else if (v.X <= 0 && v.Y >= 0)
				radian = Math.PI - Math.Abs(Math.Atan(v.Y / v.X));
			else if (v.X <= 0 && v.Y <= 0)
				radian = Math.PI + Math.Abs(Math.Atan(v.Y / v.X));
			else if (v.X > 0 && v.Y < 0)
				radian = Math.PI * 2.0 - Math.Abs(Math.Atan(v.Y / v.X));

			return (float)radian;
		}
		public static float Dot(Vector2 v1, Vector2 v2) {
			float dot = (v1.X * v2.X) + (v1.Y * v2.Y);
			dot = (float)Math.Acos(dot / (Distance(v1) * Distance(v2)));
			if (float.IsInfinity(dot) || float.IsNaN(dot))
				return 0f;
			return dot;
		}
		public static Vector2 Rotate(Vector2 v, float rot) {
			float sin = (float)Math.Sin(rot);
			float cos = (float)Math.Cos(rot);
			return new Vector2(v.X*cos - v.Y*sin,
			                   v.X*sin + v.Y*cos);
		}
		public static Vector2 Rotate(Vector2 v, float rot, Vector2 origin) {
			float sin = (float)Math.Sin(rot);
			float cos = (float)Math.Cos(rot);
			Vector2 res = v - origin;
			Vector2 vv = new Vector2(res.X*cos - res.Y*sin, res.X*sin + res.Y*cos);
			return vv + origin;
//			return new Vector2((v.X-origin.X)*cos - (v.Y-origin.Y)*sin + origin.X,
//			                   (v.X-origin.X)*sin + (v.Y-origin.Y)*cos + origin.Y);
		}
//		public static Vector2 Transform(Vector2 position, Matrix3x3 matrix) {
//			return new Vector2();
//		}
//		public static Vector2 Transform(ref Vector2 v1, ref Matrix3x3 m, out Vector2 v2) {
//			v2 = new Vector2();
//			return new Vector2();
//		}
		public static Vector2 Min(Vector2 v1, Vector2 v2) {
			return v2;	
		}
		public static Vector2 Max(Vector2 v1, Vector2 v2) {
			return v2;	
		}
//		public static Vector2 TransformNormal(Vector2 normal, Matrix3x3 matrix) {
//			return new Vector2();	
//		}
		public static Vector2 operator -(Vector2 v1, Vector2 v2) {
			return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
		}
		public static Vector2 operator +(Vector2 v1, Vector2 v2) {
			return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
		}
		public static Vector2 operator -(Vector2 v) {
			return new Vector2(-v.X, -v.Y);
		}
		public static Vector2 operator +(Vector2 v) {
			return new Vector2(v.X, v.Y);
		}
		public static Vector2 operator -(Vector2 v, float num) {
			return new Vector2(v.X - num, v.Y - num);
		}
		public static Vector2 operator +(Vector2 v, float num) {
			return new Vector2(v.X + num, v.Y + num);
		}
		public static Vector2 operator *(Vector2 v, float num) {
			return new Vector2(v.X * num, v.Y * num);
		}
		public static Vector2 operator /(Vector2 v, float num) {
			return new Vector2(v.X / num, v.Y / num);
		}
		public static explicit operator System.Drawing.Point(Vector2 v) {
			return new System.Drawing.Point((int)v.X, (int)v.Y);
		}
		public static bool operator ==(Vector2 v1, Vector2 v2) {
			if (v1.X == v2.X && v1.Y == v2.Y)
				return true;
			else
				return false;
		}
		public static bool operator !=(Vector2 v1, Vector2 v2) {
			if (v1.X == v2.X && v1.Y == v2.Y)
				return false;
			else
				return true;
		}
		public static Vector2 UnitX { get { return new Vector2(1,0); } }
		public static Vector2 UnitY { get { return new Vector2(0,1); } }
		public static Vector2 Zero = new Vector2();
		
		public override string ToString() {
			return string.Format("[X={0};Y={1}]", X,Y);
		}
	}
}

