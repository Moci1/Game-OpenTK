using System;
using InternalSection;
using Entities;
using System.Drawing;

namespace Geometry.Shapes {
	public class Line : IShape
	{
		public Vector2 Start;
		public Vector2 End;
		public Line(Vector2 start, Vector2 end) {
			Start = start;
			End = end;
		}
		/// <summary>
		/// Szakasz inicializálása kezdőponttal és iránnyal. Az irány hosszúsága 1 legyen.
		/// </summary>
		public Line(Vector2 start, Vector2 direction, float length) {
//			float leght = Vector2.Distance(direction);
			Start = start;
			End = Start + Vector2.Normailze(direction) * length;
//			if (leght < 1.001f && leght > 9.999) {
//				// ide most lehet nem ugrik be (1;1) esetén 
//				Start = start;
//				End = Start + direction * length;
//			}
//			else 
//				throw new Exception("Az irányak egységvektornak kell lennie.");
		}
		/// <summary>
		/// Ilyenkor Start egyenlő lesz point-tal End pedig az y tenely és az egyenes metszépontjával.
		/// </summary>
		public Line(Vector2 point, float radian) {
			m = (float)Math.Tan(radian);
			b = point.Y - m * point.X;
			Start = point;
			End = new Vector2((float)Math.Cos(radian), (float)Math.Sin(radian));
		}
		public Line(Vector2 point, float radian, float length) {
			m = (float)Math.Tan(radian);
			b = point.Y - m * point.X;
			Start = point;
			End = new Vector2(length * (float)Math.Cos(radian), length * (float)Math.Sin(radian));
		}
		public Line(float m, float b) {
			End = new Vector2(0f, b);
			Start = new Vector2(100f, m*100f + b); // x = 1 helyen;
		}
		float m;
		public float M {
			get {
				Vector2 v = End - Start;
				if (v.X != 0)
					return m = v.Y / v.X;
				else
					return m = 999999999f; // nah ez meg mi a rák :D
			} // a függőleges nincs értelemezve y=mx+b alakba
			private set { m = value; }
		}
		float b;
		public float B {
			get {
				return b = Start.Y - (M * Start.X);
			}
			private set { b = value; }
		}
		Vector2 direction;
		public Vector2 Direction {
			get {
				direction = End - Start;
				return direction;
			}
			set {
				direction = value;
				Vector2 v = Vector2.Normailze(value);
				End = Start + v * Vector2.Distance(End - Start);
			}
		}
		
		#region IShape implementation
		public Circle BoundingCircle {
			get { // minden síknegyeden működik!
				Vector2 v = Start - End; // x és y-ból is pozitívat csinálunk:
				v.X = Math.Abs(v.X);
				v.Y = Math.Abs(v.Y); // majd a start, end-ből a legkisebb x,y koordinátákat kiszedjük:
				Vector2 vv = new Vector2((float)Math.Min(Start.X, End.X), (float)Math.Min(Start.Y, End.Y));
				return new Circle(Vector2.Distance(v) / 2f, vv + v / 2f);// végül a minX, minY-hoz honnáadjuk
			} // pozitív x,y vektort
		}
		public Rectangle BoundingRectangle {
			get { 
				float x = 0, y = 0;
				if (Start.X <= End.X)
					x = Start.X;
				else
					x = End.X;
				if (Start.Y <= End.Y)
					y = Start.Y;
				else
					y = End.Y;

				return new Rectangle((int)x, (int)y, (int)Math.Abs(Start.X - End.X), (int)Math.Abs(Start.Y - End.Y));
			}
		}
		public void SetTransform(Transform t) {
			// itt is vhogy el kell mozdítani a szakaszt.
			// de start- és end-et is? (a közepét)
			// vagy csak az egyiket?
			Vector2 relative = t.Translate - BoundingCircle.center;
			Start += relative;
			End += relative;
		}
		#endregion

