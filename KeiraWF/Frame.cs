using System;
using InternalSection;
using System.Drawing;

namespace Entities
{
	public enum DrawMode { Geometry, Bitmap }

	public class Frame : IComparable<Frame>
	{
		public Bitmap Bitmap { get; internal set; }
		public Vector2 Scale { get; internal set; }
		public float Rotate { get; internal set; }
		public Vector2 Translate { get; internal set; }
		public DrawMode Drawed { get; set; }
		public Rectangle SourceRectangle { get; internal set; }

		public Frame(float r) {
			Rotate = r;
		}
		public Frame(Bitmap bmp, Rectangle source) {
			Bitmap = bmp;
			if (bmp != null)
				SourceRectangle = source;
			else
				throw new Exception("A 'bmp' nem lehet null ha a 'source' definiálva van.");
		}
		public Frame(Bitmap bmp, Rectangle source, DrawMode drawed) {
			Drawed = drawed;
			Bitmap = bmp;
			if (bmp != null)
				SourceRectangle = source;
			else
				throw new Exception("A 'bmp' nem lehet null ha a 'source' definiálva van.");
		}

		public Vector2[] TurnAround(Turn m) {
			if (m == Turn.Left) {
				Vector2[] destinationPoints = {
					new Vector2(Translate.X + SourceRectangle.X, Translate.Y - SourceRectangle.Y),   // destination for upper-left point of original 
					new Vector2(Translate.X - SourceRectangle.X, Translate.Y - SourceRectangle.Y),  // destination for upper-right point of  
					new Vector2(Translate.X + SourceRectangle.X, Translate.Y + SourceRectangle.Y) // destination for lower-left point of original
				};
				return destinationPoints;
			}
			if (m == Turn.Right || m == Turn.None) {
				Vector2[] destinationPoints = {
					new Vector2(Translate.X, Translate.Y),   // destination for upper-left point of original 
					new Vector2(Translate.X + SourceRectangle.X, Translate.Y - SourceRectangle.Y),  // destination for upper-right point of  
					new Vector2(Translate.X, Translate.Y + SourceRectangle.Y) // destination for lower-left point of original
				};
				return destinationPoints;
			}
			else {
				return null;
			}
		}

		public int CompareTo(Entities.Frame fr) {
			if (fr == null)
				return 1;
			float v1 = Vector2.Distance(Translate - Vector2.Zero);
			float v2 = Vector2.Distance(fr.Translate - Vector2.Zero);
			if (v1 > v2) return 1;
			else if (v1 < v2) return -1;
			else return 0;
		}
	}
}
