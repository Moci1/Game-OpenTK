using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Geometry.Shapes;
using System.Reflection;
using Entities;
using System.Threading;

namespace InternalSection
{
	public class SimpleArea
	{
		protected Circle circle;
		protected StaticModel obj;
		Random rnd = new Random();
        object locker = new object();
		public List<StaticModel> GameObjects { get; private set; }
		public TimeSpan ElapsedTest { get; private set; }
		public IShape CurrentTransformed { get; private set; }

		public SimpleArea()
		{
			GameObjects = new List<StaticModel>();
			AnimModel.TransformEvent += HandleTransform;
		}

        protected virtual bool HandleTransform(object sender, EventArgs e)
        {
            obj = sender as StaticModel;
            circle = obj.VirtualShape.BoundingCircle;

            bool result = false;

            //Console.WriteLine(Thread.CurrentThread.Name);
            result = CollisionForAll();
            //if (circle.radius == 16)
            //    Console.WriteLine(obj.Shape.ToString());
            //Console.WriteLine(circle.radius.ToString() + " Coll: " + result + rnd.Next());

            if (result)
                CurrentTransformed = obj.VirtualShape;
            return result;
        }
        protected bool CollisionForAll()
        {
            //Thread.Sleep(100);
            IShape me = obj.VirtualShape;
            Type t = typeof(Collision);

            Circle crc = new Circle(circle.radius * 1.1f, circle.center);
            for (int i = 0; i < GameObjects.Count; i++)
            {
                //Console.WriteLine(Thread.CurrentThread.Name);
                if (!GameObjects[i].Equals(obj))
                { // ez a kocsog beleugrik. gamert gamerral vizsgálja. előző lépést a mostanival. DE AKKOR H H CSAK NÉHA ROSSZ? meg a vonalakkal is jóba van?
                    t.InvokeMember("Intersect", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, new OverloadBinder(), null, new object[2] {
						crc,
						GameObjects[i].VirtualShape
					});
                    if (Collision.IsCollision)
                    {
                        //                        Console.WriteLine("-----------MAJDNEM Ütközött----------" + rnd.Next());
                        obj.Physical.CollisionMode = CollisionState.NearlyCollision;
                        if (!obj.Physical.Collosed.Contains(GameObjects[i].VirtualShape))
                            obj.Physical.Collosed.Add(GameObjects[i].VirtualShape);
                        t.InvokeMember("Intersect", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, new OverloadBinder(), null, new object[2] {
							me,
							GameObjects[i].VirtualShape
						});
                        if (Collision.IsCollision)
                        {
                            //                            Console.WriteLine("-----------Ütközött----------" + rnd.Next());
                            obj.Physical.CollisionMode = CollisionState.Collosed;
                            obj.Physical.Collosed.Clear();
                            obj.Physical.Collosed.Add(GameObjects[i].VirtualShape);
                            obj.Physical.Direction = Vector2.Zero;
                            return true; // minél hamarabb ütközik annál hamarabb befejezhetjük a vizsgálódást.
                        }
                        obj.Physical.CollisionMode = CollisionState.NearlyCollision;
                        obj.Physical.Collosed.Clear();
                        obj.Physical.Collosed.Add(GameObjects[i].VirtualShape);
                    }
                    if (obj.Physical.Collosed.Contains(GameObjects[i].VirtualShape))
                    {
						//obj.Physical.Collosed.Remove(GameObjects[i].VirtualShape); // itt kibszott egy exceptiont: System.IndexOutOfRangeException: index < lower bound
                        obj.LineLock = false; // ezért kikomcsiztam
                        if (obj.Physical.Collosed.Count == 0)
                        {
                            obj.Physical.CollisionMode = CollisionState.NotCollision;
                            //                            Console.WriteLine("-----------NEM Ütközött----------" + rnd.Next());
                        }
                    }
                } else
                {

                }
            }
            obj.Physical.CollisionMode = CollisionState.NotCollision;
            obj.Physical.Collosed.Clear();
            return false;
        }
		public virtual void Add(StaticModel ie) {
            ie.ObjIndex = GameObjects.Count;
			GameObjects.Add(ie);
		}
		public virtual bool Remove(StaticModel model) {
			return GameObjects.Remove(model);
		}
		public virtual void AddRange(List<StaticModel> objs) {
			GameObjects.AddRange(objs);
		}
        public bool SingleCollision(int i)
        {
            IShape me = obj.VirtualShape;
            Type t = typeof(Collision);

            Circle crc = new Circle(circle.radius * 1.1f, circle.center);
            //Console.WriteLine(Thread.CurrentThread.Name);
            if (!GameObjects[i].Equals(obj))
            { // ez a kocsog beleugrik. gamert gamerral vizsgálja. előző lépést a mostanival. DE AKKOR H H CSAK NÉHA ROSSZ? meg a vonalakkal is jóba van?
                t.InvokeMember("Intersect", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, new OverloadBinder(), null, new object[2] {
						crc,
						GameObjects[i].VirtualShape
					});
                if (Collision.IsCollision)
                {
                    //                        Console.WriteLine("-----------MAJDNEM Ütközött----------" + rnd.Next());
                    obj.Physical.CollisionMode = CollisionState.NearlyCollision;
                    if (!obj.Physical.Collosed.Contains(GameObjects[i].VirtualShape))
                        obj.Physical.Collosed.Add(GameObjects[i].VirtualShape);
                    t.InvokeMember("Intersect", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, new OverloadBinder(), null, new object[2] {
							me,
							GameObjects[i].VirtualShape
						});
                    if (Collision.IsCollision)
                    {
                        //                            Console.WriteLine("-----------Ütközött----------" + rnd.Next());
                        obj.Physical.CollisionMode = CollisionState.Collosed;
                        obj.Physical.Collosed.Clear();
                        obj.Physical.Collosed.Add(GameObjects[i].VirtualShape);
                        obj.Physical.Direction = Vector2.Zero;
                        return true; // minél hamarabb ütközik annál hamarabb befejezhetjük a vizsgálódást.
                    }
                    obj.Physical.CollisionMode = CollisionState.NearlyCollision;
                    obj.Physical.Collosed.Clear();
                    obj.Physical.Collosed.Add(GameObjects[i].VirtualShape);
                }
                if (obj.Physical.Collosed.Contains(GameObjects[i].VirtualShape))
                {
                    //obj.Physical.Collosed.Remove(GameObjects[i].VirtualShape); // itt kibszott egy exceptiont: System.IndexOutOfRangeException: index < lower bound
                    obj.LineLock = false; // ezért kikomcsiztam
                    if (obj.Physical.Collosed.Count == 0)
                    {
                        obj.Physical.CollisionMode = CollisionState.NotCollision;
                        //                            Console.WriteLine("-----------NEM Ütközött----------" + rnd.Next());
                    }
                }
            } else
            {

            }
            obj.Physical.CollisionMode = CollisionState.NotCollision;
            obj.Physical.Collosed.Clear();
            return false;
        }
        public bool SingleCollision(StaticModel sm)
        {
			lock (this) {
				IShape me = obj.VirtualShape;
				Type t = typeof(Collision);

				Circle crc = new Circle(circle.radius * 1.1f, circle.center);
				//Console.WriteLine(Thread.CurrentThread.Name);
				// ez a kocsog beleugrik. gamert gamerral vizsgálja. előző lépést a mostanival. DE AKKOR H H CSAK NÉHA ROSSZ? meg a vonalakkal is jóba van?
				t.InvokeMember("Intersect", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, new OverloadBinder(), null, new object[2] {
					crc,
					sm.VirtualShape
				});
				if (Collision.IsCollision) {
					obj.Physical.CollisionMode = CollisionState.NearlyCollision;
					if (!obj.Physical.Collosed.Contains(sm.VirtualShape))
						obj.Physical.Collosed.Add(sm.VirtualShape);
					t.InvokeMember("Intersect", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, new OverloadBinder(), null, new object[2] {
						me,
						sm.VirtualShape
					});
					if (Collision.IsCollision) {
						Console.WriteLine("-----------Ütközött----------" + rnd.Next() + " " + ((Circle)obj.Shape).radius.ToString());
						obj.Physical.CollisionMode = CollisionState.Collosed;
						obj.Physical.Collosed.Clear();
						obj.Physical.Collosed.Add(sm.VirtualShape);
						obj.Physical.Direction = Vector2.Zero;
						return true; // minél hamarabb ütközik annál hamarabb befejezhetjük a vizsgálódást.
					}
					Console.WriteLine("-----------MAJDNEM Ütközött----------" + rnd.Next() + " " + ((Circle)obj.Shape).radius.ToString());
					obj.Physical.CollisionMode = CollisionState.NearlyCollision;
					obj.Physical.Collosed.Clear();
					obj.Physical.Collosed.Add(sm.VirtualShape);
				}
				if (obj.Physical.Collosed.Contains(sm.VirtualShape)) {
					//obj.Physical.Collosed.Remove(sm.VirtualShape); // itt kibszott egy exceptiont: System.IndexOutOfRangeException: index < lower bound
					obj.LineLock = false; // ezért kikomcsiztam
					if (obj.Physical.Collosed.Count == 0) {
						obj.Physical.CollisionMode = CollisionState.NotCollision;
						Console.WriteLine("-----------NEM Ütközött----------" + rnd.Next());
					}
				}
				obj.Physical.CollisionMode = CollisionState.NotCollision;
				obj.Physical.Collosed.Clear();
				return false;
			}
        }
		public void AllLineSplit(float length) {
			Transform t = new Transform();
			Frame f = new Frame(0f);
			PhysicalFeatures pf = new PhysicalFeatures(.001f, new Vector2(0,0), 1);
			IShape nwLine = null;
			StaticModel linePart = null;
			List<StaticModel> lineList = new List<StaticModel>();

			for (int i = 0; i < GameObjects.Count; i++) {
				Line line = GameObjects[i].VirtualShape as Line;
				if (line != null) {
					float lineLength = Vector2.Distance(line.Direction);
					IShape[] lns = new Line[(int)(lineLength / length)];
					if (lns.Length > 1) {
						Remove(GameObjects[i]);
						i--;
						for (int j = 0; j < lns.Length; j++) {
							Vector2 dir = Vector2.Normailze(line.Direction);
							lns[j] = new Line(new Vector2(line.Start.X + (length * dir.X) * j, line.Start.Y + (length * dir.Y) * j), line.Direction, length);

							t = new Transform();
							f = new Frame(0f);
							pf = new PhysicalFeatures(.001f, new Vector2(0, 0), 1);
							nwLine = lns[j];
							linePart = new StaticModel(ref f, ref nwLine, ref t, pf);
							lineList.Add(linePart);
						}
						t = new Transform();
						f = new Frame(0f);
						pf = new PhysicalFeatures(.001f, new Vector2(0, 0), 1);
						nwLine = new Line(((Line)lns[lns.Length - 1]).End, line.End);
						linePart = new StaticModel(ref f, ref nwLine, ref t, pf);
						lineList.Add(linePart);
					}
				}
			}
			for (int i = 0; i < lineList.Count; i++) {
				Add(lineList[i]);
			}
		}
		public void LoadMapFromFile(string path) {
			StreamReader reader = new StreamReader(path);
            List<Vector2> points = new List<Vector2>();
			Frame f = new Frame(0f);
			Transform t = new Transform();
			IShape l;
			Circle c;
			PhysicalFeatures pf = default(PhysicalFeatures);
			pf.Mode = PhysicMode.Off;
            int fr = 0, to = 0, i = 0;

			while (!reader.EndOfStream) {
                Vector2 a = Vector2.Zero, b = Vector2.Zero;
				string str = reader.ReadLine();
				string[] ds = str.Replace(" ", "").Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries); // l32,34,76,43
                if (ds[0][0].Equals('['))
                {
                    ds[0] = ds[0].Replace("[", "");
                    ds[1] = ds[1].Replace("]", "");
                    fr = int.Parse(ds[0]);
                    to = int.Parse(ds[1]);
                }
                else
                {
                    a = new Vector2(float.Parse(ds[0]), float.Parse(ds[1]));
                    points.Add(a);
                    if (points.Count > 1)
                    {
                        if (i >= fr && i <= to)
                        {
                            l = new Line(points[i], points[++i]);
                            Add(new StaticModel(ref f, ref l, ref t, pf));
                        }
                        else
                            i++;
                    }
                }
                

                //str = reader.ReadLine();
                //ds = str.Replace(" ", "").Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries); // l32,34,76,43
                //ds[0] = ds[0].Replace("{X=", "");
                //ds[1] = ds[1].Replace("Y=", "");
                //ds[1] = ds[1].Replace("}", "");
                //b = new Vector2(float.Parse(ds[0]), float.Parse(ds[1]));
                //l = new Line(a, b);
                //Add(new StaticModel(ref f, ref l, ref t, pf));
				if (str[0].Equals('l')) {
//					l = new Line(new Vector2(ds[0], ds[1]), new Vector2(ds[2], ds[3]));
//					aw.Add(new StaticModel(ref f, ref l, ref t, pf));
				} else if (str[0].Equals('c')) {
//					c = new Circle(ds[0], new Vector2(ds[1], ds[2]));
//					aw.Add(new StaticModel(ref f, ref c, ref t, pf));
				}
			}
			reader.Close();

