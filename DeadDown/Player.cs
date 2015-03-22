using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Geometry.Shapes;
using Entities;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Security;
using System.Runtime.Remoting.Messaging;

// TODO: Zsákutva: megkeressük a legkisebb F-et. Megkerestük de a legkiebb mindig a célhoz legközelebbi lesz.
// ilyenkor a zsákutcából visszafele irányba akarunk kijutni. De ha innétől nagyon messze van a kiút akko szívás.


namespace InternalSection
{
    public enum ScanResults { Nothing, LookAt }

	public class Player // akár lehetne a libbe is csak akkor a Move-ot felül kell h birálja a gravitáció pl
	{
		public AnimModel model; // ezzel h privát és nem öröklődik elkerüljük a bonyodalmakat
		// egy szintel feljebb már ne kelljen ilyenekkel foglalkozni mint a model.
		public SimpleArea Area;
        public ScanResults ScanValue { get; private set; }
		Type t = typeof(Collision);
		Random rnd = new Random();
		Player otherPlayer;
		float astrOffset;
		Thread fThr = null;
		bool isRecall = false;

		public Player(AnimModel model)
		{
			this.model = model;
			astrOffset = 2f;
			AsdValue = 1;
			fThr = new Thread(new ThreadStart(delegate {
				while (!isRecall)
					Thread.Sleep(1500);
				//if (ScanComplete != null)
				//    ScanComplete(this, EventArgs.Empty);
			}));
			fThr.Name = "WaitForScan";
			fThr.Priority = ThreadPriority.Lowest;
			fThr.IsBackground = true;
		}
		public Player(AreaWorker area, AnimModel model)
		{
			this.model = model;
			this.Area = area;
			AsdValue = 1;
			astrOffset = 2f;
		}
		List<Vector2> SensorPoints(Line line) {
			List<Vector2> lookAt = new List<Vector2>(); 
			bool isCol = false;
			List<Vector2> colVects = new List<Vector2>();
			object obj = null;

			//line.End = Vector2.Rotate(line.End, .04f, line.Start);
			foreach (IShape shp in Area.GameObjects.Select(x => x.Shape)) { //Area.actShapes
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

			for (int i = 0; i < lookAt.Count; i++) {
				this.lookAt.Add(lookAt[i]);
				lookAtStr.Add(Vector2.Radian(Vector2.Normailze(lookAt[i] - line.Start)));
			}

			return lookAt;
		}
		List<Vector2> MultiSensor(Line line, float pnDegree, float step) {
			List<Vector2> pts = new List<Vector2>();
			float i = -step;
			bool switcher = true;
			Vector2 end = line.End;
			while (i <= pnDegree) {
				line.End = end;
				if (switcher) {
					line.End = Vector2.Rotate(line.End, i, line.Start);
					pts.AddRange(SensorPoints(line));
					switcher = false;
				} else {
					line.End = Vector2.Rotate(line.End, -i, line.Start);
					pts.AddRange(SensorPoints(line));
					switcher = true;
				}
				i += step;
			}

			return pts;
		}
		public List<Vector2> lookAt = new List<Vector2>();
		public List<float> lookAtStr = new List<float>();
		public List<Line> lines = new List<Line>();
		public List<Vector2> DetectedPoints(Vector2 v, float radius) { // TODO: ne csak a metszéspontokat, hanem ameddig ellátunk és nincs col is
			lines.Clear(); 
			List<Vector2> lookAt = new List<Vector2>(); // tehát a zöld szakaszok végét. habár ütközésig kéne nyujtani a szakaszokat.
			lookAtStr.Clear();
			SideBySideEqComp sbs = new SideBySideEqComp(v);
			bool lastCollVector = false;
			for (float i = 0; i < 360; i+=rnd.Next(1,41)) {
				Line line = new Line(v, new Vector2(v.X + radius * (float)Math.Cos(i * 1f * Math.PI / 180f), v.Y + radius * (float)Math.Sin(i * 1f * Math.PI / 180f)));
				object obj = null;
				foreach (IShape shp in Area.GameObjects.Select(x => x.Shape)) { //Area.actShapes
					obj = t.InvokeMember("Intersect", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, new OverloadBinder(), null, new object[2] {
						shp,
						line
					}); // 101,300-nál line to line intersect resultja NaN vektor (40,0)val i=0 v=986;294 End=1026;294
					if (obj is Vector2 && !float.IsNaN(((Vector2)obj).X) && !float.IsInfinity(((Vector2)obj).X)) {
						lookAt.Add((Vector2)obj - Vector2.Normailze(line.Direction)); //kicsit tartsunk el az ütközöt egyensetől (-line.normalize)
						lastCollVector = true;
					}

				}
				if (!lastCollVector)
					lookAt.Add(line.End- Vector2.Normailze(line.Direction));
				else
					lastCollVector = false;
			}
			for (int i = 0; i < lookAt.Count; i++) {
				for (int j = lookAt.Count - 1; j > i; j--) {
					Vector2 eqVec = isEq(v, lookAt[i], lookAt[j]); // nagyobb offset kell neki
					if (eqVec == lookAt[i])
						lookAt.Remove(lookAt[j]);
					else if (eqVec == lookAt[j])
						lookAt.Remove(lookAt[i]);
				}
			}

			for (int i = 0; i < lookAt.Count; i++) {
				this.lookAt.Add(lookAt[i]);
				lookAtStr.Add(Vector2.Radian(Vector2.Normailze(lookAt[i] - v)));
			}
			//lookAtStr = lookAtStr.Distinct().ToList();
			return lookAt;
		}
		Vector2 isEq(Vector2 source, Vector2 x, Vector2 y)
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



		public event EventHandler Inval;
		public List<Vector2> way = new List<Vector2>();
		public bool IsFoundDestination = false;
		//		int idx;
		int kx;
		float n,m;
		public void forPoints(List<Vector2> points, float idx, Vector2 destination, float offset) {
			bool isValidWay = false;
			//points.Sort(new AStar() { Source = otherPlayer.Position, Destination = destination, DistLess = AsdValue });
			//			points.ForEach(item => { 
			//				Console.Write(item + "\t");
			//				Console.WriteLine(Vector2.Distance(destination - item) + Vector2.Distance(otherPlayer.Position - item));
			//			});
			for (int k = (int)idx; k < points.Count; k++) {
				//				Console.WriteLine((++kx).ToString() + ": " + idx.ToString() + "  ");
				float dsa = Vector2.Distance(destination - points[k]);
				if (IsFoundDestination || Vector2.Distance(destination - points[k]) < offset) {
					if (!IsFoundDestination) way.Add(points[k]);
					IsFoundDestination = true;
					return;
				}
				if (way.Count > 1 && Vector2.Distance(way.Last() - points[k]) < offset) {
					Thread.Sleep(2);
					if (++round >= 5) {
						Vector2 w0 = way[0];
						way.Clear();
						way.Add(w0);
						round = 0;
						n = 0;
						AsdValue += .2f;
						forPoints(MultiSensor(new Line(way.Last(), way.Last() - Position, 900f), 1f, .3f), ++m, destination, offset);
					} else {
						AsdValue += .2f;
						forPoints(MultiSensor(new Line(way.Last(), way.Last() - Position, 900f), 1f, .1f), ++n, destination, offset);
					}
				}
				else if (Vector2.Distance(way.Last() - points[k]) < offset) {
					//way.RemoveAt(way.Count - 1);
					//					Vector2 w0 = way[0];
					//					way.Clear();
					//					way.Add(w0); 
					// az n-et kell ritkábbra venni (de mindent bejárni!) és lentebb is indítani ha pl az első 3 keresés nem talált.
					// az a lényeg h az első kitalált uton/pontokon/szakaszon már jelölje az uj utat. És hogy foltok ne legyenek
					// mert a foltok felesleges számítások. Akkor jo ha minden egyenletesen bepirosodik.

					// ha n > 3 => { felezős ugrálós algo }
					n += 1f;
					//					this.lookAt.Clear();
					forPoints(MultiSensor(new Line(way.Last(), way.Last() - Position, 1300f), 1f, .1f), n, destination, offset);
					//continue; // itt van az h nem haladtunk semmit
				}
				else { // TODO: Mikor lesz vége az aglónak? Mivan ha nem talál utat? Miért megy vissza?

					// megnéuuük a way utcso 10 eleme nincs-e közel egymáshoz
					// ha közel van akkor visszahívjuk a 
					// forPoint-ot Position + (waylast- pos)/ 2-vel mint detected pts
					// idx-et meg = 0-val;
					//					n = 0;
					way.Add(points[k]);
					//					if (Inval != null)
					//						Inval(this, EventArgs.Empty);

					//					Thread.Sleep(1000);

					k = 0; // ha idx-et változtatjuk akkor vigyázzunk mert nem tudjuk a kövi points.Count-ot (for int = k-nál le kell kezelni,
					// h mi legyen ha idx > min
					forPoints(MultiSensor(new Line(way.Last(), Position - way.Last(), 1300f), 2f, .4f), 0, destination, offset);

					//					bool isTre = false;
					//                    for (int t = way.Count - 1; way.Count - 1 - t < 10 && t >= 0; t--) {
					//                        int h = (way.Count >= 10) ? way.Count - 10 : 0;
					//                        while (h < way.Count) {
					//                            if (t == h) {
					//                                h++;
					//                                continue;
					//                            }
					//                            if (Vector2.Distance(way[t] - way[h]) < 5f) {
					//                                way.RemoveAt(t);
					//                                way.RemoveAt(h);
					////								isTre = true;
					//                                goto Breaked0;
					//                            }
					//                            h++;
					//                        }
					//                    }
					//                    goto Continue0;
					//                Breaked0:
					//  + (way.Last() - this.Position) / 2f
					//idx = 0; // ha szoget változtatom pl nem 30-100 hanem 100-180 akk kiesik a már volt utvonal (talán)
					//astrOffset = 0.1f;
					//					forPoints(DetectedPoints(otherPlayer.Position + (way.Last() - otherPlayer.Position) / 2f, rnd.Next(1, 10)),
					//					          idx, destination, offset);
					//					forPoints(DetectedPoints(otherPlayer.Position, rnd.Next(10, 100)),
					//					          idx, destination, offset);

					//return;
					//if (!IsBadWay(destination, otherPlayer)) {
					//way.Add(destination);
					//	return;
					//}
				}
				//			Continue0: // nah utt csak ugy kiugrottam az elséből :s

			}
			m = 0;
			AsdValue = 1;
			kx = 0;
			Console.WriteLine("Done" + rnd.Next(0,1000).ToString());
		}
		public float AsdValue { get; set; }
		bool ValidWay(Vector2 destination) {
			Line line = new Line(way.Last(), destination);
			var shapes = Area.GameObjects.Select(x => x.Shape);
			foreach (IShape shp in shapes) {
				if (!shp.Equals(model.Shape) && !shp.Equals(otherPlayer.model.Shape)) {
					object obj = t.InvokeMember("Intersect", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, new OverloadBinder(), null, new object[2] {
						shp,
						line
					});
					if (Collision.IsCollision || (obj != null && obj is Vector2 && !float.IsNaN(((Vector2)obj).X) && !float.IsInfinity(((Vector2)obj).X))) {
						return false;
					}
				}
			}
			return true;
		}
		bool IsBadWay(Vector2 destination, Player p) {
			Line line = new Line(way.Last(), destination);

			var shapes = Area.GameObjects.Select(x => x.Shape);
			foreach (IShape shp in shapes) {
				//if (!shp.Equals(model.Shape) && !shp.Equals(p.model.Shape)) 
				//				{
				object obj = t.InvokeMember("Intersect", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, new OverloadBinder(), null, new object[2] {
					shp,
					line
				});
				if (Collision.IsCollision && (obj != null && obj is Vector2 && !float.IsNaN(((Vector2)obj).X) && !float.IsInfinity(((Vector2)obj).X))) {
					float dis = Vector2.Distance(line.Start - (Vector2)obj);
					float strToEnd = Vector2.Distance(line.Direction);
					if (dis > 1f && dis < strToEnd - 1f)
						return true;
				}
				//				}
			}
			return false;
		}

		int round;
		Stopwatch sw = new Stopwatch();
		public bool StepTo(Player p, float speed)
		{
			bool isEnableMove = false;
			lookAt.Clear();
			lines.Clear();
			IsFoundDestination = false;

			otherPlayer = p;
			lastFollowTime = DateTime.Now;
			sw.Restart();
			//Console.WriteLine(Thread.CurrentThread.Name);
			if (sensorPoints != null) {
				if (!TryMove(v, 2f)) { // -1et tettem oda de elm nem kéne.... ? :s
					oneWay = sensorPoints[rnd.Next(0, sensorPoints.Count - 1)]; // -1? mmeg mér? mér dobott index kivételt? :s
					//Console.WriteLine(oneWay);
					followColNum = 0;
				} else {
					isEnableMove = true;
					followColNum++;
				}
				if (followColNum > 10) {
					followColNum = 0;
					return isEnableMove;
				}
			}
			FollowDelegate scD = ScanTo;
			isRecall = false;

			scD.BeginInvoke(p, out v, new AsyncCallback(delegate(IAsyncResult iar) {
				AsyncResult result = (AsyncResult)iar;
				FollowDelegate d = (FollowDelegate)result.AsyncDelegate;
				if (sensorPoints != null)
					sensorPoints.Clear();
				sensorPoints = d.EndInvoke(out v, iar);
				//Thread.CurrentThread.Join(1400);

				//Console.WriteLine(Thread.CurrentThread.Name);
				way.Clear();
				way.Add(oneWay);
				isRecall = true;
			}), null);
            //if (scan == ScanResults.LookAt && ScanResult != null)
            //    ScanResult(this, EventArgs.Empty);
			if (fThr.ThreadState == (System.Threading.ThreadState.Background | System.Threading.ThreadState.Unstarted) || 
			    fThr.ThreadState == System.Threading.ThreadState.Unstarted)
				fThr.Start();

			return isEnableMove;
		}
		DateTime lastFollowTime;
		Vector2 oneWay, v;
		List<Vector2> sensorPoints;
		int followColNum = 99;

		delegate List<Vector2> FollowDelegate(Player player, out Vector2 v);
		public event EventHandler ScanComplete;
        public event EventHandler ScanResult;
		public List<Vector2> ScanTo(Player p, out Vector2 v)
		{
			//Thread.CurrentThread.Name = "scanthr";
			// itt lehet variálni h mennyire legyen alapos a kereső
			List<Vector2> sensorPoints = MultiSensor(new Line(Position, p.Position - Position, 300f), (float)rnd.NextDouble() + 1f, (float)rnd.NextDouble() * 1f);
			if (followColNum > 0)
			{
				for (int i = 0; i < sensorPoints.Count; i++)
				{
					if (Vector2.Distance(sensorPoints[i] - p.Position) < 20f)
					{
						oneWay = sensorPoints[i];
                        ScanValue = ScanResults.LookAt;
						break;
					}
				}
			}
			if (oneWay == default(Vector2))
			{
                ScanValue = ScanResults.Nothing;
				oneWay = sensorPoints[rnd.Next(0, sensorPoints.Count)];
			}
			v = oneWay - Position;
			if (Vector2.Distance(v) < 5f)
			{
				oneWay = default(Vector2);
			}
			return sensorPoints;
		}

		int i;
		Vector2 offset;
		List<Vector2> wayTree = new List<Vector2>();
		List<int> wayTreeNums = new List<int>();
		void Follow(Vector2 source, Vector2 destination, float epsilon, AreaWorker dimension)
		{
			int count = 0;
			Vector2 v = destination - source;
			//			IShape myCircle = new Circle(model.Shape.BoundingCircle.radius, source);
			//			Frame f = new Frame(0f);
			//			Transform t = new Transform();
			//			PhysicalFeatures pf = new PhysicalFeatures(Vector2.Zero);
			//			StaticModel sm = new StaticModel(ref f, ref myCircle, ref t, pf);

			if (Vector2.Distance(v) > epsilon)
			{
				//				while (Vector2.Distance(v) > epsilon) {
				//					Vector2 pos = source + (v / 2f) + offset;
				//					offset += Vector2.Normailze(Vector2.Rotate(v, 1.57f)) * 10;
				//					Collision.Intersect(myCircle, new Line(source + myCircle.radius + 1f, pos + myCircle.radius));
				//				}

				while (Vector2.Distance(offset) < 20)
				{ // most itt végigmegyünk a felező magasságán, de
					Vector2 pos = source + (v / 2f) + offset; // előtte meg kell nézni, hogy szimplán oda lehet-e jutni a célhoz
					model.Transformation.Translate = pos;
					offset += Vector2.Normailze(Vector2.Rotate(v, 1.57f)) * 10; // és akkor nem kéne számolni semmit
					if (!dimension.TestCollision(model))
					{
						wayTree.Add(pos); // itt meg lehetne ugy is csinálni, hogy emlékezetből utkeresés
						count++; // tehát amit nem látunk arra emlékszünk arra a pontra ami anno még szabad volt
					} // de a látásunkkal megvizsgáljuk mikor már belátható
				}
				while (i < wayTree.Count)
				{
					offset = Vector2.Zero;
					Follow(wayTree[i++], destination, epsilon, dimension);
				}
				wayTreeNums.Add(count);

			}
		}
		public bool TryMove(Vector2 direction, float speed)
		{
			//                Console.WriteLine(Thread.CurrentThread.Name);
			direction = Vector2.Normailze(direction); // 2x-es Normalize az jó?
			Vector2 trs = model.Transformation.Translate;
			float x = trs.X + direction.X * speed, y = trs.Y + direction.Y * speed;
			return (model.Move(x, y)); // elmozdult most kell kirajzolni arra az invalidRect-re (model.InvalidRectangle <=> model.Shape.BoundingRectangle;)
		}

		public Vector2 Position {
			get { return model.Transformation.Translate; }
		}
		public Vector2 Rotation {
			get { return model.Transformation.Rotate; }
		}
		public Vector2 Size {
			get { return model.Transformation.Scale; }
		}
	}
}

