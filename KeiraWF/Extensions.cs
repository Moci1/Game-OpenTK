using System;
using System.Collections;
using System.Collections.Generic;
//using System.Windows.Forms;
//using System.Drawing;
using InternalSection;

namespace Worker {
	public static class Extensions
	{
		/*
		 * C c = new C();
			Type tt = typeof(C).FindParent();
			while (tt != null)
			{
				Console.WriteLine(tt.Name);
				tt = tt.FindParent();
			}
		*/
		public static Type FindParent(this Type type)
		{
			if (type != null && type.BaseType != null) {
				return type.BaseType;
			}
			return null;
		}

		/// <summary>
		/// Rendezi a gyűjteményt és az összehasonlítás alapja a paraméterben lévő másik tömb.
		/// </summary>
		public static void QuickIndicesSort<T>(this IList<int> c, IList<T> fromArray)
			where T : IComparable<T>
		{
			int i = 0, j = c.Count - 1, swap = 0;
			T middle = fromArray[(fromArray.Count - 1) / 2];
			
			while (i <= j) {
				while (fromArray[c[i]].CompareTo(middle) == -1)
					i++;
				while (fromArray[c[j]].CompareTo(middle) == 1)
					j--;
				if (i <= j) {
					swap = c[i];
					c[i] = c[j];
					c[j] = swap;
					i++;
					j--;
				}
			}
		}
		public static void QuickIndicesSort<T>(this IList<int> c, IList<T> fromArray, IComparer<T> comparer, params int[] args)
		{
			if (c.Count == 0) throw new Exception("This count of list is zero.");
			else if (c.Count > fromArray.Count) throw new Exception("Range of index list is biggest.");
			
			if (args.Length == 0 || args[0] < args[1]) {
				int i, j, swap = 0;
				if (args.Length == 1) { i = args[0]; j = c.Count - 1; }
				else if (args.Length >= 2) { i = args[0]; j = args[1]; }
				else { i = 0; j = c.Count - 1; args = new int[2] { 0, c.Count - 1 }; }
				
				T middle = fromArray[c[(i+j) / 2]];
				
				while (i <= j) {
					while (comparer.Compare(fromArray[c[i]], middle) == -1) i++;
					while (comparer.Compare(fromArray[c[j]], middle) == 1) j--;
					if (i <= j) {
						swap = c[i];
						c[i] = c[j];
						c[j] = swap;
						i++;
						j--;
					}
				}
				QuickIndicesSort<T>(c, fromArray, comparer, args[0], j);
				QuickIndicesSort<T>(c, fromArray, comparer, i, args[1]);
			}
		}
		
		static int counter;
//		public static void SavePanelRect(this Control ctrll) {
//	        Bitmap bmp = new Bitmap(250,250);
//			ctrll.DrawToBitmap(bmp, new Rectangle(0,0,50,50));
////          new System.Drawing.Rectangle((int)Physics.Pont1.X-50, (int)Physics.Pont1.Y-50, 50, 50));
////			bmp.Save(@"/testBmps" + counter.ToString() + ".jpg");
//			counter++;
//		}
		
//		public static List<List<int>> TwoSort<T>(this IList<T> lst, params IComparer<T>[] comparers) {
//			List<List<int>> result = new List<List<int>>();
//			int[] i = new int[comparers.Length], j = new int[comparers.Length];
//			T middle = lst[(lst.Count - 1) / 2];
//			
//			int a;
//			for (a = 0; a < j.Length; a++) {
//				j[a] = lst.Count - 1;
//			}
//			
//			for (a = 0; a < comparers.Length; a++)
//			while (i[a] <= j[a]) {
//				for (a = 0; a < comparers.Length; a++)
//					if (comparers[a].Compare(lst[i[a]], middle) == -1)
//						i[a]++;
//				for (a = 0; a < comparers.Length; a++)
//					if (comparers[a].Compare(lst[j[a]], middle) == -1)
//						j[a]--;
//				for (a = 0; a < comparers.Length; a++)
//				if (i[a] <= j[a]) {
//					swap = result[a][i];
//					result[a][i] = result[a][j];
//					result[a][j] = swap;
//					i[a]++;
//					j[a]--;
//				}
//			}
//		}
	}
}

