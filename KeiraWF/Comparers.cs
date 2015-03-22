using System;
using System.Collections.Generic;
using InternalSection;
using Geometry.Shapes;

namespace InternalSection {
	public class DistanceElementComparer : IComparer<StaticModel>
	{
		public Vector2 Origin { get; private set; }
		public DistanceElementComparer(Vector2 origin) {
			this.Origin = origin;
		}
		public DistanceElementComparer() {
			this.Origin = new Vector2();
		}
		#region IComparer[StaticModel] implementation
		public int Compare(StaticModel x, StaticModel y) {
			Circle c = x.Shape.BoundingCircle;
			Circle c1 = y.Shape.BoundingCircle;
			float d = Vector2.Distance(c1.center - Origin) - Vector2.Distance(c.center - Origin);
			if (d > 0) return -1;
			else if (d < 0) return 1;
			else return 0;
		}
		#endregion
	}
	public class AngleElementComparer : IComparer<StaticModel>
	{
		public Vector2 Origin { get; private set; }
		public AngleElementComparer(Vector2 origin) {
			this.Origin = origin;
		}
		public AngleElementComparer() {
			this.Origin = new Vector2();
		}
		#region IComparer[StaticModel] implementation
		public int Compare(StaticModel x, StaticModel y) {
			Circle c = x.Shape.BoundingCircle;
			Circle c1 = y.Shape.BoundingCircle;
			float d = Vector2.Angle(c1.center - Origin) - Vector2.Angle(c.center - Origin);
			if (d > 0) return -1;
			else if (d < 0) return 1;
			else return 0;
		}
		#endregion
	}
	public class SideBySideEqComp : IEqualityComparer<Vector2> 
	{
		Vector2 source;
		public Vector2 EqVector { get; private set; }
		public float Angle1 { get; private set; }
		public float Angle2 { get; private set; }

		public SideBySideEqComp(Vector2 src) {
			source = src;
		}
		public bool Equals(Vector2 x, Vector2 y)
		{
			Vector2 v1 = x - source;
			Vector2 v2 = y - source;

			float a1 = Vector2.Angle(Vector2.Normailze(v1));
			float a2 = Vector2.Angle(Vector2.Normailze(v2));

			Angle1 = a1;
			Angle2 = a2;
			float mod = Math.Abs((a1 - a2) % 360f);

			if (mod < 3f || mod > 357f) {
				if (Vector2.Distance(v1) < Vector2.Distance(v2))
					return true;
				else
					return false;
			}
			EqVector = Vector2.Zero;
			return false;
		}

		public int GetHashCode(Vector2 obj)
		{
			return obj.X.GetHashCode() ^ obj.Y.GetHashCode();
		}
	}
}

