using System;
using System.Diagnostics;
using Geometry.Shapes;
using InternalSection;
using Entities;
using Keira;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

// TODO: A többi kör hogyan mozogjon (mint playerek) AI kell nekik (TryMove)
// TODO: Mivel az enemy is Player lehessen váltani köztük és a támadás (katt), viselkedés playertől függ de az absztrakció közös
// TODO: 1. xml - még alakul
// TODO: 2. textures, anim - még arrább lesz
// TODO: 3. menu - lehet külső rendszer lesz
// TODO: 4. mvc - ennek még ki kell forrnia
// TODO: 5. IShape.CompareTo, hogy ezzel most mi van
// TODO: 6. Physic.Update ShapeGroup kezelése (goodVel)
// TODO: 7. Collosed.Contains(other) nem működik akkor is belepakolja ha már tartalmazza
// TODO: 8. player megfelelő irányba valo kirajzolása.
using System.Drawing.Drawing2D;
using System.Collections.Generic;


namespace DeadDown {
	public class BitmapRenderer
	{
		//Graphics g;
		Pen pen;
		public Bitmap bmp;
		Graphics g;
		RectArea aw;
		int fpsCounter=0, fps=0; // fps
		TimeSpan elapsedTime; // fps
		Random rnd = new Random();
		Rectangle invalidRect;
		Vector2 playerDirToMouse;
		System.Timers.Timer timer = new System.Timers.Timer();
        int enemyUpdateIterator = 0;
		public event EventHandler NeedInvalidation;
		public Vector2 MousePos { get; private set; }
		public Player Gamer { get; private set; }
		public List<Player> Enemies { get; private set; }
        System.Windows.Forms.Timer enemiesLowTimer, enemiesMediumTimer, enemiesHighTimer;
        List<int> mediumIdx = new List<int>();
		public bool IsDebugMode { get; set; }
		public bool IsRendering { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int Fps { get; set; }
		public bool NeedInvalidate { get; set; }
		public Rectangle InvalidRect { get; private set; }
		public Control OwnCtrl { get; private set; }
        Thread thr;

		public BitmapRenderer(Control ownCtrl, int w, int h)
		{
			OwnCtrl = ownCtrl;
			Width = w;
			Height = h;
			bmp = new Bitmap(1500, 1500);
			g = Graphics.FromImage(bmp);
			g.InterpolationMode = InterpolationMode.Low;
			g.CompositingMode = CompositingMode.SourceOver;
			g.CompositingQuality = CompositingQuality.HighSpeed;
			g.SmoothingMode = SmoothingMode.HighSpeed;
			IsRendering = true;
			Init();
		}
		void HandleButtonClick (object sender, EventArgs e)
		{
			IsRendering = true;
			Init();
			enemiesLowTimer.Stop();
		}

		public void Init() {
            aw = new RectArea(new RectangleF(0, 0, 1800, 900));
			pen = new Pen(Brushes.Black, 2f);
			Enemies = new List<Player>();
			aw.LoadMapFromFile("map5.txt");

			IShape c = new Line(new Vector2(60, 60), new Vector2(200, 200));
			Transform t = new Transform();
			Frame f = new Frame(0f);
			PhysicalFeatures pf = new PhysicalFeatures(.001f, new Vector2(0,0), 1);
//			StaticModel sm = new StaticModel(ref f, ref c, ref t, pf);
//			aw.Add(sm);
//			c = new Line(new Vector2(-60, 60), new Vector2(-200, 200));
//			sm = new StaticModel(ref f, ref c, ref t, pf);
//			aw.Add(sm);
//			c = new Line(new Vector2(60, -60), new Vector2(200, -200));
//			sm = new StaticModel(ref f, ref c, ref t, pf);
//			aw.Add(sm);
//			c = new Line(new Vector2(-60, -60), new Vector2(-200, -200));
//			sm = new StaticModel(ref f, ref c, ref t, pf);
//			aw.Add(sm);

			int x = 0;
			for (int a = 0; a < 0; a++)
			{
				//				c = new Circle(rnd.Next(10,11), new Vector2(370+x, 1+a*30 - (x)*30));
				c = new Circle(rnd.Next(10,11), new Vector2(rnd.Next(200,1301), rnd.Next(103,205)));
				//				c = new Circle(30, new Vector2(c.BoundingCircle().center.X, c.BoundingCircle().center.Y - 40));
				f = new Frame(0f);
				t = new Transform();
				pf = new PhysicalFeatures((float)rnd.NextDouble(), new Vector2(0,0), 1);
				StaticModel lr = new StaticModel(ref f, ref c, ref t, pf);
				aw.Add(lr);
				if (a % 30 == 0 && a > 0) {
					x = a;
				}
			}
			c = new Circle(16, new Vector2(rnd.Next(200,1301), rnd.Next(103,355)));
			//f = new Frame(new Bitmap("preview.png"), new System.Drawing.Rectangle(0,0, 60,90), Entities.DrawMode.Bitmap);
			f = new Frame(0f);
			t = new Transform();
			pf = new PhysicalFeatures((float)rnd.NextDouble(), new Vector2(0,0), 1);
			pf.Mode = PhysicMode.On;
			aw.Add(new AnimModel(ref f, ref c, ref t, pf));
			Gamer = new Player((AnimModel)aw.GameObjects[aw.GameObjects.Count - 1]);
			Gamer.Area = aw;

			for (int a = 0; a < 10; a++) {
				f = new Frame(0f);
				t = new Transform();
				c = new Circle(10, new Vector2(rnd.Next(30, 1201), rnd.Next(30, 600)));
				pf = new PhysicalFeatures((float)rnd.NextDouble(), new Vector2(0, 0), 1);
				pf.Mode = PhysicMode.On;
				aw.Add(new AnimModel(ref f, ref c, ref t, pf));
				Enemies.Add(new Player((AnimModel)aw.GameObjects[aw.GameObjects.Count - 1]));
				Enemies[a].Area = aw;
                //Enemies[a].ScanResult += delegate(object sender, EventArgs eea)
                //{
                //    Player item = (Player)sender;
                //    if (item.StepTo(Gamer, 5f))
                //    {
                //        NeedInvalidate = true;
                //        Rectangle r = item.model.InvalidRectangle;
                //        InvalidRect = new Rectangle((int)(r.X + -Gamer.Position.X + Width / 2f), (int)(r.Y + -Gamer.Position.Y + Height / 2f), r.Width, r.Height);
                //        if (NeedInvalidation != null)
                //            NeedInvalidation(this, EventArgs.Empty);
                //    }
                //};
			}
            //TimerManager ghjv = new TimerManager(new TimerCallback(
            thr = new Thread(new ThreadStart(delegate
            {
                while (true)
                {
                    Vector2 v = Vector2.Zero;
                    for (int i = 0; i < Enemies.Count; i++)
                    {
                        Enemies[i].ScanTo(Gamer, out v);
                        bool b = mediumIdx.Contains(i);
                        if (Enemies[i].ScanValue == ScanResults.Nothing && b)
                            mediumIdx.Remove(i);
                        if (Enemies[i].ScanValue == ScanResults.LookAt && !b)
                        {
                            mediumIdx.Add(i);
                            
                        }
                    }
                }
            }));
            thr.IsBackground = true;
            thr.Priority = ThreadPriority.Lowest;
			if (enemiesLowTimer == null) {
				enemiesLowTimer = new System.Windows.Forms.Timer();
				enemiesLowTimer.Interval = 220;
                enemiesLowTimer.Tick += delegate(object sender, EventArgs eea)
                {
                    for (int i = 0; i < Enemies.Count; i++)
                    {
                        Player item = Enemies[ i];
                        if (item.StepTo(Gamer, 20f))
                        {
                            NeedInvalidate = true;
                            Rectangle r = item.model.InvalidRectangle;
                            InvalidRect = new Rectangle((int)(r.X + -Gamer.Position.X + Width / 2f), (int)(r.Y + -Gamer.Position.Y + Height / 2f), r.Width, r.Height);
                            if (NeedInvalidation != null)
                                NeedInvalidation(this, EventArgs.Empty);
                        }
                        if (item.ScanValue == ScanResults.LookAt && !mediumIdx.Contains(i))
                        {
                            mediumIdx.Add(i);
                            enemiesMediumTimer.Start();
                        }

                    }
                };
			}
            if (enemiesMediumTimer == null)
            {
                enemiesMediumTimer = new System.Windows.Forms.Timer();
                enemiesMediumTimer.Interval = 1;
                enemiesMediumTimer.Tick += delegate(object sender, EventArgs eea)
                {
                    for (int i = 0; i < mediumIdx.Count; i++)
                    {
                        Player item = Enemies[mediumIdx[i]];
                        if (item.StepTo(Gamer, 2f))
                        {
                            NeedInvalidate = true;
                            Rectangle r = item.model.InvalidRectangle;
                            InvalidRect = new Rectangle((int)(r.X + -Gamer.Position.X + Width / 2f), (int)(r.Y + -Gamer.Position.Y + Height / 2f), r.Width, r.Height);
                            if (NeedInvalidation != null)
                                NeedInvalidation(this, EventArgs.Empty);
                        }
                    }
                };
            }
            if (enemiesHighTimer == null)
            {
                enemiesHighTimer = new System.Windows.Forms.Timer();
                enemiesHighTimer.Interval = 30;
                enemiesHighTimer.Tick += delegate(object sender, EventArgs eea)
                {
                    if (enemyUpdateIterator < Enemies.Count - 11)
                        enemyUpdateIterator++;
                    else
                        enemyUpdateIterator = 0;
                    for (int i = 0; i < 10; i++)
                    {
                        Player item = Enemies[enemyUpdateIterator + i];
                        if (item.StepTo(Gamer, 20f))
                        {
                            NeedInvalidate = true;
                            Rectangle r = item.model.InvalidRectangle;
                            InvalidRect = new Rectangle((int)(r.X + -Gamer.Position.X + Width / 2f), (int)(r.Y + -Gamer.Position.Y + Height / 2f), r.Width, r.Height);
                            if (NeedInvalidation != null)
                                NeedInvalidation(this, EventArgs.Empty);
                        }
                    }
                };
            }
			enemiesLowTimer.Stop();
            enemiesMediumTimer.Stop();
            enemiesHighTimer.Stop();

			aw.AllLineSplit(80);
			aw.MapModels();

			//            IShape l;
			//            for (int a = 0; a < 4; a++)
			//            {
			////				l = new Line(new Vector2(rnd.Next(0,666), rnd.Next(0,255)), new Vector2(rnd.Next(667,1251), rnd.Next(255,551)));
			//                l = new Line(new Vector2(00, 600), new Vector2(300, 500));
			//                f = new Frame(0f);
			//                t = new Transform();
			//                pf = new PhysicalFeatures(Vector2.Zero, ((Line)l).Start - ((Line)l).End);
			//                pf.Mode = PhysicMode.Off;
			//                aw.Add(new StaticModel(ref f, ref l, ref t, pf));
			//            }
			//            l = new Line(new Vector2(200, 550), new Vector2(600, 550));
			//            f = new Frame(0f);
			//            t = new Transform();
			//            pf = new PhysicalFeatures(Vector2.Zero, ((Line)l).Start - ((Line)l).End);
			//            pf.Mode = PhysicMode.Off;
			//            aw.Add(new StaticModel(ref f, ref l, ref t, pf));
			//            l = new Line(new Vector2(600, 550), new Vector2(900, 250));
			//            f = new Frame(0f);
			//            t = new Transform();
			//            pf = new PhysicalFeatures(Vector2.Zero, ((Line)l).Start - ((Line)l).End);
			//            pf.Mode = PhysicMode.Off;
			//            aw.Add(new StaticModel(ref f, ref l, ref t, pf));
			//            l = new Line(new Vector2(900, 250), new Vector2(950, 200));
			//            f = new Frame(0f);
			//            t = new Transform();
			//            pf = new PhysicalFeatures(Vector2.Zero, ((Line)l).Start - ((Line)l).End);
			//            pf.Mode = PhysicMode.Off;
			//            aw.Add(new StaticModel(ref f, ref l, ref t, pf));
			//            l = new Line(new Vector2(950, 200), new Vector2(990, 250));
			//            f = new Frame(0f);
			//            t = new Transform();
			//            pf = new PhysicalFeatures(Vector2.Zero, ((Line)l).Start - ((Line)l).End);
			//            pf.Mode = PhysicMode.Off;
			//            aw.Add(new StaticModel(ref f, ref l, ref t, pf));
			//            l = new Line(new Vector2(990, 250), new Vector2(1091, 550));
			//            f = new Frame(0f);
			//            t = new Transform();
			//            pf = new PhysicalFeatures(Vector2.Zero, ((Line)l).Start - ((Line)l).End);
			//            pf.Mode = PhysicMode.Off;
			//            aw.Add(new StaticModel(ref f, ref l, ref t, pf));
			//            l = new Line(new Vector2(990, 550), new Vector2(1366, 250));
			//            f = new Frame(0f);
			//            t = new Transform();
			//            pf = new PhysicalFeatures(Vector2.Zero, ((Line)l).Start - ((Line)l).End);
			//            pf.Mode = PhysicMode.Off;
			//            aw.Add(new StaticModel(ref f, ref l, ref t, pf));

			//			l = new Line(new Vector2(250, 350), new Vector2(350, 400));
			//			c = new Circle(20, new Vector2(265, 265));
			//			f = new Frame(0f);
			//			t = new Transform();
			//			pf = new PhysicalFeatures(Vector2.Zero);
			//			pf.Mode = PhysicMode.On;
			//			IShape sg = new ShapeGroup(c, l, new Circle(20, new Vector2(300,250)));
			//			aw.Add(new StaticObject(ref f, ref sg, ref t, pf));

			//l = new Line(new Vector2(60, 66), new Vector2(1300, 66));
			//f = new Frame(0f);
			//t = new Transform();
			//pf = new PhysicalFeatures(Vector2.Zero);
			//pf.Mode = PhysicMode.Off;
			//aw.Add(new StaticModel(ref f, ref l, ref t, pf));
			//l = new Line(new Vector2(330, 170), new Vector2(360, 140));
			//f = new Frame(0f);
			//t = new Transform();
			//pf = new PhysicalFeatures(Vector2.Zero);
			//pf.Mode = PhysicMode.Off;
			//aw.Add(new StaticModel(ref f, ref l, ref t, pf));
			//aw.SaveMap("map.xml");
			//			aw.SortIndices(); // mivel később már csak insert lesz. Ugyeeeee?


			//			Enemy.Inval += (object sender, EventArgs e) => OnPaint(new PaintEventArgs(CreateGraphics(), ClientRectangle));



//			Invalidate();

			//			timer.Interval = 1000;
			//			timer.Elapsed += HandleTimerElapsed;
			//timer.Start();
		}
		public void OnKeyDown(object sender, KeyEventArgs e) {
			Control ctrl = (sender as Control);
			if (e.KeyData == Keys.R) {
				Init();
				enemiesLowTimer.Stop();
                enemiesMediumTimer.Stop();
                enemiesHighTimer.Stop();
				ctrl.Invalidate();
			}
            if (e.KeyData == Keys.X)
            {
                thr.Start();
                enemiesMediumTimer.Start();
            }
			if (e.KeyData == Keys.A) {
				//				aw.GetAreaShapes(Enemy.model);
                Enemies.ForEach(item => { 
                    item.StepTo(Gamer, 6f);
                    item.ScanComplete += delegate(object s, EventArgs ea)
                    {
                        //Console.WriteLine(Thread.CurrentThread.Name);
                        //ctrl.Invalidate();
                        //item.BindFollow(Gamer, 6f);
                    };
                });
                enemiesLowTimer.Start();
                //Thread.Sleep(10);
                //enemiesMediumTimer.Start();
                //Thread.Sleep(10);
                //enemiesHighTimer.Start();
				//				Thread thr0 = new Thread(new ThreadStart(delegate() {
				//
				//					//Gamer.DetectedPoints(Gamer.Position);
				//				}));
				//				thr0.IsBackground = true;
				//				thr0.Priority = ThreadPriority.Lowest;
				//				thr0.Start();
			}
			if (e.KeyData == Keys.Q) {
				Enemies[0].StepTo(Gamer, 2f);
			}
			if (e.KeyData == Keys.Up) {
                //Thread.Sleep(10);
                lock (this)
                {
                    if (Gamer.TryMove(MousePos - Gamer.Position, 2f))
                    {
                        Rectangle r = Gamer.model.InvalidRectangle;
                        //ctrl.Invalidate(new Rectangle((int)(r.X - Gamer.Position.X + Width / 2f), (int)(r.Y - Gamer.Position.Y + Height / 2f), r.Width, r.Height));
						NeedInvalidate = true;
						InvalidRect = new Rectangle(0, 0, Width, Height);
						if (NeedInvalidation != null)
							NeedInvalidation(this, EventArgs.Empty);
                    } else
                    {

                    }
                }
			}
			if (e.KeyData == Keys.Right) {
				Gamer.TryMove(Vector2.Rotate(MousePos - Gamer.Position, 1.57f), 1.5f);
			}
			if (e.KeyData == Keys.Left) {
				Gamer.TryMove(Vector2.Rotate(MousePos - Gamer.Position, -1.57f), 1.5f);
			}
			if (e.KeyData == Keys.Down) {
				Gamer.TryMove(MousePos - Gamer.Position, -3f);
			}

		}
		public void OnKeyUp(object sender, KeyEventArgs e) {

		}

		public void OnMouseMove(object sender, MouseEventArgs e) {
			MousePos = new Vector2(e.X + Gamer.Position.X - Width / 2f, e.Y + Gamer.Position.Y - Height / 2f);
		}
		public void HandleResize(object sender, EventArgs e)
		{
			Control c = (Control)sender;
			Width = c.Width;
			Height = c.Height;
			OnPaint(this, null);
			c.Invalidate();
		}

		DateTime lastPaint;
		double totalMsAw;
		bool isDrawed;
		public void OnPaint(object sender, PaintEventArgs e) {

			if (IsRendering)
			{
				bool wasSec = false;
				elapsedTime += DateTime.Now - lastPaint;
				lastPaint = DateTime.Now;
				if (elapsedTime.TotalMilliseconds >= 1000)
				{
					fps = Fps = fpsCounter;
					fpsCounter = 0;
					elapsedTime = TimeSpan.Zero;
					totalMsAw = aw.ElapsedTest.TotalMilliseconds;
					wasSec = true;
				}

//				Graphics g = e.Graphics;
				g.Clear(Color.White);

				g.DrawString(fps.ToString(), new Font(FontFamily.Families[80], 19f), Brushes.Green, new PointF());
				g.DrawString(totalMsAw.ToString(), new Font(FontFamily.Families[80], 10f), Brushes.Green, new PointF(0, 40));
				g.DrawString(MousePos.ToString(), new Font(FontFamily.Families[80], 10f), Brushes.Green, new PointF(0, 80));

				Matrix m = new Matrix(1, 0, 0, 1, -Gamer.Position.X + Width / 2f, -Gamer.Position.Y + Height / 2f);
				g.Transform = m;

				//			Console.WriteLine(fps);

				//			for (int a = 0; a < aw.DistanceIndices.Count; a++) {
				//				StaticModel obj = aw.GameObjects[aw.DistanceIndices[a]];
				//				if (aw.GameObjects[aw.DistanceIndices[a]].Shape is Circle) {
				for (int a = 0; a < aw.GameObjects.Count; a++)
				{
					StaticModel obj = aw.GameObjects[a];
					Circle bc = obj.Shape.BoundingCircle;
					g.DrawCircle(new Pen(Brushes.Green), bc);
//					g.DrawString(aw.BoundNumber(a).ToString(), new Font(FontFamily.Families[80], 14f), Brushes.Green, new PointF(bc.center.X-5, bc.center.Y-5));
                    if (obj.UpdateCommands != null)
                        obj.UpdateCommands();
					if (aw.GameObjects[a].Shape is Circle)
					{
						Circle drwCirc = aw.GameObjects[a].Shape.BoundingCircle;
						// new System.Drawing.RectangleF(obj.Transformation.Translate.X-18, obj.Transformation.Translate.Y-24, 36, 42)
						if (obj.Frame.Drawed == Entities.DrawMode.Bitmap)
							;
						//						g.DrawImage(obj.Frame.Bitmap, 
						//					    	obj.DrawPoints,
						//					    	obj.Frame.SourceRectangle, GraphicsUnit.Pixel);
						else ;
						g.DrawCircle(pen, drwCirc);

						Vector2 v = (drwCirc.center - drwCirc.radius);
//						if (Vector2.Distance(v) > Math.Sqrt(bmp.Width * bmp.Width + bmp.Height * bmp.Height))
//							break; // vagy ha kisebb mint x=0 & y=0
					}
					else if (aw.GameObjects[a].Shape is Line)
					{//aw.GameObjects[aw.DistanceIndices[a]]
						Line drwLine = aw.GameObjects[a].Shape as Line;
						//g.DrawLine(new Pen(Brushes.Black), new Point(0, 0), new Point(1500, 500));
						//g.DrawCircle(new Pen(Brushes.Bisque), new Circle(33, new Vector2(32,33)));
						g.DrawLine(new Pen(Brushes.Black), (Point)drwLine.Start, (Point)drwLine.End);
						//g.DrawLines
						//						g.DrawLine(pen, new Point(33,344),
						//						           new Point((int)drwLine.Start.X, (int)drwLine.Start.Y),
						//						           new Point((int)drwLine.End.X, (int)drwLine.End.Y));
						//						           new Point());
					}
					else if (aw.GameObjects[a].Shape is ShapeGroup)
					{
						ShapeGroup sGroup = aw.GameObjects[a].Shape as ShapeGroup;
						for (int j = 0; j < sGroup.Members.Count; j++)
						{
							if (sGroup.Members[j] is Circle)
							{
								//g.DrawImage(obj.Frame.Bitmap, obj.Frame.SourceRectangle);
							}
							else if (sGroup.Members[j] is Line)
							{
							}
							//...
						}
					}
					//...

				}

//                for (int i = 0; i < Enemy[0].lookAt.Count; i++)
//                {
//                    g.FillCircle(Brushes.Red, new Circle(2f, Enemy[0].lookAt[i]));
//                    //				g.DrawString(Vector2.Angle(Vector2.Normailze(Gamer.lookAt[i] - Enemy.Position)).ToString(), new Font(FontFamily.Families[80],10f), Brushes.Green, 
//                    //					new PointF(Gamer.lookAt[i].X - 10, Gamer.lookAt[i].Y+5));
//                }
//                Enemy[0].lookAt.Clear();
//                Enemy[0].lines.Clear();
//                for (int i = 0; i < Enemy[0].lines.Count; i++)
//                {
//                    g.DrawLine(new Pen(Brushes.LightGreen, 2f), (Point)Enemy[0].lines[i].Start, (Point)Enemy[0].lines[i].End);
//                }
//                for (int i = 0; i < Enemy[0].way.Count; i++)
//                {
//                    g.FillCircle(Brushes.Green, new Circle(3f, Enemy[0].way[i]));
//                }
//				g.FillCircle(Brushes.Blue, new Circle(5f, new Vector2(Collision.X1, Collision.Y1)));
//				g.FillCircle(Brushes.Blue, new Circle(5f, new Vector2(Collision.X2, Collision.Y2)));

				fpsCounter++;
				//			if (fps > 155) // meg ha van szabad időnk, de ez egy kicsit önbizalomhiányra utal Hmm
				//				Invalidate(); // a tökéletes algo mégsem tökéletes? :OOO
				//			else
				//				Invalidate(invalidRect);

				//				Thread.Sleep(1);
//				((Control)sender).Invalidate();
				isDrawed = true;

			}
		}
		/*protected bool GamePaint(int a, Paints pts, Graphics g) {
			this.g = g;
			StaticObject obj = aw.GameObjects[aw.DistanceIndices[a]];
			if (aw.GameObjects[aw.DistanceIndices[a]].Shape is Circle) {
				Circle drwCirc = aw.GameObjects[aw.DistanceIndices[a]].Shape.BoundingCircle;
				// new System.Drawing.RectangleF(obj.Transformation.Translate.X-18, obj.Transformation.Translate.Y-24, 36, 42)
				if (obj.Frame.Drawed == Entities.DrawMode.Bitmap)
					g.DrawImage(obj.Frame.Bitmap, 
				    	obj.DrawPoints,
				    	obj.Frame.SourceRectangle, GraphicsUnit.Pixel);
				else
					g.DrawCircle(pen, drwCirc);

				Vector2 v = (drwCirc.center - drwCirc.radius);
				if (Vector2.Distance(v) > Math.Sqrt(bmp.Width * bmp.Width + bmp.Height * bmp.Height))
					return true; // vagy ha kisebb mint x=0 & y=0
			}
			else if (aw.GameObjects[aw.DistanceIndices[a]].Shape is Line) {
//				if (pts == Paints.OnlyBmp)
//					g.DrawImage(obj.Frame.Bitmap, obj.Frame.SourceRectangle);
//				else if (pts == Paints.OnlyLine) {
					Line drwLine = aw.GameObjects[aw.DistanceIndices[a]].Shape as Line;
//					g.DrawLine(new Pen(Brushes.Bisque), new PointF(drwLine.Start.X, drwLine.Start.Y), new PointF(drwLine.End.X, drwLine.End.Y));
//					g.DrawEllipse(pen, new System.Drawing.Rectangle(0,0,100,100));
//					g.DrawLine(new Pen(Brushes.Bisque), new PointF(22,22), new PointF(33,33));

//				}
//				else if (pts == Paints.All) {
//					g.DrawImage(obj.Frame.Bitmap, obj.Frame.SourceRectangle);
//					Line drwLine = aw.GameObjects[aw.DistanceIndices[a]].Shape as Line;
//					g.DrawLine(pen, (Point)drwLine.Start, (Point)drwLine.End);
//				}
			}
			else if (aw.GameObjects[aw.DistanceIndices[a]].Shape is ShapeGroup) {
				ShapeGroup sGroup = aw.GameObjects[aw.DistanceIndices[a]].Shape as ShapeGroup;
				for (int j = 0; j < sGroup.Members.Count; j++) {
					if (sGroup.Members[j] is Circle) {
						g.DrawImage(obj.Frame.Bitmap, obj.Frame.SourceRectangle);
					}
					else if (sGroup.Members[j] is Line) {
						if (pts == Paints.OnlyBmp)
							g.DrawImage(obj.Frame.Bitmap, obj.Frame.SourceRectangle);
						else if (pts == Paints.OnlyLine) {
							Line drwLine = sGroup.Members[j] as Line;
							g.DrawLine(pen, (Point)drwLine.Start, (Point)drwLine.End);
						}
						else if (pts == Paints.All) {
							g.DrawImage(obj.Frame.Bitmap, obj.Frame.SourceRectangle);
							Line drwLine = sGroup.Members[j] as Line;
							g.DrawLine(pen, (Point)drwLine.Start, (Point)drwLine.End);
						}
					}
					//...
				}
			}
			//...
			return false;
		}
		protected bool DebugPaint(int a) {
			if (aw.GameObjects[aw.DistanceIndices[a]].Shape is Circle) {
				Circle drwCirc = aw.GameObjects[aw.DistanceIndices[a]].Shape.BoundingCircle;
				g.DrawCircle(pen, drwCirc);
				//g.FillCircle(new SolidBrush(System.Drawing.Color.FromArgb((int)(aw.GameObjects[aw.DistanceIndices[a]].Physics.Weight*255f), 1, 1)), drwCirc);
				Vector2 v = (drwCirc.center - drwCirc.radius);
				if (Vector2.Distance(v) > Math.Sqrt(bmp.Width * bmp.Width + bmp.Height * bmp.Height))
					return true; // vagy ha kisebb mint x=0 & y=0
			}
			else if (aw.GameObjects[aw.DistanceIndices[a]].Shape is Line) {
				Line drwLine = aw.GameObjects[aw.DistanceIndices[a]].Shape as Line;
				g.DrawLine(pen, (Point)drwLine.Start, (Point)drwLine.End);
			}
			else if (aw.GameObjects[aw.DistanceIndices[a]].Shape is ShapeGroup) {
				ShapeGroup sGroup = aw.GameObjects[aw.DistanceIndices[a]].Shape as ShapeGroup;
				for (int j = 0; j < sGroup.Members.Count; j++) {
					if (sGroup.Members[j] is Circle) {
						Circle drwCirc = sGroup.Members[j].BoundingCircle;
						g.DrawCircle(pen, drwCirc);
					}
					else if (sGroup.Members[j] is Line) {
						Line drwLine = sGroup.Members[j] as Line;
						g.DrawLine(pen, (Point)drwLine.Start, (Point)drwLine.End);
					}
					//...
				}
			}
			//...
			//				g.DrawArrow(aw.GameObjects[aw.DistanceIndices[a]].Transformation.Translate,
//				aw.GameObjects[aw.DistanceIndices[a]].Transformation.Translate + aw.GameObjects[aw.DistanceIndices[a]].Physical.Velocity, 48f);
//				Circle ccd = aw.GameObjects[aw.DistanceIndices[a]].Shape as Circle;
//				if (ccd != null)
//					if (aw.GameObjects[aw.DistanceIndices[a]].Meta.Collosed != null)

//						g.DrawLine(new Pen(Brushes.Red, 3f), (Point)aw.GameObjects[aw.DistanceIndices[a]].Transformation.Translate, 
//						           (Point)(aw.GameObjects[aw.DistanceIndices[a]].Transformation.Translate+aw.GameObjects[aw.DistanceIndices[a]].Physical.Velocity));
			g.DrawArrow(aw.GameObjects[aw.DistanceIndices[a]].Transformation.Translate, 
	        	(aw.GameObjects[aw.DistanceIndices[a]].Transformation.Translate+
	        	aw.GameObjects[aw.DistanceIndices[a]].Physical.Velocity), 55f);
			if (AreaWorker.line != null)
				g.DrawLine(pen, (Point)AreaWorker.line.Start, (Point)AreaWorker.line.End);
			g.DrawLine(new Pen(Brushes.Red), new Point(100,100), (Point)Vector2.Rotate(new Vector2(140,100), GraphicsExtension.fff, new Vector2(100,100)));
			g.DrawLine(pen, new Point(140,100), new Point(100,100));
			g.DrawLine(pen, (Point)Physics.Pont1, (Point)Physics.Pont2);
			g.FillRectangle(Brushes.Red, Collision.X1, Collision.Y1, 5, 5);
			g.FillRectangle(Brushes.Blue, Collision.X2, Collision.Y2, 5, 5);
			g.FillRectangle(Brushes.Green, Collision.lny.X-3, Collision.lny.Y-3, 3, 3);

			g.DrawArrow(Gamer.Model.Transformation.Translate, MousePos, 0f);
//			Circle ccc = aw.GameObjects[aw.GameObjects.Count - 1].Shape.BoundingCircle();
//			g.DrawCircle(pen, ccc);

			return false;
		}*/
	}
}


