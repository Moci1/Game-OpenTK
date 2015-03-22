using System;
using Entities;
using Geometry.Shapes;
using System.Xml;
using System.Xml.Serialization;
using System.Drawing;
using System.Threading;

namespace InternalSection {

	public enum Turn { None, Left, Right }

	/// <summary>
	/// Nem hat rá gravitáció és csak transzformációkkal lehet mozgatni, de ütközésvizsgálat az van.
	/// </summary>
	public class StaticModel
	{
		public bool isEndMove;
		public Frame Frame { get; internal set; } // Leírja: Bitmap, Transform (ahol most van)
		public IShape Shape { get; internal set; }
		public IShape VirtualShape { get; protected set; }
		public Transform Transformation { get; internal set; } // ahova és ahogy megy (translate, rotate, scale..)
		public PhysicalFeatures Physical;
		public Turn MoveDirection { get; internal set; }
		public Vector2[] DrawPoints { get; protected set; }
		public Rectangle InvalidRectangle { get { return Shape.BoundingRectangle; } }
		public static event TransformHandler TransformEvent;
		public static event EventHandler CollisionEvent; // TODO: PhysicUpdate deklaráció
		public ArgsDelegate UpdateCommands; // Behaivor vagy mi Viselkedés cím jobban illene rá
		public DateTime LastPhysicsTime; // és azért jó me kapcsba áll mindenfelé. AInak jó lesz.
		public int AreaId { get; internal set; }
        public int ObjIndex { get; internal set; } // ha törlődik az obj a listábol minden egyel feljebb csuszik!->utána mindenkinek megváltozik az indexe
		internal bool LineLock;

        
		[XmlAttribute]
		public String Name { get; private set; }

		public StaticModel() {
			Name = "Empty";
		}
		public StaticModel(String name) {
			Name = name;
		}
		public StaticModel(ref Frame f, ref IShape shape, ref Transform t, PhysicalFeatures pf) {
			this.Frame = f;
			this.Shape = shape;
			this.VirtualShape = (IShape)shape.Clone();
			this.Transformation = t;
			this.Physical = pf;
			Transformation.Translate = Shape.BoundingCircle.center;
			this.Frame.Translate = Transformation.Translate;
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

		/// <summary>
		/// Csak a példánnyal egy dll-ben lévő p
		/// </summary>
		public bool Move(float x, float y) { // hova, hogyan, 
			// út meghatározása (globális - kis akadájokat nem figyel), út megtételének módja-
			// transzformációk folyamatos adagolása (mivel itt van a Frame meg a Circle)

			// while (...) { amíg oda nem ért...
			isEndMove = false;
			Transform t = new Transform();
			if (Transformation.Translation(x, y)) { // ha tényleg változott a helye
				t.Translate = new Vector2(x, y); // 930;107
				VirtualShape.SetTransform(t);
			}
			else {
				return false;
			}
			bool result = false;
			bool isTransformed = false;
			if (TransformEvent != null) { // CollManager figyeli ezt. És az AreaWorker-rel megnézni,
				isTransformed = TransformEvent(this, EventArgs.Empty); // hogy mivel ütközik
				if (Physical.CollisionMode == CollisionState.Collosed) { // ha ütközünk akkor: -megállunk, -way irányba eldzsalunk
					//Shape.SetTransform(Transformation);
					result = false;
					if (CollisionEvent != null)
						CollisionEvent(this, EventArgs.Empty);
				}
				else { // speedet is ki kéne számolni és akkor a velocity is csak direction lehetne
					Shape.SetTransform(t);
					Vector2 velocity = (Shape.BoundingCircle.center - Transformation.Translate); // egyenes esetén azé nem esik le mert a bounding circle ugyanazt adja mint a translate
					Physical.Speed = Vector2.Distance(velocity);
					Physical.Direction = Vector2.Normailze(velocity);
					Transformation.Translate = Shape.BoundingCircle.center;
					Frame.Translate = Transformation.Translate;
					result = true;
				    DrawPoints = Frame.TurnAround(MoveDirection);
				}
				// ha ütközik tehát false a Transform akkor circle-t vissza kell állítani.
			}
			isEndMove = true;
			return result;
		}
	}
}

