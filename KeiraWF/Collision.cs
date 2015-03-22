using System;
using Geometry.Shapes;
using System.Reflection;
using System.Drawing;

namespace InternalSection {
    public static class Collision
    {
        static object locker = new object();

        #region Two texture datas:
        public static Rectangle RectA { get; set; }
        public static Rectangle RectB { get; set; }
        public static Color[] ColorA { get; set; }
        public static Color[] ColorB { get; set; }
//        public static Matrix3x3 TransformA { get; set; }
//        public static Matrix3x3 TransformB { get; set; }
        #endregion

		public static IShape Intersect(IShape shp, ShapeGroup grp) {
			IShape result = null;
			for (byte b = 0; b < grp.Members.Count; b++) {
				Type t = typeof(Collision);
				object val = t.InvokeMember("Intersect", BindingFlags.InvokeMethod | BindingFlags.Static, new OverloadBinder(), null, new object[2] 
				                            { grp.Members[b], shp });
				if (IsCollision && (val == null || (val is Vector2 && ((Vector2)val).X != float.NaN))) {
					result = grp.Members[b];
					break;
				}
			}
			return result;
		}
		public static Vector2[] Intersect(Circle c, Line ln)
        {
            lock (locker)
            {
                Vector2[] rs = new Vector2[2];
                if (Math.Abs(ln.Direction.X) <= 3)
                {
                    float xDist = Math.Abs(c.center.X - ln.Start.X);
                    if (xDist <= c.radius)
                    { // start.x eq end.x
                        if ((ln.Start.Y < ln.End.Y && c.center.Y > ln.Start.Y && c.center.Y < ln.End.Y) ||
                               (ln.Start.Y > ln.End.Y && c.center.Y < ln.Start.Y && c.center.Y > ln.End.Y))
                        {
                            IsCollision = true;
							if (xDist > c.radius - 1f) {
								rs[0] = new Vector2(c.center.X - xDist, c.center.Y);
								return rs;
							} else { //TODO: 2 ponton ütközik! tesztelve még nem lett h jo-e ez az vec1 és vec2!
								Vector2 vec1 = Vector2.Rotate(new Vector2(c.radius + c.center.X, c.center.Y), (float)Math.Acos(xDist / c.radius), c.center);
								Vector2 vec2 = Vector2.Rotate(new Vector2(c.radius + c.center.X, c.center.Y), -(float)Math.Acos(xDist / c.radius), c.center);
								rs[0] = vec1;
								rs[1] = vec2;
								return rs;
							}
                        } else
                        {
                            IsCollision = false;
                            return null;
                        }
                    } else
                    {
                        IsCollision = false;
                        return null;
                    }
                } else
                {
                    float m = ln.M;
                    //(ln.End.Y - ln.Start.Y) / (ln.End.X - ln.Start.X); // meredekség
                    float b = ln.Start.Y - (m * ln.Start.X);

                    float a1, b1, c1, a2, b2, c2;
                    MathHelper.QuadraticEquation(1f, -c.center.X, out a1, out b1, out c1);
                    MathHelper.QuadraticEquation(m, b - c.center.Y, out a2, out b2, out c2);

                    a2 += a1;
                    b2 += b1;
                    c2 += c1; // el kell osztani 2vel tehát a c.radius nem is az r hanem a d az átmérő bazz :
                    c2 -= (c.radius) * (c.radius); // c.radius*c.radius/40f

                    float? x1, x2;
                    MathHelper.QuadraticEquation(a2, b2, c2, out x1, out x2);
                    if (x1.HasValue || x2.HasValue)
                    {
                        X1 = x1.Value;
                        X2 = x2.Value;
                        //				x1 = (float)Math.Floor(x1.Value);
                        //				x2 = (float)Math.Ceiling(x2.Value);
                        float y1 = m * x1.Value + b, y2 = m * x2.Value + b;
                        Y1 = y1;
                        Y2 = y2;
                        bool asd = (x1.Value > ln.Start.X && x2.Value < ln.End.X);
                        bool dsa = (x2.Value > ln.Start.X && x1.Value < ln.End.X);
                        //					if (((x1.Value > ln.Start.X && x2.Value < ln.End.X) ||
                        //					    (x2.Value > ln.Start.X && x1.Value < ln.End.X)))
                        //(Math.Abs(y1 - c.center.Y) < c.radius || Math.Abs(y2 - c.center.Y) < c.radius))
                        IsCollision = true;
                        rs[0] = new Vector2(x1.Value, y1);
                        if (!ContainsLine(rs[0], ln))
                        {
                            rs[0] = new Vector2(x2.Value, y2);
                            if (!ContainsLine(rs[0], ln))
                            {
                                IsCollision = false;
                                return null;
                            }
                            return rs; // jo helyen jon elo
                        }
                        rs[1] = new Vector2(x2.Value, y2);
                        if (ContainsLine(rs[1], ln))
                            return rs; // jo helyen jon elo
                        rs[1] = new Vector2(float.NaN, float.NaN);
                        return rs;// jo helyen jon elo
                    } // az egyenes minden pontján ütközne, de mivel ez egy szakasz és körbe van rajzolva...
                    else
                    { // ...a körberajzolt szakasz körének 2xesétöl futtatjuk csak ezt az Intersect metodust. (SearchCollision)
                        IsCollision = false;
                        return null;
                    }
                }
            }
        }
		public static bool ContainsLine(Vector2 v, Line line) {
			Vector2 va = line.Start - v;
			Vector2 vb = line.End - v;

			va.X = (float)Math.Floor(va.X);
			va.Y = (float)Math.Floor(va.Y);
			vb.X = (float)Math.Floor(vb.X);
			vb.Y = (float)Math.Floor(vb.Y);

			bool xValid = ((va.X >= 0 && vb.X <= 0) || (va.X <= 0 && vb.X >= 0)) && ((va.Y >= 0 && vb.Y <= 0) || (va.Y <= 0 && vb.Y >= 0));
			if (xValid)
				return true;
			return false;
		}
		public static Vector2 Intersect(Line ln1, Line ln2) {
			IsCollision = false;
			float x = (float)Math.Round((ln2.B - ln1.B) / (ln1.M - ln2.M + .001), 3);
			if (x.Equals(float.NaN)) {
				if (ln1.Start.X >= ln2.Start.X && ln1.Start.X <= ln2.End.X) {
					IsCollision = true; // ezt majd gondold át me csak ideirtam
					return ln1.Start; 
				}
				if (ln1.End.X >= ln2.Start.X && ln1.End.X <= ln2.End.X) {
					IsCollision = true; // ezt majd gondold át me csak ideirtam
					return ln1.End;
				}
			}
			float y = (float)Math.Round(ln1.M * x + ln1.B, 3); // függő egyenest, szakaszok levágását csináld már meg normálisan!
//			if (ln1.Start.X < ln1.End.X && ln2.Start.X < ln1.End.X) {
//				if (x >= (float)Math.Round(ln2.Start.X, 3) && x <= (float)Math.Round(ln2.End.X, 3) && x >= (float)Math.Round(ln1.Start.X, 3) && x <= (float)Math.Round(ln1.End.X, 3)) {
			Vector2 result = new Vector2(x, y);

			Vector2 va = ln1.Start - result;
			Vector2 vb = ln1.End - result;
			va.X = (float)Math.Floor(va.X);
			va.Y = (float)Math.Floor(va.Y);
			vb.X = (float)Math.Floor(vb.X);
			vb.Y = (float)Math.Floor(vb.Y);
			bool xValid = ((va.X >= 0 && vb.X <= 0) || (va.X <= 0 && vb.X >= 0) || va.X == vb.X) && ((va.Y >= 0 && vb.Y <= 0) || (va.Y <= 0 && vb.Y >= 0)  || va.Y == vb.Y);
			if (xValid) {
				va = ln2.Start - result;
				vb = ln2.End - result;
				va.X = (float)Math.Floor(va.X);
				va.Y = (float)Math.Floor(va.Y);
				vb.X = (float)Math.Floor(vb.X);
				vb.Y = (float)Math.Floor(vb.Y);
				bool yValid = ((va.X >= 0 && vb.X <= 0) || (va.X <= 0 && vb.X >= 0) || va.X == vb.X) && ((va.Y >= 0 && vb.Y <= 0) || (va.Y <= 0 && vb.Y >= 0) || va.Y == vb.Y);
				if (yValid){ //((!xValid && yValid) || (xValid && littleY) || (xValid && yValid)) {
					return result;
				}
			}

//			if (s1.X / s1.Y == s2.X / s2.Y) {
//				float dist1 = Vector2.Distance(s1);
//				float dist2 = Vector2.Distance(s2);
//				if (dist1 >= dist2) {
//					return result;
//				}
//			}
//			} else {
//				if (x <= (float)Math.Round(ln2.Start.X, 3) && x >= (float)Math.Round(ln2.End.X, 3) && x <= (float)Math.Round(ln1.Start.X, 3) && x >= (float)Math.Round(ln1.End.X, 3)) {
//					IsCollision = true;
//					return new Vector2(x, y);
//				}
//			}
			return new Vector2(float.NaN, float.NaN);
		}
		public static bool Intersect(Line ln, Vector2 v) {
			float lnY = ln.M * v.X + ln.B; // ha v.X helyen az egyenes y pontja
			return (lnY == v.Y)? true : false; // ugyanaz mint a v.Y akkor rajta van
		}
		public static bool Intersect(Line ln, Vector2 v, float offset) {
			float lnY = ln.M * v.X + ln.B; // ha v.X helyen az egyenes y pontja
			lny = new Vector2(v.X, lnY);
			float ofs = Math.Abs(offset); // ugyanaz mint a v.Y akkor rajta van
			return (lnY + ofs > v.Y && lnY - ofs < v.Y)? true : false; 
		}
		public static bool PointInTriangle(Vector2 P, Vector2 A, Vector2 B, Vector2 C) {
			// Compute vectors        
			Vector2 v0 = C - A;
			Vector2 v1 = B - A;
			Vector2 v2 = P - A;

			// Compute dot products
			float dot00 = Vector2.Dot(v0, v0);
			float dot01 = Vector2.Dot(v0, v1);
			float dot02 = Vector2.Dot(v0, v2);
			float dot11 = Vector2.Dot(v1, v1);
			float dot12 = Vector2.Dot(v1, v2);

			// Compute barycentric coordinates
			float invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
			float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
			float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

			// Check if point is in triangle
			bool res = (u >= 0f) && (v >= 0f) && (u + v < .99995f);
			return res;
		}
		public static Vector2 lny;