            //int i = 0;
            //while (i < points.Count - 1)
            //{
            //    l = new Line(points[i], points[++i]);
            //    Add(new StaticModel(ref f, ref l, ref t, pf));
            //}
		}

        public string SaveMap(string path)
        {
			return "";
		}
		public void LoadMap(Bitmap bmp) {
			if (bmp != null) {
				List<Vector2> map = new List<Vector2>();
				Color c = Color.Black;
				int n = 4;
				int nx = 0, ny = 0, cellNum = 0, maxCellNum = 255 * 3 * n * n;
				int x = 0, y = 0;
				bool isw = nx + x < bmp.Width, ish = ny + y < bmp.Height;

				while (isw && ish) {
					for (x = 0; x < n; x++) {
						for (y = 0; y < n; y++) {
							isw = nx + x < bmp.Width;
							ish = ny + y < bmp.Height;
							if (isw && ish)
								c = bmp.GetPixel(nx + x, ny + y);
							else if (!isw && ish)
								c = bmp.GetPixel(x, ny + y);
							else if (isw && !ish)
								c = bmp.GetPixel(nx + x, y);
							else if (!isw && !ish)
								c = bmp.GetPixel(x, y);

							cellNum += c.R + c.G + c.B;
						}
					}
					if (cellNum < maxCellNum / 1.1f)
						map.Add(new Vector2(nx, ny));
					cellNum = 0;
					nx += n;
					if (nx >= bmp.Width) {
						nx = 0;
						ny += n;
					}
					//					Console.WriteLine(nx);
					//					Console.WriteLine(ny);
				}
				Console.WriteLine(map.Count);

				Bitmap test = new Bitmap(256, 256);
				Graphics g = Graphics.FromImage(test);
				Vector2 lastLink = Vector2.Zero;
				Vector2 str = Vector2.Zero, end = Vector2.Zero;

				while (map.Count > 1) {
					Color rc = Color.FromArgb(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256));
					Console.WriteLine(map.Count);
					lastLink = map[0];
					Console.WriteLine("new last: {0}",lastLink);
					Vector2 dir = map[1] - map[0];
					str = map[0];

					Line line = new Line(map[0], Vector2.Radian(dir));
					Console.WriteLine(map[0]);
					//					test.SetPixel((int)map[0].X, (int)map[0].Y, Color.Red);
					//					g.DrawLine(new Pen(Brushes.Black), (Point)line.Start, (Point)line.End);
					//test.Save(@"test.jpg");
					//					Console.WriteLine("map[1]: {0};     map[0]: {1}", map[1], map[0]);

					for (int i = 1; i < map.Count; i++) {
						//						test.SetPixel((int)map[i].X, (int)map[i].Y, Color.Red);
						dir = map[i] - lastLink;
						//						dir.X = (float)Math.Abs(dir.X);
						//						dir.Y = (float)Math.Abs(dir.Y);
						Console.WriteLine("map[i]: {0}, last: {1}, dir: {2}", map[i],  lastLink, dir);
						if (Vector2.Distance(dir) <= n * 2) {
							Console.WriteLine(map[0]);
							if (Collision.Intersect(line, map[i], n)) {
								//								Console.WriteLine("map[i]: {0};     last: {1}", map[i], lastLink);
								Console.WriteLine("(remove: {0})", map[i]);
								lastLink = map[i];
								end = map[i];
								//g.DrawLine(new Pen(Brushes.Black), (Point)str, (Point)end);

								//								test.SetPixel((int)map[i+1].X, (int)map[i+1].Y, rc);
								test.SetPixel((int)map[i].X, (int)map[i].Y, rc);
								test.Save(@"test.jpg");
								//								Thread.Sleep(1000);
								map.RemoveAt(i);
								i--;
								//map.RemoveAt(i);
							}
						} else {
							for (int j = i + 1; j < map.Count; j++) {
								dir = map[j] - lastLink;
								if (Vector2.Distance(dir) <= n * 2) {
									i = --j;
									break;
								}
							}
						}
					}
					map.RemoveAt(0);
				}
				test.SetPixel((int)end.X, (int)end.Y, Color.Red);
				Console.WriteLine(map.Count);
				test.Save(@"test.jpg");
			}

