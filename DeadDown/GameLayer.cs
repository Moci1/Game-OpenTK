using System;
using System.Timers;
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
	public class GameLayer : Panel
	{
		//Graphics g;
		Pen pen;
		Bitmap bmp;
		Graphics g;
		SimpleArea aw;
		int fpsCounter=0, fps=0; // fps
		TimeSpan elapsedTime; // fps
		Random rnd = new Random();
		Rectangle invalidRect;
		Vector2 playerDirToMouse;
		System.Timers.Timer timer = new System.Timers.Timer();
		public Vector2 MousePos { get; private set; }
		public Player Gamer { get; private set; }
		public List<Player> Enemy { get; private set; }
		public bool IsDebugMode { get; set; }
        public bool IsRendering { get; set; }

		public GameLayer() : base()
		{
//			bmp = new Bitmap(1500, 1500);
//			g = Graphics.FromImage(bmp);
			this.DoubleBuffered = true; 
			this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            this.Size = new Size(1300, 650);
			this.ResizeRedraw = true;
            IsRendering = false;
			Button button = new Button();
			button.Size = new Size(70,21);
			button.Location = new Point(100, 20);
			button.Text = "Reset";
			button.Click += HandleButtonClick;
			this.Controls.Add(button);
		}
		protected override Size DefaultSize {
			get {
				return base.DefaultSize;
			}
		}
		protected override void OnClientSizeChanged(EventArgs e)
		{
			base.OnClientSizeChanged(e);
		}
		protected override void OnInvalidated(InvalidateEventArgs e)
		{
		//	Invalidate();
			base.OnInvalidated(e);

		}
		protected override void OnValidating(System.ComponentModel.CancelEventArgs e)
		{
			base.OnValidating(e);

		}
		protected override void OnImeModeChanged(EventArgs e)
		{
			base.OnImeModeChanged(e);
		}
		void HandleButtonClick (object sender, EventArgs e)
		{
            IsRendering = true;
			Init();
			Focus();
		}
		protected override CreateParams CreateParams {
  			get {
    			CreateParams cp = base.CreateParams;
    			cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
    			return cp;
  			}
		}
		protected override void OnGotFocus(EventArgs e) {
			
		}
		
		public void Init() {
			aw = new SimpleArea();
			pen = new Pen(Brushes.Black, 2f);
			Enemy = new List<Player>();
            aw.LoadMapFromFile("map3.txt");
			//aw.LoadMap(new Bitmap(@"map1.jpg"));

			IShape c = new Circle(40f, new Vector2(10,100));
			Transform t = new Transform();
			Frame f = new Frame(0f);
			PhysicalFeatures pf = new PhysicalFeatures(.001f, new Vector2(0,0), 1);
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
				Enemy.Add(new Player((AnimModel)aw.GameObjects[aw.GameObjects.Count - 1]));
				Enemy[a].Area = aw;
			}
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


			System.Windows.Forms.Timer drawThr = new System.Windows.Forms.Timer();
			drawThr.Tick += delegate(object sender, EventArgs e) {
				if ((DateTime.Now - lastPaint).TotalMilliseconds > 200) {
					DrawBmp();
					Invalidate();
				}
			};
			drawThr.Interval = 100;
			drawThr.Stop();
			drawThr.Start();
			//Invalidate();
			
//			timer.Interval = 1000;
//			timer.Elapsed += HandleTimerElapsed;
			//timer.Start();
		}

		void HandleTimerElapsed(object sender, EventArgs e)
		{
			Invalidate();
//			if (isDrawed) {
//				int a = rnd.Next(4,10);
//				Circle c = aw.GameObjects[a].Shape.BoundingCircle();
//				aw.GameObjects[a].Move(c.center.X + 10, c.center.Y);
//				invalidRect.X = (int)aw.GameObjects[a].InvalidRectangle.X - 10;
//				invalidRect.Y = (int)aw.GameObjects[a].InvalidRectangle.Y - 10;
//				invalidRect.Width = (int)aw.GameObjects[a].InvalidRectangle.Width + 20;
//				invalidRect.Height = (int)aw.GameObjects[a].InvalidRectangle.Height + 20;
//				isDrawed = false;
//			}
		}
		protected override void OnKeyDown(KeyEventArgs e) {
//			if (isDrawed) {
//				int a = 0; //rnd.Next(4,2000);
//				Circle c = aw.GameObjects[a].Shape.BoundingCircle();
//				aw.GameObjects[a].Moving(c.center.X + 2, c.center.Y + 0);
//				invalidRect.X = (int)aw.GameObjects[a].InvalidRectangle.X - 10;
//				invalidRect.Y = (int)aw.GameObjects[a].InvalidRectangle.Y - 10;
//				invalidRect.Width = (int)aw.GameObjects[a].InvalidRectangle.Width + 20;
//				invalidRect.Height = (int)aw.GameObjects[a].InvalidRectangle.Height + 20;
//				isDrawed = false;
//			}
			if (e.KeyData == Keys.A) {
//				aw.GetAreaShapes(Enemy.model);
				Enemy.ForEach(item => item.StepTo(Gamer, 6f));
//				Thread thr0 = new Thread(new ThreadStart(delegate() {
//
//					//Gamer.DetectedPoints(Gamer.Position);
//				}));
//				thr0.IsBackground = true;
//				thr0.Priority = ThreadPriority.Lowest;
//				thr0.Start();
			}
			if (e.KeyData == Keys.Up) {
				if (Gamer.TryMove(MousePos - Gamer.Position, 3f))
					;
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
		protected override void OnKeyUp(KeyEventArgs e) {

		}

		protected override void OnMouseMove(MouseEventArgs e) {
			MousePos = new Vector2(e.X + Gamer.Position.X - Width / 2f, e.Y + Gamer.Position.Y - Height / 2f);
		}
        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            Focus();
        }
		protected void DrawBmp() {
//			base.OnPaintBackground(e);
//			e.Graphics.DrawImage(bmp, new PointF(0, 0));
		}

		DateTime lastPaint;
		double totalMsAw;
		bool isDrawed;
		protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);

            if (IsRendering)
            {
                bool wasSec = false;
                elapsedTime += DateTime.Now - lastPaint;
                lastPaint = DateTime.Now;
                if (elapsedTime.TotalMilliseconds >= 1000)
                {
                    fps = fpsCounter;
                    fpsCounter = 0;
                    elapsedTime = TimeSpan.Zero;
                    totalMsAw = aw.ElapsedTest.TotalMilliseconds;
                    wasSec = true;
                }

                Graphics g = e.Graphics;
				g.InterpolationMode = InterpolationMode.Low;
				g.CompositingMode = CompositingMode.SourceOver;
				g.CompositingQuality = CompositingQuality.HighSpeed;
				g.SmoothingMode = SmoothingMode.HighSpeed;
//                g.Clear(BackColor);

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
                        if (Vector2.Distance(v) > Math.Sqrt(Width * Width + Height * Height))
                            break; // vagy ha kisebb mint x=0 & y=0
                    }
                    else if (aw.GameObjects[a].Shape is Line)
                    {//aw.GameObjects[aw.DistanceIndices[a]]
                        Line drwLine = aw.GameObjects[a].Shape as Line;
                        //g.DrawLine(new Pen(Brushes.Black), new Point(0, 0), new Point(1500, 500));
                        //g.DrawCircle(new Pen(Brushes.Bisque), new Circle(33, new Vector2(32,33)));
                        g.DrawLine(new Pen(Brushes.Black), (Point)drwLine.Start, (Point)drwLine.End);
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
                
//                for (int i = 0; i < Enemy.lookAt.Count; i++)
//                {
//                    g.FillCircle(Brushes.Red, new Circle(2f, Enemy.lookAt[i]));
//                    //				g.DrawString(Vector2.Angle(Vector2.Normailze(Gamer.lookAt[i] - Enemy.Position)).ToString(), new Font(FontFamily.Families[80],10f), Brushes.Green, 
//                    //					new PointF(Gamer.lookAt[i].X - 10, Gamer.lookAt[i].Y+5));
//                }
//				Enemy.lookAt.Clear();
//				Enemy.lines.Clear();
//				for (int i = 0; i < Enemy.lines.Count; i++)
//				{
//					g.DrawLine(new Pen(Brushes.LightGreen, 2f), (Point)Enemy.lines[i].Start, (Point)Enemy.lines[i].End);
//				}
//                for (int i = 0; i < Enemy.way.Count; i++)
//                {
//                    g.FillCircle(Brushes.Green, new Circle(3f, Enemy.way[i]));
//                }
                g.FillCircle(Brushes.Blue, new Circle(5f, new Vector2(Collision.X1, Collision.Y1)));
                g.FillCircle(Brushes.Blue, new Circle(5f, new Vector2(Collision.X2, Collision.Y2)));

                fpsCounter++;
                //			if (fps > 155) // meg ha van szabad időnk, de ez egy kicsit önbizalomhiányra utal Hmm
                //				Invalidate(); // a tökéletes algo mégsem tökéletes? :OOO
                //			else
                //				Invalidate(invalidRect);

//				Thread.Sleep(1);
//                Invalidate();
				isDrawed = true;
				g.Flush();
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
				if (Vector2.Distance(v) > Math.Sqrt(Width * Width + Height * Height))
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
				if (Vector2.Distance(v) > Math.Sqrt(Width * Width + Height * Height))
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

