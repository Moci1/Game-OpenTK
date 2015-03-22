using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ControlKit;
using InternalSection;

namespace DeadDown
{
	public static class Ext {
		public static bool IsFineInvalidate(this Control c) {
			return true;
		}
	}
    public partial class Form1 : Form
    {
		BitmapRenderer br;
		Point mousePoint;

        public Form1()
        {
            InitializeComponent();
			DoubleBuffered = true;
			br = new BitmapRenderer(this, Width,Height);
			MouseMove += br.OnMouseMove;
			KeyDown += br.OnKeyDown;
			KeyUp += br.OnKeyUp;
			Resize += br.HandleResize;
			br.OnPaint(this, new PaintEventArgs(CreateGraphics(), ClientRectangle));
			Invalidate();
			br.NeedInvalidation += delegate(object sender, EventArgs e) {
				if (br.NeedInvalidate && isInvalitated) {
					isInvalitated = false;
					Invalidate(br.InvalidRect);
				}
				else {
					// ha ide belefut akkor még tart az invalidálás és akkor ennek az objektnek nem fog invalidálni
					// ennek a br.InvalidRect-nek mert a köv eventnél megváltozik.
					// igaz következő körbe meg lehet megint belefut ide.
					// de a lényeg h ez csak nagy terhelésnél jöhet szóba.
				}
			};
			System.Windows.Forms.Timer drawThr = new System.Windows.Forms.Timer();
			drawThr.Tick += delegate(object sender, EventArgs e) {
//				if ((DateTime.Now - lastPaint).TotalMilliseconds > 200) {
//					br.OnPaint(this, new PaintEventArgs(CreateGraphics(), ClientRectangle));
				if (br.NeedInvalidate && isInvalitated)	{
					isInvalitated = false;
					Invalidate(br.InvalidRect);
				}
//				}
			};
			drawThr.Interval = 5;
			drawThr.Stop();
//			drawThr.Start();

//            gameLayer1.Init();


			Button button = new Button();
			button.Size = new Size(70,21);
			button.Location = new Point(100, 20);
			button.Text = "Reset";
			button.Click += HandleButtonClick;
//			this.Controls.Add(button);
        }
		protected override void OnResize(EventArgs e)
		{
			if (br != null) {
				br.Width = Width;
				br.Height = Height;
				Invalidate();
			}
		}
		bool isInvalitated;
        protected override void OnInvalidated(InvalidateEventArgs e)
		{
			base.OnInvalidated(e);
			isInvalitated = true;
			br.NeedInvalidate = false;
		}
		void HandleButtonClick (object sender, EventArgs e)
		{
			br = new BitmapRenderer(this, Width, Height);
			Focus();
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			mousePoint = e.Location;
		}
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			Graphics g = e.Graphics;
			
//			g.DrawString(totalMsAw.ToString(), new Font(FontFamily.Families[80], 10f), Brushes.Green, new PointF(0, 40));
//			g.DrawString(MousePos.ToString(), new Font(FontFamily.Families[80], 10f), Brushes.Green, new PointF(0, 80));
			br.OnPaint(this, new PaintEventArgs(CreateGraphics(), ClientRectangle));
			g.DrawImage(br.bmp, Point.Empty);
            g.DrawString(br.Fps.ToString(), new Font(FontFamily.Families[80], 19f), Brushes.Green, new PointF());
			g.DrawString(mousePoint.ToString(), new Font(FontFamily.Families[80], 10f), Brushes.Green, new PointF(0, 80));
            Console.WriteLine(br.Fps);
		}


		void HandleValueChanged(object sender, System.EventArgs e)
		{
//			gameLayer1.Enemy.AsdValue = ((Slider)sender).Value;
//			gameLayer1.Gamer.AsdValue = ((Slider)sender).Value;
		}

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            //gameLayer1.Gamer.AsdValue = trackBar1.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (trackBar1.Enabled)
            //    trackBar1.Enabled = false;
            //else
            //    trackBar1.Enabled = true;
            gameLayer1.Focus();
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
//            gameLayer1.Enemy.AsdValue = (float)numericUpDown1.Value;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
//            gameLayer1.Enemy.AsdValue = (float)numericUpDown1.Value;
//            gameLayer1.Gamer.AsdValue = (float)numericUpDown1.Value;
//            gameLayer1.Enemy.BindFollow(gameLayer1.Gamer, .2f);
        }
    }
}
