using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace glControlKit
{
	public class Slider : Control
	{
		Bitmap thumbBmp, backBmp;
		Texture2D thumbTexture, backTexture;
		PointF slidePoints;
		float thumbWidth = 15;

		public event EventHandler ValueChanged;
		public float ThumbWidth {
			get { return thumbWidth; }
			set { 
				thumbWidth = value;
				Redraw();
			}
		}
		public float Maximum { get; set; }
		public float Minimum { get; set; }

		public Slider() : base()
		{
			InitControl();
			ControlGraphics = new Spritebatch();
			ControlGraphics.ColorBrush = Color.Black;
			Width = thumbWidth + 100;
			Height = 26;
			Redraw();
			Minimum = 0f;
			Maximum = 100f;
		}
		public Slider(Control parent)
		{
			base.InitControl(parent);
			ControlGraphics = new Spritebatch();
			ControlGraphics.ColorBrush = Color.Black;
			Width = thumbWidth + 100;
			Height = 26;
			Redraw();
			Minimum = 0f;
			Maximum = 100f;
		}
		void DrawBack() {
			backBmp = new Bitmap((int)(Width - thumbWidth - 10f), (int)Height);
			backBmp.MakeTransparent();
			Graphics g;
			Pen p = new Pen(Brushes.Gray, 1f);
			g = Graphics.FromImage(backBmp);
			g.DrawLine(p, new PointF(0, Height / 2 - p.Width - 5), new PointF(Width, Height / 2 - p.Width - 5));
			p.Color = Color.LightGray;
			g.DrawLine(p, new PointF(0, Height / 2 - 5), new PointF(Width, Height / 2 - 5));
			p.Color = Color.Black;
			g.DrawLine(p, new PointF(0, Height / 2 - p.Width - 5), new PointF(Width, Height / 2 - p.Width - 5));
			backTexture = ContentPipe.CreateTexture2D(backBmp);
		}
		void DrawThumb() {
			thumbBmp = new Bitmap((int)thumbWidth, (int)Height);
			Graphics g;
			g = Graphics.FromImage(thumbBmp);//SystemBrushes.Control
			g.FillRectangle(Brushes.LightGray, new RectangleF(1, 1, thumbWidth, Height - Height / 2));
			g.FillPolygon(Brushes.LightGray, new PointF[] {
				new PointF(0, Height - Height / 2),
				new PointF(thumbWidth, Height - Height / 2),
				new PointF(thumbWidth / 2 + 1, Height - 10)
			});
			Pen p = new Pen(Brushes.White, 1.5f);
			g.DrawLine(p, new PointF(0, 0), new PointF(thumbWidth, 0));
			g.DrawLine(p, new PointF(0, 0), new PointF(0, Height - Height / 2));
			g.DrawLine(p, new PointF(0, Height - Height / 2), new PointF(thumbWidth / 2, Height - 10));
			p.Color = Color.Black;
			p.Width = 0.5f;
			g.DrawLine(p, new PointF(thumbWidth - 1, 1), new PointF(thumbWidth - 1, Height - Height / 2 - 1));
			g.DrawLine(p, new PointF(thumbWidth - 1, Height - Height / 2), new PointF(thumbWidth / 2 + 1, Height - 10));
			thumbTexture = ContentPipe.CreateTexture2D(thumbBmp);
		}

		float currValue;
		public float Value {
			get { 
				return currValue; 
			}
			set {
				currValue = value;
				Redraw();
				if (ValueChanged != null)
					ValueChanged(this, EventArgs.Empty);
			}
		}

		void Redraw() {
			DrawBack();
			DrawThumb();
//			Invalidate();
		}

		public override void SetInputSource(GameWindow gw) {
			EnableInput = true;
			gw.MouseMove += OnMouseMove;
			gw.MouseDown += OnMouseDown;
			gw.MouseUp += OnMouseUp;
		}

		public void SetSlidePoints(float y) {
			for (int x = 0; x < Width; x++) {
				slidePoints = new PointF(x, y);
			}
		}

		bool isDrag;
		protected virtual void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			Point location = e.Position;
			if (location.X - thumbWidth >= Value / Maximum * backBmp.Width - thumbWidth / 2 && location.X - thumbWidth <= Value / Maximum * backBmp.Width + thumbWidth / 2) {
				isDrag = true;
			}
		}
		protected virtual void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			isDrag = false;
		}
		protected virtual void OnMouseMove(object sender, MouseMoveEventArgs e)
		{
			Point location = e.Position;
			if (isDrag && ((float)location.X - thumbWidth) / (float)backBmp.Width * (float)Maximum <= Maximum && ((float)location.X - thumbWidth) / (float)backBmp.Width * (float)Maximum >= Minimum) {
				Value = ((float)e.X - thumbWidth) / (float)backBmp.Width * (float)Maximum;
			}
		}
		public virtual void OnPaint()
		{
			float f = Value / Maximum * backBmp.Width - thumbWidth / 2;
//			g.ResetScreen(RootControl.Width, RootControl.Height);
			ControlGraphics.ColorBrush = Color.White;
			ControlGraphics.DrawTexture(backTexture, new Vector2(Location.X + thumbWidth, Location.Y + 0f), new Vector2(1,1), Vector2.Zero);
			ControlGraphics.DrawTexture(thumbTexture, new Vector2(Location.X + thumbWidth + f, Location.Y + 0f), new Vector2(1,1), Vector2.Zero);
			//g.DrawString(Value.ToString(), new Font(FontFamily.Families[0], 10f), Brushes.Black, new PointF(f + thumbWidth, Height - 10));
		}
	}
}

