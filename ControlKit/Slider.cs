using System;
using System.Drawing;
using System.Windows.Forms;

namespace ControlKit
{
	public class Slider : Control
	{
		Bitmap thumbBmp, backBmp;
		PointF slidePoints;
		int thumbWidth = 15;

		public event EventHandler ValueChanged;
		public int ThumbWidth {
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
			ResizeRedraw = true;
			DoubleBuffered = true;
			Width = thumbWidth + 100;
			Height = 26;
			Redraw();
			Minimum = 0f;
			Maximum = 100f;
		}

		void DrawBack() {
			backBmp = new Bitmap(Width - thumbWidth - 10, Height);
			Graphics g;
			Pen p = new Pen(Brushes.Gray, 1f);
			g = Graphics.FromImage(backBmp);
			g.DrawLine(p, new PointF(0, Height / 2 - p.Width - 5), new PointF(Width, Height / 2 - p.Width - 5));
			p.Color = Color.LightGray;
			g.DrawLine(p, new PointF(0, Height / 2 - 5), new PointF(Width, Height / 2 - 5));
			p.Color = Color.Black;
			g.DrawLine(p, new PointF(0, Height / 2 - p.Width - 5), new PointF(Width, Height / 2 - p.Width - 5));
		}
		void DrawThumb() {
			thumbBmp = new Bitmap(thumbWidth, Height);
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

		public new Size Size {
			get {
				return base.Size;
			}
			set {
				base.Size = value;
				Redraw();
			}
		}

		void Redraw() {
			DrawBack();
			DrawThumb();
			Invalidate();
		}

		public void SetSlidePoints(float y) {
			for (int x = 0; x < Width; x++) {
				slidePoints = new PointF(x, y);
			}
		}

		bool isDrag;
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Location.X - thumbWidth >= Value / Maximum * backBmp.Width - thumbWidth / 2 && e.Location.X - thumbWidth <= Value / Maximum * backBmp.Width + thumbWidth / 2) {
				isDrag = true;
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			isDrag = false;
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (isDrag && ((float)e.Location.X - thumbWidth) / (float)backBmp.Width * (float)Maximum <= Maximum && ((float)e.Location.X - thumbWidth) / (float)backBmp.Width * (float)Maximum >= Minimum) {
				Value = ((float)e.X - thumbWidth) / (float)backBmp.Width * (float)Maximum;
			}
		}
		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			base.OnPaintBackground(pevent);

			float f = Value / Maximum * backBmp.Width - thumbWidth / 2;
			Graphics g = pevent.Graphics;
			g.DrawImage(backBmp, new PointF(thumbWidth, 0f));
			g.DrawImage(thumbBmp, new PointF(thumbWidth + f, 0f));
			g.DrawString(Value.ToString(), new Font(FontFamily.Families[0], 10f), Brushes.Black, new PointF(f + thumbWidth, Height - 10));
		}



	}
}

