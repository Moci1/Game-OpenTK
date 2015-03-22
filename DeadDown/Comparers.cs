using System;
using System.Collections.Generic;

namespace InternalSection
{
	public class PercentDistribution : IComparer<Vector2> {
		public Vector2 Source { get; set; }
		public Vector2 Destination { get; set; }
		public int Compare(Vector2 x, Vector2 y)
		{
			float ds = Vector2.Distance(Destination - Source);
			float dist1 = Vector2.Distance(Destination - x);
			float dist2 = Vector2.Distance(Destination - y);
			float dist3 = Vector2.Distance(Source - x);
			float dist4 = Vector2.Distance(Source - y);// mind2nél azt nézzük h mennyit tettünk meg eddig a célhoz képest
			float pX = 1f - (dist1 / ds * 2f) + (dist3 / ds / 4f);// minél többet haladunk a cél fele annál magyobb
			float pY = 1f - (dist2 / ds * 2f) + (dist4 / ds / 4f);// amelyik nagyobb az nyer
			if (pX > pY)
				return -1;
			else if (pX == pY) // itt el lehet rontani az irányt, mert azonos, de mégis lehet h az egyik irány jo a másik meg zsákutc
				return 0; // v csak hosszabb
			else // TODO: Miért megy vissza a zöld, ha nem találja a rövidebb utat? 
				return 1;// másik h a forPointba nincsekek elgazások pX=pY esetén mind2-t meg kell vizsgálni.
		}
	}
	public class AStar : IComparer<Vector2> {
		public Vector2 Destination { get; set; }
		public Vector2 Source { get; set; }
		public float DistLess { get; set; }
		public int Compare(Vector2 x, Vector2 y)
		{
			float f1 = Vector2.Distance(Destination - x) * DistLess + Vector2.Distance(Source - x);
			float f2 = Vector2.Distance(Destination - y) * DistLess + Vector2.Distance(Source - y);

			if (f1 < f2)
				return -1;
			else if (f1 == f2)
				return 0;
			else
				return 1;
		}
	}
	public class ShortToDestination : IComparer<Vector2>
	{
		public Vector2 Destination { get; set; }
		public int Compare(Vector2 x, Vector2 y)
		{
			float dist1 = Vector2.Distance(Destination - x);
			float dist2 = Vector2.Distance(Destination - y);
			if (dist1 < dist2)
				return -1;
			else if (dist1 == dist2)
				return 0;
			else
				return 1;
		}
	}
	public class BiggesMovement : IComparer<Vector2> 
	{
		public Vector2 FromPosition { get; set; }
		public int Compare(Vector2 x, Vector2 y)
		{
			float dist1 = Vector2.Distance(FromPosition - x);
			float dist2 = Vector2.Distance(FromPosition - y);
			if (dist1 > dist2)
				return -1;
			else if (dist1 == dist2)
				return 0;
			else
				return 1;
		}
	}
}

