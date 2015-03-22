using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace glControlKit
{
	// van 2 texturája (on, off) inputtal 
	public class Button : Control
	{
		public Bitmap onBmp, offBmp;
		Texture2D onTexture, offTexture;
		Color[] onColors, offColors;
		public bool Clicked;

		public Button(Bitmap offBmp, Bitmap onBmp)
		{
			this.offBmp = offBmp;
			this.onBmp = onBmp;
			InitControl();
			onTexture = ContentPipe.CreateTexture2D(onBmp, out onColors);
			offTexture = ContentPipe.CreateTexture2D(offBmp, out offColors);
			BoundRect = new Rectangle(0, 0, offBmp.Width, offBmp.Height);
		}
		public Button(Control parent, Bitmap offBmp, Bitmap onBmp)
		{
			this.offBmp = offBmp;
			this.onBmp = onBmp;
			InitControl(parent);
			onTexture = ContentPipe.CreateTexture2D(onBmp, out onColors);
			offTexture = ContentPipe.CreateTexture2D(offBmp, out offColors);
			BoundRect = new Rectangle(0, 0, offBmp.Width, offBmp.Height);
		}

		public override void SetInputSource(GameWindow gw)
		{
			EnableInput = true;
			gw.MouseMove += HandleMouseMove; // benn van a button rectjébe? ha igen akkor pixel van az egér helyén?
			gw.MouseDown += HandleMouseDown;
			gw.MouseUp += HandleMouseUp;
		}

		protected virtual void HandleMouseUp (object sender, MouseButtonEventArgs e)
		{
			
		}

		protected virtual void HandleMouseDown (object sender, MouseButtonEventArgs e)
		{
			Point loc = e.Position;
			if (loc.X > BoundRect.X && loc.X < BoundRect.X + BoundRect.Width) {
				if (loc.Y > BoundRect.Y && loc.Y < BoundRect.Y + BoundRect.Height) {
					Point p = new Point((int)(loc.X - BoundRect.X), (int)(loc.Y - BoundRect.Y));
					if (offBmp.GetPixel(p.X, p.Y) != Color.Transparent) {
						Clicked = true;
						return;
					}
				}
			}
			Clicked = false;
		}

		protected virtual void HandleMouseMove (object sender, MouseMoveEventArgs e)
		{
			
		}
		public void OnPaint() {
			ControlGraphics.ColorBrush = Color.White;
			if (Clicked)
				ControlGraphics.DrawTexture(onTexture, new Vector2(Location.X + BoundRect.X, Location.Y + BoundRect.Y), new Vector2(1, 1), Vector2.Zero);
			else
				ControlGraphics.DrawTexture(offTexture, new Vector2(Location.X + BoundRect.X, Location.Y + BoundRect.Y), new Vector2(1, 1), Vector2.Zero);
		}
	}
}