		#region IComparable implementation
		public int CompareTo(object obj) {
			return 0;
		}
		#endregion
		public override string ToString() {
			return Direction.ToString();
		}
		public override bool Equals(object obj) {
			if (obj is Line) {
				Line ln = obj as Line; // két Line-nek = lehet az M és a B ha egy vonalon vannak
				if (ln.Start == this.Start && ln.End == this.End) // de mint szakasz különbözhetnek!
					return true;
			}
			return base.Equals(obj);
		}
		// TODO: most ezek az összehasonlító operátorok jók Circle-hez is ezt illeszd
		public static bool operator ==(Line ln1, Line ln2) {
			if (object.Equals(ln1, null) && object.Equals(ln2, null))
				return false;
			else if (!object.Equals(ln1, null) && !object.Equals(ln2, null))
			{
				if (System.Object.ReferenceEquals(ln1, ln2))
	        		return true;
				if (ln1.M == ln2.M && ln1.B == ln2.B)
					return true;
				else
					return false;
			}
			return false;
		}
		public static bool operator !=(Line ln1, Line ln2) {
			if (object.Equals(ln1, null) && object.Equals(ln2, null))
				return false; // ez itt problémás
			else if (!object.Equals(ln1, null) && object.Equals(ln2, null))
				return true;
			else if (object.Equals(ln1, null) && !object.Equals(ln2, null))
				return true;
			if (System.Object.ReferenceEquals(ln1, ln2))
        		return false;
			if (ln1.M == ln2.M && ln1.B == ln2.B)// ez se feltétlenül igaz
				return false; // pl ha egyvonalba vannak
			else
					return true;
		}
		public static bool operator !=(Line ln1, object obj) {
			if (obj is Line) {
				Line ln2 = (Line)obj;
				if (object.Equals(ln1, null) && object.Equals(ln2, null))
					return false;
				else if (!object.Equals(ln1, null) && object.Equals(ln2, null))
					return true;
				else if (object.Equals(ln1, null) && !object.Equals(ln2, null))
					return true;
				if (System.Object.ReferenceEquals(ln1, ln2))
	        		return false;
				if (ln1.M == ln2.M && ln1.B == ln2.B)
					return false;
				else
					return true;
			}
			return false;
		}
		public static bool operator ==(Line ln1, object obj) {
			if (obj is Line) {
				Line ln2 = (Line)obj;
				if (object.Equals(ln1, null) && object.Equals(ln2, null))
					return false;
				else if (!object.Equals(ln1, null) && !object.Equals(ln2, null))
				{
					if (System.Object.ReferenceEquals(ln1, ln2))
		        		return true;
					if (ln1.M == ln2.M && ln1.B == ln2.B)
						return true;
					else
						return false;
				}
			}
			return false;
		}
		public object Clone()
		{
			return this.MemberwiseClone();
		}
//		public static bool operator !=(Line ln1, dynamic d) {
//			if (obj is Line) {
//				Line ln2 = obj;
//				if (object.Equals(ln1, null) && object.Equals(ln2, null))
//					return false;
//				else if (!object.Equals(ln1, null) && object.Equals(ln2, null))
//					return true;
//				else if (object.Equals(ln1, null) && !object.Equals(ln2, null))
//					return true;
//				if (System.Object.ReferenceEquals(ln1, ln2))
//	        		return false;
//				if (ln1.M == ln2.M && ln1.B == ln2.B)
//					return false;
//				else
//					return true;
//			}
//			return false;
//		}
//		public static bool operator ==(Line ln1, dynamic obj) {
//			if (obj is Line) {
//				Line ln2 = obj;
//				if (object.Equals(ln1, null) && object.Equals(ln2, null))
//					return false;
//				else if (!object.Equals(ln1, null) && !object.Equals(ln2, null))
//				{
//					if (System.Object.ReferenceEquals(ln1, ln2))
//		        		return true;
//					if (ln1.M == ln2.M && ln1.B == ln2.B)
//						return true;
//					else
//						return false;
//				}
//			}
//			return false;
//		}
	}
}

