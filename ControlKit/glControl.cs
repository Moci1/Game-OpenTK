using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using OpenTK;

namespace glControlKit
{
	public enum CoordSystem { Screen, World }
	public abstract class Control
	{
		public float Width { get; set; }
		public float Height { get; set; }
		public RectangleF Margin { get; set; }
		public RectangleF Padding { get; set; }
		public string Text { get; set; }
		public List<Control> Content { get; set; }
		public Color ColorBlend { get; set; }
		public RectangleF Size { get; set; }
		public bool Visible { get; set; }
		public bool Enable { get; set; }
		public bool EnableInput { get; protected set; }
		public Spritebatch ControlGraphics;

		Control rootControl, parent;
		public Control RootControl {
			get { return rootControl; }
			set {
				rootControl = value;
				if (parent == null)
					parent = rootControl;
			}
		}
		public Control Parent { 
			get { return parent; }
			set {
				if (rootControl == null)
					rootControl = value;
				parent = value;
			}
		}

		CoordSystem locateTo;
		PointF location;
		RectangleF boundRect;
		public RectangleF BoundRect {
			get { return boundRect; }
			set { boundRect = value; }
		}
		public CoordSystem LocateTo { 
			get {
				return locateTo;
			}
			set {
				if (value == CoordSystem.Screen && locateTo != CoordSystem.Screen) {
					Location.X -= Parent.Width / 2f;
					Location.Y -= Parent.Height / 2f;
				} else if (value == CoordSystem.World && locateTo != CoordSystem.World) {
					Location.X += Parent.Width / 2f;
					Location.Y += Parent.Height / 2f;
				}
				locateTo = value;
			}
		}
		public PointF Location {
			get { return location; }
			set {
				location = value; 
				boundRect.X = location.X;
				boundRect.Y = location.Y;
			}
		}
		void InitLocateTo(CoordSystem cs) {
			if (cs == CoordSystem.Screen) {
				location.X -= Parent.Width / 2f;
				boundRect.X -= location.X;
				location.Y -= Parent.Height / 2f;
				boundRect.Y -= location.Y;
			} else if (cs == CoordSystem.World) {
				location.X += Parent.Width / 2f;
				boundRect.X += location.X;
				location.Y += Parent.Height / 2f;
				boundRect.Y += location.Y;
			}
			locateTo = cs;
		}
		public abstract void SetInputSource(GameWindow gw);

		public void InitControl(Control parent)
		{
			Parent = parent;
			Enable = true;
			InitLocateTo(CoordSystem.Screen);
			ControlGraphics = new Spritebatch();
		}
		public void InitControl()
		{
			parent = this;
			Enable = true;
			InitLocateTo(CoordSystem.Screen);
			ControlGraphics = new Spritebatch();
		}
	}
}

