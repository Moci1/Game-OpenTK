using System;
using InternalSection;
using Entities;
using System.Drawing;

namespace Geometry.Shapes {
	// http://en.wikipedia.org/wiki/Minimum_bounding_rectangle
	// http://www.codeproject.com/Articles/22568/Computational-Geometry-C-and-Wykobi
	public interface IShape : IComparable, ICloneable {
		Circle BoundingCircle { get; }
		Rectangle BoundingRectangle { get; }
		void SetTransform(Transform t);
		bool Equals(object obj);

	}
	public class Circle : IShape
	{
		public float radius;
		public Vector2 center;
		
		public Circle(float radius, Vector2 center) {
			this.radius = radius;
			this.center = center;
		}

		public static bool operator ==(Circle c, Circle c1) {
			if (object.Equals(c, null) || object.Equals(c1, null))
				return false;
			if (System.Object.ReferenceEquals(c, c1))
        		return true;
			if (c.center == c1.center && c.radius == c1.radius)
				return true;
			return false;
		}
		public static bool operator !=(Circle c, Circle c1) {
			if (object.Equals(c, null) || object.Equals(c1, null))
				return false;
			if (!System.Object.ReferenceEquals(c, c1))
        		return true;
			if (c.center != c1.center && c.radius != c1.radius)
				return true;
			return false;
		}
		public override bool Equals(object obj) {
			if (obj is Circle) {
				Circle c = obj as Circle;
				if (c.center == this.center && c.radius == this.radius)
					return true;
			}
			return false;
		}
		public override int GetHashCode() {
			return radius.GetHashCode() ^ center.GetHashCode();
		}
		public int CompareTo(object other) {
			Circle c = other as Circle;
			if (c == null)
				return 1;
			double fi1 = Math.Atan2(center.Y, center.X);
			double fi2 = Math.Atan2(c.center.Y, c.center.X);
			float d = Vector2.Distance(c.center) - Vector2.Distance(center);
			if (d > 0) return 1;
			else if (d < 0) return -1;
			else {
				if (fi1 > fi2) return 1;
				else if (fi1 < fi2) return -1;
				else return 0;
			}// hát azé még nézd meg hogy jól raktam-e a <=> -ket
		}
		public override string ToString() {
			return string.Format("Circle: radius={0}, center={1}", radius, center);
		}
		
		public Rectangle BoundingasdRectangle() {
//			return new Rectangle((center.X - radius/2f), (center.Y - radius/2f),
//			                                  radius, radius);
			return new Rectangle();
		}

		#region IShape implementation
		public Circle BoundingCircle {
			get { return new Circle(radius, center); }
		}
		public Rectangle BoundingRectangle {
			get {
				int x = (int)(center.X - radius - 5), y = (int)(center.Y - radius - 5), wh = (int)(radius * 2f + 10);
				return new Rectangle(x, y, wh, wh);
			}
		}
		public void SetTransform(Transform t) {
			this.center = t.Translate;
			// a centert majd még el lehet forgatni, skálázni,
			// mátrixxal transzformálni...
		}
		#endregion

		#region ICloneable implementation

		public object Clone()
		{
			return this.MemberwiseClone();
		}

		#endregion


	}
}

