using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Geometry;
using Geometry.Shapes;
using Worker;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;
using System.Threading;
using Entities;

namespace InternalSection {
	public class AreaWorker : SimpleArea
	{
		public List<int> DistanceIndices { get; private set; }
		public List<int> AngleIndices { get; private set; }
		public List<IShape> actShapes = new List<IShape>();
		
		XmlSerializer serializer;
		Circle circle;
		StaticModel obj;

		Random rnd = new Random();

		public AreaWorker() : base() {
//			serializer = new XmlSerializer(typeof(List<StaticObject>));
			DistanceIndices = new List<int>();
			AngleIndices = new List<int>();
//			AnimModel.TransformEvent += HandleTransform;
		}
		public static Line line;
		protected bool CollisionTest(int otherDi, int otherAi) {
			IShape other;// = GameObjects[DistanceIndices[otherDi]].VirtualShape;
			IShape me = obj.VirtualShape;
			Type t = typeof(Collision);

			if (otherDi >= 0)
				other = GameObjects[DistanceIndices[otherDi]].VirtualShape;
			else if (otherAi >= 0)
				other = GameObjects[AngleIndices[otherAi]].VirtualShape;
			else
				throw new Exception("The first two parameters shouldn't be -1 at the same time."); // TODO: ez így értelmes mondat? :)

			Circle crc = new Circle(circle.radius * 1.1f, circle.center);
			t.InvokeMember("Intersect", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, new OverloadBinder(), null, new object[2] {
				crc, other 
			});
			if (Collision.IsCollision) {
//				Console.WriteLine("-----------MAJDNEM Ütközött----------" + rnd.Next());
				obj.Physical.CollisionMode = CollisionState.NearlyCollision;
				if (!obj.Physical.Collosed.Contains(other))
					obj.Physical.Collosed.Add(other);
				t.InvokeMember("Intersect", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, new OverloadBinder(), null, new object[2] {
					me, other
				});
				if (Collision.IsCollision) {
//					Console.WriteLine("-----------Ütközött----------" + rnd.Next());
					obj.Physical.CollisionMode = CollisionState.Collosed;
					obj.Physical.Collosed.Clear();
					obj.Physical.Collosed.Add(other);
					obj.Physical.Direction = Vector2.Zero;
					return true;
				}
			}
			if (obj.Physical.Collosed.Contains(other)) {
				obj.Physical.Collosed.Remove(other);
				obj.LineLock = false;
				if (obj.Physical.Collosed.Count == 0) {
					obj.Physical.CollisionMode = CollisionState.NotCollision;
//					Console.WriteLine("-----------NEM Ütközött----------" + rnd.Next());
			}
	//				obj.MoveDirection = Moving.None;return false;
			}
			return false;
		}
		public static Vector2 VelocityNow;

		bool BoundCircleTest(int otherDi, int otherAi, out float circleDist) {
			Circle other; // = GameObjects[DistanceIndices[otherDi]].VirtualShape.BoundingCircle;
			Circle me = obj.VirtualShape.BoundingCircle;
			Circle crc = new Circle(circle.radius * 1.1f, circle.center);
			if (otherDi >= 0)
				other = GameObjects[DistanceIndices[otherDi]].VirtualShape.BoundingCircle;
			else if (otherAi >= 0)
				other = GameObjects[AngleIndices[otherAi]].VirtualShape.BoundingCircle;
			else
				throw new Exception("The first two parameters shouldn't be -1 at the same time."); // TODO: ez így értelmes mondat? :)

			circleDist = Collision.Intersect(me, other);
			if (Collision.IsCollision) {
				Collision.Intersect(me, other);
				if (Collision.IsCollision)
					return true;
			}
			return false;
		}

		public int BoundNumber(int objIndex) {
			return DistanceIndices.Single(item => item == objIndex);
		}

