using System;
using System.Collections.Generic;
using Geometry.Shapes;
using Entities;
using Worker;

namespace InternalSection {
	/// <summary>
	/// Localizator reference. Ez csak van. Aki nem internal az ne nyúlkáljon ebbe bele.
	/// Mert ha akar valamit akkor majd azt módosítja amire ezek a refek mutatnak.
	/// </summary>
	public class AnimModel : StaticModel
	{
		static Random rnd = new Random(); // nah ilyen se volt még a base()-be felhasználhatok statikus tagokat
		
		public AnimModel(ref Frame f, ref IShape shape, ref Transform t, PhysicalFeatures pf)  : base("Player" + rnd.Next().ToString())
		{
			this.Frame = f;
			this.Shape = shape;
			this.VirtualShape = (IShape)shape.Clone();
			this.Transformation = t;
			this.Physical = pf;
//			DeniedList = new List<IShape>();
			LastPhysicsTime = DateTime.Now;
			Transformation.Translate = Shape.BoundingCircle.center;
			this.Frame.Translate = Transformation.Translate;
			MoveDirection = Turn.None;
			if (Frame.SourceRectangle != null) {
				DrawPoints = new Vector2[] {
					new Vector2(Frame.Translate.X, Frame.Translate.Y),   // destination for upper-left point of original 
					new Vector2(Frame.Translate.X + Frame.SourceRectangle.X, Frame.Translate.Y - Frame.SourceRectangle.Y),  // destination for upper-right point of  
					new Vector2(Frame.Translate.X, Frame.Translate.Y + Frame.SourceRectangle.Y) // destination for lower-left point of original
				};
			}
			else if (Frame.Bitmap != null) {
				DrawPoints = new Vector2[] {
					new Vector2(Frame.Translate.X, Frame.Translate.Y),   // destination for upper-left point of original 
					new Vector2(Frame.Translate.X + Frame.Bitmap.Width, Frame.Translate.Y - Frame.Bitmap.Height),  // destination for upper-right point of  
					new Vector2(Frame.Translate.X, Frame.Bitmap.Width + Frame.Bitmap.Height) // destination for lower-left point of original
				};
			} // else draw simetric geometry
		}
	}
}