			//			FileStream fs = null;
			//			try {
			//				fs = new FileStream(path, FileMode.Open);
			//				GameObjects = (List<StaticObject>)serializer.Deserialize(fs);
			//				return "";
			//			}
			//			catch (Exception e) {
			//				return e.Message;
			//			}
			//			finally {
			//				fs.Close();
			//			}
		}
		//		void loadPixels(Bitmap bmp) {
		//			List<Point> line = new List<Point>(); 
		//			for (int x = 0; x < bmp.Width; x++) {
		//				for (int y = 0; y < bmp.Height; y++) {
		//					Color c = bmp.GetPixel(x, y);
		//					if (c.R < 200 && c.G < 200 && c.B < 200) {
		//						blacks.Add(new ColorPosition() {
		//							c = c,
		//							p = new Point(x, y)
		//						});
		//					}
		//				}
		//			} for (int i = 0; i < blacks.Count; i++) {
		//				Point dir = Point.Empty;
		//				for (int j = 0; j < blacks.Count - 1; j++) {
		//					Point pp = blacks[j].p.Sub(blacks[j + 1].p);
		//					pp.X = (int)Math.Abs(pp.X);
		//					pp.Y = (int)Math.Abs(pp.Y);
		//					if (j == 0)
		//						dir = pp;
		//					if ((pp.X == 1 && pp.Y == 0) || (pp.X == 0 && pp.Y == 1) || (pp.X == 1 && pp.Y == 1)) {
		//						if (pp == dir) {
		//							line.Add(blacks[j].p);
		//							blacks.RemoveAt(j);
		//							if (j == 127)
		//								Console.WriteLine();
		//						} else
		//							Console.WriteLine("end");
		//					}
		//				}
		//			} 
		//		}
	}
}

