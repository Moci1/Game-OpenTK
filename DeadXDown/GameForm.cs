using System;
using System.Timers;
using System.Drawing;
using System.Threading;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using InternalSection;
using System.Collections.Generic;
using Geometry.Shapes;
using Entities;
using glControlKit;

namespace DeadXDown
{
	public class GameForm : GameWindow
	{
		View view;
		Spritebatch spritebatch;
		Texture2D texture;

		DateTime lastPaint, lastUpdate;
		double totalMsAw;
        RectArea aw;
		float fpsCounter=0, lastFps = 1, fps=1, updateCounter = 0, uFps = 1, updateCounter1; // fps
		TimeSpan elapsedTime, uElapsedTime; // fps, performanceBallance
		Random rnd = new Random();
		InternalSection.Vector2 MousePos = new InternalSection.Vector2();
		System.Timers.Timer enemiesTimer;
		public Player Gamer { get; private set; }
		public List<Player> Enemies { get; private set; } // ezeket külön lehetne választani. pl 1 akik mindig követnek lista. és akkor azokat felváltva bejárni ahogy most is van
		public float Fps { get; set; } // vagy csak az objektek indexét tároljuk külön....

		Slider sld;
		Button btn;
		Control rootWin;


		public GameForm(int w, int h) : base(w, h)
		{
			spritebatch = new Spritebatch();
			Input.Initialize(this);

			view = new View(OpenTK.Vector2.Zero, 1.0, 0.0); // change view position, zoom and rotation
		}

		public void LoadGame() {
			aw = new RectArea(new RectangleF( -200, -200, 3000, 2000));
			Enemies = new List<Player>();
			aw.LoadMapFromFile("map5.txt");

			IShape c = new Circle(40f, new InternalSection.Vector2(10,100));
			Transform t = new Transform();
			Frame f = new Frame(0f);
			PhysicalFeatures pf = new PhysicalFeatures(.001f, new InternalSection.Vector2(0,0), 1);
			int x = 0;

			c = new Circle(16, new InternalSection.Vector2(rnd.Next(200,1301), rnd.Next(103,355)));
			//f = new Frame(new Bitmap("preview.png"), new System.Drawing.Rectangle(0,0, 60,90), Entities.DrawMode.Bitmap);
			f = new Frame(0f);
			t = new Transform();
			pf = new PhysicalFeatures((float)rnd.NextDouble(), new InternalSection.Vector2(0,0), 1);
			pf.Mode = PhysicMode.On;
			aw.Add(new AnimModel(ref f, ref c, ref t, pf));
			Gamer = new Player((AnimModel)aw.GameObjects[aw.GameObjects.Count - 1]);
			Gamer.Area = aw;

			for (int a = 0; a < 1; a++) {
				f = new Frame(0f);
				t = new Transform();
				c = new Circle(10, new InternalSection.Vector2(rnd.Next(30, 1201), rnd.Next(30, 600)));
				pf = new PhysicalFeatures((float)rnd.NextDouble(), new InternalSection.Vector2(0, 0), 1);
				pf.Mode = PhysicMode.On;
				aw.Add(new AnimModel(ref f, ref c, ref t, pf));
				Enemies.Add(new Player((AnimModel)aw.GameObjects[aw.GameObjects.Count - 1]));
				Enemies[a].Area = aw;
                //Enemies[a].ScanComplete += (s, e) =>
                //{

                //};
			}
			if (enemiesTimer == null) {
				enemiesTimer = new System.Timers.Timer();
				enemiesTimer.Interval = 80;
				enemiesTimer.Elapsed += delegate(object sender, ElapsedEventArgs eea) {
                    for (int a = 0; a < Enemies.Count; a++)
                    {
                        Enemies[a].StepTo(Gamer, 20f);
                        //Thread.Sleep(25);
                    }
				};
			}
			enemiesTimer.Stop();

            aw.AllLineSplit(180);
            aw.MapModels();

			view = new View(new OpenTK.Vector2(Gamer.Position.X, Gamer.Position.Y), 1.0, 0.0); // change view position, zoom and rotation
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			LoadGame();
			texture = ContentPipe.LoadTexture("302.jpg");
			rootWin = new RootWin(); // abstract miatt kell egy WinControl v vmi hasonlo
			rootWin.Width = Width;
			rootWin.Height = Height;
			sld = new Slider(rootWin);
			Bitmap offBmp = new Bitmap(100, 23), onBmp = new Bitmap(100, 23);
			for (int x = 0; x < offBmp.Width; x++) {
				for (int y = 0; y < offBmp.Height; y++) {
					offBmp.SetPixel(x, y, Color.Red);
					onBmp.SetPixel(x, y, Color.Green);
				}
			}
			btn = new Button(rootWin, offBmp, onBmp);
			//btn.Location = new PointF(10, 50);
			btn.SetInputSource(this);
			sld.SetInputSource(this);
		}

		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			base.OnMouseMove(e);
			MousePos = new InternalSection.Vector2(e.X + Gamer.Position.X - Width / 2f, e.Y + Gamer.Position.Y - Height / 2f);
		}
        int enemyId = -1, enemyIdStep, enemyIdMax = 4;

