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
using System.Drawing;


namespace InternalSection
{
	public enum ScanResults { Nothing, LookAt }

	// TODO: tedd bele a libbe azokat a dolgokat amik általánosak pl SensorPoints, Multisensor, de még a StepTo, ScanTo-kat is lehetne TALÁN
	public class Player // akár lehetne a libbe is csak akkor a Move-ot felül kell h birálja a gravitáció pl
	{
		// ezzel h privát és nem öröklődik elkerüljük a bonyodalmakat
		AnimModel model; // egy szintel feljebb már ne kelljen ilyenekkel foglalkozni mint a model.
		public ScanResults ScanValue { get; private set; }
		public Rectangle InvalidRectangle { get { return model.InvalidRectangle; } }
		public bool IsScanComplete { get; private set; }
		public SimpleArea Area;
		Random rnd = new Random();
		Player otherPlayer;
		bool isRecall = false;

		public Player(AnimModel model)
		{
			this.model = model;
		}
		public Player(AreaWorker area, AnimModel model)
		{
			this.model = model;
			this.Area = area;
		}
		public List<Vector2> way = new List<Vector2>();
		int round;
		Stopwatch sw = new Stopwatch();
		public bool StepTo(Player p, float speed)
		{
			bool isEnableMove = false; // ez meg hol lesz true?

			otherPlayer = p;
			lastFollowTime = DateTime.Now;
			sw.Restart();
			//Console.WriteLine(Thread.CurrentThread.Name);
			//sensorPoints = ScanTo(p, speed, out v); // ez a szinkron megoldás
			if (sensorPoints != null) {
				if (!TryMove(v, speed)) {
					int rndI = rnd.Next(0, sensorPoints.Count);
					oneWay = sensorPoints[rndI];
					//Console.WriteLine(oneWay);
					followColNum = 0;
				} else {
					followColNum++;
					isEnableMove = true;
				}
				if (followColNum > 10) {
					followColNum = 0;
					return isEnableMove; 
				}
			}
			FollowDelegate scD = ScanTo;
			isRecall = false;

			if (followColNum > 5 || followColNum == 0)// TODO: ezt a 5öst is be kéne adni a performance managernek
			{ // ha már 5 lépést megtettünk ütközés nélkül || mert trymove false lett
				IsScanComplete = false;
				scD.BeginInvoke(p, speed, out v, new AsyncCallback(delegate(IAsyncResult iar)
				                                                   {
					AsyncResult result = (AsyncResult)iar;
					FollowDelegate d = (FollowDelegate)result.AsyncDelegate;
					if (sensorPoints != null)
						sensorPoints.Clear();
					sensorPoints = d.EndInvoke(out v, iar);
					IsScanComplete = true;
					//Thread.CurrentThread.Join(1400);
					if (ScanComplete != null)
						ScanComplete(this, EventArgs.Empty);
					//Console.WriteLine(Thread.CurrentThread.Name);
					way.Clear();
					way.Add(oneWay);
					isRecall = true;
				}), null);
			}

			return isEnableMove;
		}
		DateTime lastFollowTime;
		Vector2 oneWay, v;
		public List<Vector2> sensorPoints;
		int followColNum = 99;

		delegate List<Vector2> FollowDelegate(Player player, float spd, out Vector2 v);
		public event EventHandler ScanComplete;
		public List<Vector2> ScanTo(Player p, float spd, out Vector2 v)
		{
			// itt lehet variálni h mennyire legyen alapos a kereső
			List<Vector2> sensorPoints = MapScan.MultiSensor(Area.GameObjects.Select(item => item.Shape), new Line(Position, p.Position - Position, 300f), (float)rnd.NextDouble() + 1f, (float)rnd.NextDouble() * 1f);
			if (followColNum > 3) // TODO: ezt a 3ast is be kéne adni a performance managernek és ne it legyen már ez a korlátozás hanem kívül
			{// me mi van ha vmi nem is használja a másik fv-t amibe ez a follow var van és csak ezt a ScanTo-t használja..
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
			if (Vector2.Distance(v) < 25f)
			{
				oneWay = default(Vector2);
			}
			return sensorPoints;
		}
		public bool isEndMove = true;
		public bool TryMove(Vector2 direction, float speed)
		{
			isEndMove = false;
			direction = Vector2.Normailze(direction); // 2x-es Normalize az jó?
			Vector2 trs = model.Transformation.Translate;
			float x = trs.X + direction.X * speed, y = trs.Y + direction.Y * speed;
			bool res = model.Move(x, y);// elmozdult most kell kirajzolni arra az invalidRect-re (model.InvalidRectangle <=> model.Shape.BoundingRectangle;)
			isEndMove = true;
			return res;
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