		/// <summary>
        /// Megállapítja, hogy a két kör ütközik-e és beállítja a ResultDirection tulajdonságot.
        /// </summary>
        public static float Intersect(Circle c1, Circle c2)
        {
            float d = Vector2.Distance(c1.center - c2.center);
            if (d <= c1.radius + c2.radius)
            {
                if (PerPixel != PerPixelMode.None)
                    IsPerPixelCollision();
                else
                    IsCollision = true;
            }
            else if (d <= c1.radius + c2.radius + 1)
            {
                if (PerPixel != PerPixelMode.None)
                    IsPerPixelCollision();
                else
                    IsCollision = true;
            }
            else
            {
                IsCollision = false;
            }
			return d - c1.radius - c2.radius;
        }
        
		public static float X1, X2, Y1, Y2;
        /*public static void Intersect(Line ln, Rect r)
        {
            throw new NotImplementedException();
        }
        public static void Intersect(Line ln, Rectangle r)
        {
            throw new NotImplementedException();
        }*/
        public static bool Intersect(Circle c, Vector2 v)
        {
            if (Vector2.Distance(c.center - v) <= c.radius)
                return true;
            return false;
        }
        /*public static void Intersect(Rect r, Circle c)
        {
            Vector2 middle = new Vector2(r.Width / 2f, r.Height / 2f); // nem ez a middle
            if ((c.center.X + c.radius >= r.Left || c.center.X - c.radius <= r.Right) &&
                (c.center.Y + c.radius >= r.Top || c.center.Y - c.radius <= r.Bottom))
            {
                IsPerPixelCollision();
            }
            else
                IsCollision = false;
        }*/
        public static bool InRect(RectangleF r, Circle c)
        {
			if (c.center.X + c.radius > r.X && c.center.Y + c.radius > r.Y) {
				if (c.center.X + c.radius < r.X + r.Width && c.center.Y + c.radius < r.Y + r.Height) {
					return true;
				}
			}
			return false;
//            Vector2 middle = new Vector2(r.Width / 2f, r.Height / 2f); // nem ez a middle
//            if ((c.center.X + c.radius >= r.Left || c.center.X - c.radius <= r.Right) &&
//                (c.center.Y + c.radius >= r.Top || c.center.Y - c.radius <= r.Bottom))
//            {
//                IsPerPixelCollision();
//            }
//            else
//                IsCollision = false;
        }
        /// <summary>
        /// Egyszerű vizsglattal megállapítja, hogy ütközik-e két téglalap és beállítja a ResultDirection tulajdonságot. Továbbá van lehetőség per-pixel vizsgálatra is.
        /// </summary>
//        public static void Intersect(Rectangle rectangleA, Rectangle rectangleB)
//        {
//            int top = Math.Max(rectangleA.Top, rectangleB.Top);
//            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
//            int left = Math.Max(rectangleA.Left, rectangleB.Left);
//            int right = Math.Min(rectangleA.Right, rectangleB.Right);
//
//            if (top < bottom && left < right)
//            {
//                Vector2 v = (new Vector2(rectangleB.X, rectangleB.Y) - new Vector2(rectangleA.X, rectangleA.Y));
//                int x = right - left, y = bottom - top;
//                if (x < y)
//                    v.Y = 0;
//                else if (x > y) // top, bottom, 
//                    v.X = 0;
//                else
//                {
//                    v.X = 0;
//                    v.Y = 0;
//                }
//                if (PerPixel == PerPixelMode.None)
//                {
//                    IsCollision = true;
//                    return;
//                }
//                IsPerPixelCollision();
//                
//            }
//            else
//            {
//                IsCollision = false;
//            }
//        }
        public static void IsPerPixelCollision()
        {
//            switch (PerPixel)
//            {
//                case PerPixelMode.Transform:
//                    IntersectPixels(TransformA, RectA.Width, RectA.Height, ColorA, TransformB, RectB.Width, RectB.Height, ColorB);
//                    break;
//                case PerPixelMode.NonTransform: IntersectPixels(RectA, ColorA, RectB, ColorB);
//                    break;
//                case PerPixelMode.None:
//                    break;
//            }
        }
        /// <summary>
        /// Meghatározza azt a pontot ahol a két vonal metszi egymást
        /// </summary>
        public static void CommonPoint(Line l1, Line l2)
        {
            Vector2 v = new Vector2(((l1.Start.X * l1.End.Y - l1.Start.Y * l1.End.X) * (l2.Start.X - l2.End.X)
                - (l1.Start.X - l1.End.X) * (l2.Start.X * l2.End.Y - l2.Start.Y * l2.End.X)) / ((l1.Start.X - l1.End.X) * (l2.Start.Y - l2.End.Y) - (l1.Start.Y - l1.End.Y) * (l2.Start.X - l2.End.X)),
                ((l1.Start.X * l1.End.Y - l1.Start.Y * l1.End.X) * (l2.Start.Y - l2.End.Y)
                - (l1.Start.Y - l1.End.Y) * (l2.Start.X * l2.End.Y - l2.Start.Y * l2.End.X)) / ((l1.Start.X - l1.End.X) * (l2.Start.Y - l2.End.Y) - (l1.Start.Y - l1.End.Y) * (l2.Start.X - l2.End.X)));
            if (v.X != float.NaN && v.Y != float.NaN && !float.IsInfinity(v.X) && !float.IsInfinity(v.Y))
            {
                 //= l1.Start + (l1.End - l1.Start) - l2.Start + (l2.End - l2.Start);
                IsCollision = true;
            }
            IsCollision = false;
        }
/// <summary>
        /// Determines if there is overlap of the non-transparent pixels between two
        /// sprites.
        /// </summary>
        /// <param name="transformA">World transform of the first sprite.</param>
        /// <param name="widthA">Width of the first sprite's texture.</param>
        /// <param name="heightA">Height of the first sprite's texture.</param>
        /// <param name="dataA">Pixel color data of the first sprite.</param>
        /// <param name="transformB">World transform of the second sprite.</param>
        /// <param name="widthB">Width of the second sprite's texture.</param>
        /// <param name="heightB">Height of the second sprite's texture.</param>
        /// <param name="dataB">Pixel color data of the second sprite.</param>
//        public static void IntersectPixels(
//                            Matrix3x3 transformA, int widthA, int heightA, Color[] dataA,
//                            Matrix3x3 transformB, int widthB, int heightB, Color[] dataB)
//        {
//            // Calculate a matrix which transforms from A's local space into
//            // world space and then into B's local space
//            Matrix3x3 transformAToB = transformA * Matrix3x3.Invert(transformB);
//
//            // When a point moves in A's local space, it moves in B's local space with a
//            // fixed direction and distance proportional to the movement in A.
//            // This algorithm steps through A one pixel at a time along A's X and Y axes
//            // Calculate the analogous steps in B:
//            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
//            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);
//
//            // Calculate the top left corner of A in B's local space
//            // This variable will be reused to keep track of the start of each row
//            Vector2 yPosInB = Vector2.Transform(Vector2.Zero, transformAToB);
//
//            // For each row of pixels in A
//            for (int yA = 0; yA < heightA; yA++)
//            {
//                // Start at the beginning of the row
//                Vector2 posInB = yPosInB;
//
//                // For each pixel in this row
//                for (int xA = 0; xA < widthA; xA++)
//                {
//                    // Round to the nearest pixel
//                    int xB = (int)Math.Round(posInB.X);
//                    int yB = (int)Math.Round(posInB.Y);
//
//                    // If the pixel lies within the bounds of B
//                    if (0 <= xB && xB < widthB &&
//                        0 <= yB && yB < heightB)
//                    {
//                        // Get the colors of the overlapping pixels
//                        Color colorA = dataA[xA + yA * widthA];
//                        Color colorB = dataB[xB + yB * widthB];
//
//                        // If both pixels are not completely transparent,
//                        if (colorA.A != 0 && colorB.A != 0)
//                        {
//                            IsCollision = true;
//                            return;
//                        }
//                    }
//
//                    // Move to the next pixel in the row
//                    posInB += stepX;
//                }
//
//                // Move to the next row
//                yPosInB += stepY;
//            }
//
//            // No intersection found
//            IsCollision = false;
//        }
        /// <summary>
        /// Calculates an axis aligned rectangle which fully contains an arbitrarily
        /// transformed axis aligned rectangle.
        /// </summary>
        /// <param name="rectangle">Original bounding rectangle.</param>
        /// <param name="transform">World transform of the rectangle.</param>
        /// <returns>A new rectangle which contains the trasnformed rectangle.</returns>
//        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle,
//                                                           Matrix3x3 transform)
//        {
//            // Get all four corners in local space
//            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
//            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
//            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
//            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);
//
//            // Transform all four corners into work space
//            Vector2.Transform(ref leftTop, ref transform, out leftTop);
//            Vector2.Transform(ref rightTop, ref transform, out rightTop);
//            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
//            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);
//
//            // Find the minimum and maximum extents of the rectangle in world space
//            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
//                                      Vector2.Min(leftBottom, rightBottom));
//            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
//                                      Vector2.Max(leftBottom, rightBottom));
//
//            // Return that as a rectangle
//            return new Rectangle((int)min.X, (int)min.Y,
//                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
//        }
        /// <summary>
        /// Determines if there is overlap of the non-transparent pixels
        /// between two sprites.
        /// </summary>
        /// <param name="rectangleA">Bounding rectangle of the first sprite</param>
        /// <param name="dataA">Pixel data of the first sprite</param>
        /// <param name="rectangleB">Bouding rectangle of the second sprite</param>
        /// <param name="dataB">Pixel data of the second sprite</param>
        /// <returns>True if non-transparent pixels overlap; false otherwise</returns>
        public static void IntersectPixels(Rectangle RectA, Color[] ColorA,
                                           Rectangle RectB, Color[] ColorB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(RectA.Top, RectB.Top);
            int bottom = Math.Min(RectA.Bottom, RectB.Bottom);
            int left = Math.Max(RectA.Left, RectB.Left);
            int right = Math.Min(RectA.Right, RectB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = ColorA[(x - RectA.Left) +
                                         (y - RectA.Top) * RectA.Width];
                    Color colorB = ColorB[(x - RectB.Left) +
                                         (y - RectB.Top) * RectB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        IsCollision = true;
                        return;
                    }
                }
            }

            // No intersection found
            IsCollision = false;
        }
		
        /// <summary>
        /// Ha két alap-alakzat ütközik akkor utánna van lehetőség per-pixel ütközés vizsgálatra is, de ez nem befolyásolja a ResultDirection prop-ot.
        /// </summary>
        public static PerPixelMode PerPixel { get; set; }
        /// <summary>
        /// Azért van, mert ha egy vizsgálat után nincs ütközés, akkor a ResultDirection nem változik, de ez igen.
        /// </summary>
        public static bool IsCollision { get; private set; }
    }
    public enum PerPixelMode
    {
        None,
        NonTransform,
        Transform
    }
}