		protected override void OnUpdateFrame(FrameEventArgs e)
		{
			updateCounter1++;
			uElapsedTime += DateTime.Now - lastUpdate;
			lastUpdate = DateTime.Now;
			if (uElapsedTime.TotalMilliseconds >= 100.0)
			{
				float ff = 100f / (float)uElapsedTime.TotalMilliseconds;
				uFps = (updateCounter * ff) * 10f;
//				Console.WriteLine(uFps);
				updateCounter = 0;
				uElapsedTime = TimeSpan.Zero;
			}

			base.OnUpdateFrame(e);
            

			if (Input.KeyDown(Key.R)) {
				LoadGame();
				enemiesTimer.Stop();
			}
            if (Input.KeyDown(Key.S))
            {
                enemyId = 0;
                enemyIdStep = enemyIdMax;
            }
            if (enemyId != -1)
            {
				Console.ForegroundColor = ConsoleColor.Red;
//				Console.WriteLine(enemyIdMax);
				Console.ForegroundColor = ConsoleColor.Black;
				while (enemyId <= enemyIdStep && enemyId < Enemies.Count) //  
                { // a lényeg hogy a speed képernyőn is akkora legyen amekkorára állítjuk 1 obj meg n obj  esetén is. Így fps-hez igazítva nem lehet pontosan beállítani,de több a semminél
					// speed: .1f * (1f + lastFps) / fps
					Enemies[enemyId++].StepTo(Gamer, 1f); // TODO: (PerfMan) mert 60 fps az már jo annál jobb nem kell, 
                    if (enemyId == Enemies.Count - 1) // de  ha akarod lehet több is és akkor kicsi fpsnél gyorsabb lesz nagynál meg lasabb
                    {// a speed-et az Enemies.Count-al arányosan váltani kell, ha nagyobb akkor több speed, ha kevesebb akkor kisebb speed. (PerformanceManager!)
                        enemyId = 0;
                        enemyIdStep = enemyIdMax;
                    }
                    if (enemyId == enemyIdStep)
                    {
                        enemyIdStep += enemyIdMax;
                        break;
                    }
                }
                if (enemyId >= Enemies.Count)
                {
                    enemyId = 0;
                    enemyIdStep = enemyIdMax;
                }
                if (fps < 30 && enemyIdMax > 1)
                {
                    enemyIdMax--;
                } else if (fps > 30 && updateCounter1 > 200 && enemyIdMax < Enemies.Count - 1) // TODO: updateCounter to PerfManager, ja meg kéne olyan is h 1 event mikor sok a szabad erőforrás
                {
                    if (updateCounter1 > 100)
                    {
                        enemyIdMax++;
                        updateCounter1 = 0;
                    }
                }
            }
			if (Input.KeyDown(Key.A)) {
                //Enemies.ForEach(item => { 
                //    item.BindFollow(Gamer, 20f);
                //    item.ScanComplete += delegate(object s, EventArgs ea)
                //    {
                //        //Console.WriteLine(Thread.CurrentThread.Name);
                //        //ctrl.Invalidate();
                //        //item.BindFollow(Gamer, 20f);
                //    };
                //});
				enemiesTimer.Start();

				//				Thread thr0 = new Thread(new ThreadStart(delegate() {
				//
				//					//Gamer.DetectedPoints(Gamer.Position);
				//				}));
				//				thr0.IsBackground = true;
				//				thr0.Priority = ThreadPriority.Lowest;
				//				thr0.Start();
			}
			if (Input.KeyDown(Key.Q)) {
				Enemies[0].StepTo(Gamer, 2000f);
			}
			if (Input.KeyDown(Key.W)) {
				Enemies[0].TryMove(MousePos - Enemies[0].Position, 1f);
			}
			if (Input.KeyDown(Key.Up) && Gamer.isEndMove) { // TODO: irányítás!!!
//				Thread.Sleep(10);

				float spd = 2f * (1f + lastFps) / fps; // 1 a speed
				if (Gamer.TryMove(MousePos - Gamer.Position, 2f)) {
					Rectangle r = Gamer.InvalidRectangle;
					//ctrl.Invalidate(new Rectangle((int)(r.X - Gamer.Position.X + Width / 2f), (int)(r.Y - Gamer.Position.Y + Height / 2f), r.Width, r.Height));
					//ctrl.Invalidate();
					InternalSection.Vector2 vec = MousePos - Gamer.Position;
					vec = InternalSection.Vector2.Normailze(vec); // 2x-es Normalize az jó?
					InternalSection.Vector2 trs = Gamer.Position; // Gamer.model.Transformation.Translate;
					float x = vec.X * (2), y = vec.Y * (2);
					view.Position.X += x;
					view.Position.Y += y;
				} else {

				}

			}
			if (Input.KeyDown(Key.Right)) {
				Gamer.TryMove(InternalSection.Vector2.Rotate(MousePos - Gamer.Position, 1.57f), 1.5f);
			}
			if (Input.KeyDown(Key.Left)) {
				Gamer.TryMove(InternalSection.Vector2.Rotate(MousePos - Gamer.Position, -1.57f), 1.5f);
			}
			if (Input.KeyDown(Key.Down)) {
				Gamer.TryMove(MousePos - Gamer.Position, -3f);
			}



			Input.Update();
			view.Update();
			updateCounter++;
		}