		/// <summary>
		/// Csak addig vizsgálja amíg kell és minden méretu kornél mukszik.
		/// </summary>
		bool DistanceToZero(Circle circle, int index, float maxDist) { // TODO: Sztem ebbe van a hiba. Nem jót dob a ColTest-nek
			Circle c1 = null;
			int x = 1;
			int di = DistanceIndices.IndexOf(index);
			int ai = AngleIndices.IndexOf(index);
			float dist = Vector2.Distance(circle.center);
			double angle = Math.Atan(circle.center.Y / circle.center.X);
			obj.Physical.CollisionMode = CollisionState.NotCollision;
			bool collosed = false;
			float crcDist = 0.0f;
			actShapes.Clear();

			int debugCW = 0;
			
			//Console.WriteLine("SearchDist:");
			while (di + x < DistanceIndices.Count) {
				c1 = GameObjects[DistanceIndices[di + x]].VirtualShape.BoundingCircle;
				float d = Vector2.Distance(c1.center);
				if (d - c1.radius < Math.Abs(dist + circle.radius * 2f)) {
//					SearchCollision(di + x, ai, angle);
					actShapes.Add(GameObjects[DistanceIndices[di + x]].VirtualShape);
					if (DistanceIndices[di + x] == AngleIndices[ai + x] && CollisionTest(di + x, -1)) {
						collosed = true;
						Console.WriteLine("DI: " + DistanceIndices[di + x].ToString());
						Console.WriteLine("AI: " + AngleIndices[ai + x].ToString());
						return collosed;
					}

//					if (BoundCircleTest(di + x, -1, out crcDist)) {
//						if (BoundCircleTest(-1, AngleIndices.Single(item => item == DistanceIndices[di + x]), out crcDist)
//						    && CollisionTest(di + x, -1)) {
//								collosed = true;
//						}
//					}
//					else
//						debugCW++;
//
//					if (crcDist > maxDist)
//						break;
				}
				else if (collosed)
					break;
				x++;
			}
			//nem állítjuk le mert kihagyott egyet
			//de ha megírnád a váltvaforgó algoritmust akkor jó lenne!
			// TODO: 8642x1357
			x = 1;
			while (di - x >= 0) {
				c1 = GameObjects[DistanceIndices[di - x]].VirtualShape.BoundingCircle;
				float d = Vector2.Distance(c1.center);
				if (d + c1.radius > Math.Abs(dist - circle.radius * 2f) || d == 0f) { //Console.WriteLine(di+x);
//					SearchCollision(di - x, ai, angle);
					actShapes.Add(GameObjects[DistanceIndices[di - x]].VirtualShape);

					if (DistanceIndices[di - x] == AngleIndices[ai - x] && CollisionTest(di - x, -1)) {
						collosed = true;
						Console.WriteLine("DI: " + DistanceIndices[di - x].ToString());
						Console.WriteLine("AI: " + AngleIndices[ai - x].ToString());
						return collosed;
					}
//					if (BoundCircleTest(di - x, -1, out crcDist) && CollisionTest(di - x, -1))
//						collosed = true;
//					else
//						debugCW++;
//
//					if (crcDist > maxDist)
//						break;
				}
				else if (collosed)
					break;
				x++;
			}
//
//			x = 1;
//			while (ai + x < AngleIndices.Count) {
//				c1 = GameObjects[AngleIndices[ai + x]].VirtualShape.BoundingCircle;
//				float d = Vector2.Distance(c1.center);
//				if (d - c1.radius < Math.Abs(dist + circle.radius * 2f)) {
//					actShapes.Add(GameObjects[AngleIndices[ai + x]].VirtualShape);
//					if (BoundCircleTest(-1, ai + x, out crcDist) && CollisionTest(-1, ai + x)) { // meg kell nézni a (di + x)-edik távolságát. a circle, meg a c1 távot és ha nagyobb mint 'offset' akkor break, és a köv ciklusba is ugyanez
//						collosed = true;
//					}
//					else
//						debugCW++;
//
//					if (crcDist > maxDist)
//						break;
//				}
//				else if (collosed)
//					break;
//				x++;
//			}
//			x = 1;
//			while (ai - x >= 0) {
//				c1 = GameObjects[AngleIndices[ai - x]].VirtualShape.BoundingCircle;
//				float d = Vector2.Distance(c1.center);
//				if (d + c1.radius > Math.Abs(dist - circle.radius * 2f) || d == 0f) { //Console.WriteLine(di+x);
//					//					SearchCollision(di - x, ai, angle);
//					actShapes.Add(GameObjects[AngleIndices[ai - x]].VirtualShape);
//					if (BoundCircleTest(-1, ai - x, out crcDist) && CollisionTest(-1, ai - x))
//						collosed = true;
//					else
//						debugCW++;
//
//					if (crcDist > maxDist)
//						break;
//				}
//				else if (collosed)
//					break;
//				x++;
//			}
			//Console.WriteLine("\t" + (di+x-1));
			//Console.WriteLine("---------------------------");
//			Console.WriteLine(debugCW);
			return collosed;
		}
		protected override bool HandleTransform(object sender, EventArgs e)
		{
			// Next algo step-to-step: Di {0,4,2,6,8,9,3,7,1,5}, Ai {7,2,5,9,0,1,8,2,6,4}
			// Di[Search(sender)] & Ai[Search(sender)] => 9
			// while (GameObjects[Di[9+x]].Distance(GameObjects[Di[9]]) < ASD) x++; ha ennek vége x-- -szal is while...
			// Angle-vel is ugyanez mint belső ciklus
			
//			Console.WriteLine("rendez");
//			DistanceIndices.QuickIndicesSort<StaticModel>(GameObjects, new DistanceElementComparer());
//			AngleIndices.QuickIndicesSort<StaticModel>(GameObjects, new AngleElementComparer());

//			obj = (StaticModel)sender;
//			circle = obj.VirtualShape.BoundingCircle;
//			int index = GameObjects.IndexOf(obj); // ha nem szerepel a listában akkor -1 lesz. az egér menet közben
			// amíg ideér elmozdulhat és akkor már -1 is lesz az index
			
//			return DistanceToZero(circle, index, 200f); // TODO: konstans?...
			return false;
//			Console.WriteLine(metaOfRegion.NearGeometry.Count);
//			return true;
		}

