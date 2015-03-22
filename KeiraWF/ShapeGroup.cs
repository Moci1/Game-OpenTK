using System;
using System.Collections;
using System.Collections.Generic;
using InternalSection;
using Entities;
using System.Drawing;

namespace Geometry.Shapes {
	public class ShapeGroup : IShape
	{
		public List<IShape> Members { get; private set; }
		private Circle boundingCircle;

		public ShapeGroup(params IShape[] elements)
		{
			Members = new List<IShape>();
			Members.AddRange(elements);
			CalcBoundingCircle();
		}

		#region IComparable implementation
		public int CompareTo(object obj) {
			return 0;
		}
		#endregion

		void CalcBoundingCircle() {
			Vector2 min = new Vector2(99999f, 999999f), max = new Vector2();
			for (int i = 0; i < Members.Count; i++) {
				Circle ci = Members[i].BoundingCircle;
				float nx = ci.center.X - ci.radius;
				float ny = ci.center.Y - ci.radius;
				if (nx < min.X)
					min.X = nx;
				if (ny < min.Y)
					min.Y = ny;
				float xx = ci.center.X + ci.radius;
				float xy = ci.center.Y + ci.radius;
				if (xx > max.X)
					max.X = xx;
				if (xy > max.Y)
					max.Y = xy;
			}
			Vector2 v = (max - min) / 2f;
			float r = Vector2.Distance(v);
			Vector2 center = min + v;
			boundingCircle = new Circle(r, center);
		}

		#region IShape implementation
		public Circle BoundingCircle {
			get {
				return boundingCircle;
			}
		}
		public Rectangle BoundingRectangle {
			get {
				// MBR wiki. ez még nem megoldott. az ishape deklaráciojánál van a weblink
				return new Rectangle(0,0,0,0);
			}
		}
		public void SetTransform(Transform t) {
			// mindent eltolunk
			Transform relative = new Transform();
			relative.Translate = t.Translate - boundingCircle.center;
			for (int i = 0; i < Members.Count; i++) {
				Transform abs = new Transform();
				abs.Translate = relative.Translate + Members[i].BoundingCircle.center;
				Members[i].SetTransform(abs);
			}
			CalcBoundingCircle();
		}
		#endregion
		public object Clone()
		{
			return this.MemberwiseClone();
		}
		public override string ToString() {
			return string.Format("[ShapeGroup]");
		}
	}
}