		protected override void OnRenderFrame(FrameEventArgs e)
		{
			base.OnRenderFrame(e);

			elapsedTime += DateTime.Now - lastPaint;
			lastPaint = DateTime.Now;
			if (elapsedTime.TotalMilliseconds >= 100.0) {
				lastFps = fps;
				float ff = 100f / (float)elapsedTime.TotalMilliseconds;
				fps = Fps = (fpsCounter * ff) * 10f;
				//Console.WriteLine(fps);
				fpsCounter = 0;
				elapsedTime = TimeSpan.Zero;
				totalMsAw = aw.ElapsedTest.TotalMilliseconds;
			}

			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.ClearColor(Color.White);

			spritebatch.ResetScreen(Width, Height);
			sld.OnPaint();
			btn.OnPaint();
			view.ApplyTransform();
//			if (Enemies[0].sensorPoints != null)
//			for (int i = 0; i< Enemies[0].sensorPoints.Count; i++) {
//				spritebatch.DrawPolygon(Enemies[0].sensorPoints[i].X, Enemies[0].sensorPoints[i].Y, 2, 4);
//			} //hmm i dont need

			for (int a = 0; a < aw.GameObjects.Count; a++)
			{
				StaticModel obj = aw.GameObjects[a];
//				Circle bo = aw.GameObjects[a].Shape.BoundingCircle;
//				spritebatch.DrawPolygon(bo.center.X, bo.center.Y, bo.radius, 24);
//				if (obj.UpdateCommands != null)
//					obj.UpdateCommands();
				if (aw.GameObjects[a].Shape is Circle)
				{
					Circle drwCirc = aw.GameObjects[a].Shape.BoundingCircle;
					spritebatch.DrawPolygon(drwCirc.center.X, drwCirc.center.Y, drwCirc.radius, 8);

					InternalSection.Vector2 v = (drwCirc.center - drwCirc.radius);

//					if (InternalSection.Vector2.Distance(v) > Math.Sqrt(Width * Width + Height * Height))
//						break; // vagy ha kisebb mint x=0 & y=0
				}
				else if (aw.GameObjects[a].Shape is Line)
				{
					Line drwLine = aw.GameObjects[a].Shape as Line;
					InternalSection.Vector2 srt = drwLine.Start, end = drwLine.End;
					spritebatch.DrawLine(srt.X, srt.Y, end.X, end.Y);
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
			this.SwapBuffers();
			fpsCounter++;
		}
		void test346() {
			//			spritebatch.Begin(Width, Height);
			//			view.ApplyTransform();
			//			GL.Color3(Color.Red);
			//
			//			spritebatch.DrawTexture(texture, Vector2.Zero, new Vector2(2f, 2f), Color.Red, new Vector2(10f, 10f));
			//			spritebatch.DrawLines(30f, new Vector2[] { 
			//				new Vector2(0,0), new Vector2(30,30), new Vector2(123,10), new Vector2(45, 33)
			//			});
			//			GL.Color3(Color.White);
			//			spritebatch.DrawLines(30f, new Vector2[] { 
			//				new Vector2(-5,-5), new Vector2(-30,-30), new Vector2(-123, -10), new Vector2(-45, -33)
			//			});
			//			GL.Color3(Color.Green);
			//			spritebatch.DrawCircle(23f, 44f, 60f, 20, 3f);
		}
	}
}