		// nem kész angleindices nincs benn mint látom
		public List<IShape> GetAreaShapes(StaticModel model)
		{
			Circle c1 = null, circle = model.VirtualShape.BoundingCircle;
			int x = 1;
			int di = DistanceIndices.IndexOf(GameObjects.IndexOf(model));
			//int ai = AngleIndices.IndexOf(index);
			float dist = Vector2.Distance(circle.center);
			//double angle = Math.Atan(circle.center.Y / circle.center.X);
			actShapes.Clear();

			while (di + x < DistanceIndices.Count) {
				c1 = GameObjects[DistanceIndices[di + x]].VirtualShape.BoundingCircle;
				float d = Vector2.Distance(c1.center);
				if (d - c1.radius < Math.Abs(dist + circle.radius * 2f)) {
					actShapes.Add(GameObjects[DistanceIndices[di + x]].VirtualShape);
				}
				else
					break;
				x++;
			}
			//nem állítjuk le mert kihagyott egyet
			//de ha megírnád a váltvaforgó algoritmust akkor jó lenne!
			// TODO: 8642x1357
			x = 1;
			while (di - x >= 0) {
				c1 = GameObjects[DistanceIndices[di - x]].VirtualShape.BoundingCircle;
				float d = Vector2.Distance(c1.center);
				if (d + c1.radius > Math.Abs(dist - circle.radius * 2f) || d == 0f) { //Console.WriteLine(di+x);
					actShapes.Add(GameObjects[DistanceIndices[di - x]].VirtualShape);
				}
				else
					break;
				x++;
			}
			return actShapes;
		}

		// obsolate
		public bool TestCollision(StaticModel model) {
			bool c = GameObjects.Contains(model);
			if (!c)
				Add(model);
			DistanceIndices.QuickIndicesSort<StaticModel>(GameObjects, new DistanceElementComparer());
			obj = model as StaticModel;
			circle = obj.VirtualShape.BoundingCircle;
			int index = GameObjects.IndexOf(obj);
			bool isCol = DistanceToZero(circle, index, 50f); // TODO: konstans?...
			if (!c)
				Remove(model);
			return isCol;
		}

		public override void Add(StaticModel ie) {
			GameObjects.Add(ie);
			DistanceIndices.Add(GameObjects.Count - 1);
			AngleIndices.Add(GameObjects.Count - 1);
		}
		public override bool Remove(StaticModel model) {
			int index = GameObjects.IndexOf(model); // lehet h indicesbe van olyan de modelbe nincs..
			if (GameObjects.Remove(model) && DistanceIndices.Remove(index) && AngleIndices.Remove(index)) {
				for (int i = index; i < DistanceIndices.Count; i++) {
					DistanceIndices[i]--;
					AngleIndices[i]--;
				}
				return true;// ha modelek közt nincs akk nem nyulunk az indexekhez
			}
			return false;
		}
		public override void AddRange(List<StaticModel> objs) {
			GameObjects.AddRange(objs);
			for (int a = 0; a < objs.Count; a++) {
				DistanceIndices.Add(a);
				AngleIndices.Add(a);
			}

//			AngleIndices.Add(GameObjects.Count - 1);
		}
		public void SortIndices() {
			// majd csak insert kell a megfelelő helyre: (BinarySearch()->Insert()...)
			DistanceIndices.QuickIndicesSort<StaticModel>(GameObjects, new DistanceElementComparer());
			AngleIndices.QuickIndicesSort<StaticModel>(GameObjects, new AngleElementComparer());
		}

	}
}

