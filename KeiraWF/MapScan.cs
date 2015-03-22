using System;
using System.Collections.Generic;
using Geometry.Shapes;
using System.Reflection;

namespace InternalSection
{
	public class MapScan
	{
		static Type t = typeof(Collision);
		public static List<Vector2> SensorPoints(Line line, IEnumerable<IShape> shapes) {
			List<Vector2> lookAt = new List<Vector2>(); 
			bool isCol = false;
			List<Vector2> colVects = new List<Vector2>();
			object obj = null;

			//line.End = Vector2.Rotate(line.End, .04f, line.Start);
			foreach (IShape shp in shapes) { //Area.actShapes
				obj = t.InvokeMember("Intersect", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, new OverloadBinder(), null, new object[2] {
					shp,
					line
				}); // 101,300-nál line to line intersect resultja NaN vektor (40,0)val i=0 v=986;294 End=1026;294
				isCol = obj is Vector2 && !float.IsNaN(((Vector2)obj).X) && !float.IsInfinity(((Vector2)obj).X);
				if (isCol) {
					colVects.Add((Vector2)obj - Vector2.Normailze(line.Direction));
					//lookAt.Add((Vector2)obj - Vector2.Normailze(line.Direction) * 2f); //kicsit tartsunk el az ütközöt egyensetől (-line.normalize)
				}
			}
			if (colVects.Count > 0) {
				float minDist = 999999999999999f;
				int i = 0, di = -1;
				while (i < colVects.Count) {
					float d = Vector2.Distance(colVects[i] - line.Start);
					if (d < minDist) {
						minDist = d;
						di = i;
					}
					i++;
				}
				Vector2 rv = new Line(line.Start, colVects[di]).Direction / 20f;
				for (int j = 1; j <= 20; j++) {
					lookAt.Add(line.Start + rv * j - Vector2.Normailze(line.Direction) * 10f);
				}
			} else {
				Vector2 rv = line.Direction / 10f;
				for (int j = 1; j <= 10; j++) {
					lookAt.Add(line.Start + rv * j);
				}
			}
			return lookAt;
			//for (int i = 0; i < lookAt.Count; i++)
			//{
			//    for (int j = lookAt.Count - 1; j > i; j--)
			//    {
			//        Vector2 eqVec = isEq(line.Start, lookAt[i], lookAt[j]); // nagyobb offset kell neki
			//        if (eqVec == lookAt[i])
			//            lookAt.Remove(lookAt[j]);
			//        else if (eqVec == lookAt[j])
			//            lookAt.Remove(lookAt[i]);
			//    }
			//}
		}
		public static List<Vector2> MultiSensor(IEnumerable<IShape> shapes, Line line, float pnDegree, float step) {
			List<Vector2> pts = new List<Vector2>();
			float i = -step;
			bool switcher = true;
			Vector2 end = line.End;
			while (i <= pnDegree) {
				line.End = end;
				if (switcher) {
					line.End = Vector2.Rotate(line.End, i, line.Start);
					pts.AddRange(SensorPoints(line, shapes));
					switcher = false;
				} else {
					line.End = Vector2.Rotate(line.End, -i, line.Start);
					pts.AddRange(SensorPoints(line, shapes));
					switcher = true;
				}
				i += step;
			}

			return pts;
		}
	}
}

