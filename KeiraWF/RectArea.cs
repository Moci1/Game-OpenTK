using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using InternalSection;
using Entities;
using Geometry.Shapes;

namespace InternalSection
{
	public struct AreaModel {
		public StaticModel Model;
		public List<int> AreaIDs;

		public AreaModel(StaticModel model, params int[] ids) {
			this.Model = model;
			AreaIDs = new List<int>();
			AreaIDs.AddRange(ids);
		}
		public AreaModel(StaticModel model) {
			this.Model = model;
			AreaIDs = new List<int>();
		}
	}
	public class RectArea : SimpleArea
	{
		int lastRect;
		List<List<int>> areaIDs = new List<List<int>>();
        List<List<int>> inAreas = new List<List<int>>();
        public RectangleF OuterRect { get; private set; }
		public RectArea(RectangleF r) : base()
		{
            OuterRect = r;
		}
		public void MapModels() {
            areaIDs = new List<List<int>>();
            inAreas = new List<List<int>>();
			for (int i = 0; i < GameObjects.Count; i++) {
                areaIDs.Add(new List<int>());
                j = 0;
                lastRect = 0;
				MapModel(OuterRect, i); // or default to int?:
                GameObjects[i].AreaId = areaIDs[i].LastOrDefault(); // és ha 0 elemû? pl nincs benn a 0. rect-be. 
			}
            int max = areaIDs.Max(item => { 
                if (item.Count != 0)
                    return item.Max();
                return -1;
            });
            inAreas = new List<List<int>>(max);
            for (int i = 0; i <= max; i++)
            {
                var m = areaIDs.SelectMany((item, index) => {
                    var vr = item.Where(it => it == i).Select(itm => index);
                    if (item.Contains(i))
                        return vr;
                    return vr;
                });
                inAreas.Add(m.ToList()); // ha kicsik az area rectek akkor kevés elem megy bele
            } // így sok lesz az üres tömb az objsInAreas-be
		} // inAreas 0. tagja tartalmazza az összes objectet ami beleesik az OuterRect-be, az összes többinek az elemszáma az inAreas-be = a inAreas[0]-val
        void InGroup(int i)
        {
            // ezt csináld meg for ciklusosra mert fukk bele "A gyüjtemény modosult ..... számbavételi xarság..."
            var m = areaIDs.SelectMany((item, index) =>
            {
                var vr = item.Where(it => it == i).Select(itm => index);
                if (item.Contains(i))
                    return vr;
                return vr;
            });
            if (i <= inAreas.Count - 1)
            {
                inAreas[i] = new List<int>(m.Count());
                inAreas[i] = m.ToList();
            } else
            {
                inAreas.Add(m.ToList()); // TODO: ez így nem oké mert ha a jobb also sarokba van akkor az addig hiányzokat is hozzá kell tenni még ha 0 elemüek is!!
            }
        }
        int j;
        /// <summary>
        /// Need clear actual before use this function.
        /// </summary>
		public bool MapModel(RectangleF r, int i) {
			Circle c = GameObjects[i].Shape.BoundingCircle;
            int aidIdx = (areaIDs.Count <= i) ? areaIDs.Count - 1 : i;
			if (areaIDs[i].Count < 3 && Collision.InRect(r, c)) { // 3 konstans! írd meg dinamikusra. ha 3 akkor 3 szintu fa
                areaIDs[i].Add(lastRect++);
                lastRect = (4 * lastRect + 1) - 4;
				RectangleF[] rs = new RectangleF[4] {
					new RectangleF(r.X ,r.Y, r.Width / 2f,r.Height /2f),
					new RectangleF(r.X + r.Width/ 2f, r.Y, r.Width / 2f,r.Height /2f),
					new RectangleF(r.X, r.Y + r.Height / 2f, r.Width / 2f,r.Height /2f),
					new RectangleF(r.X + r.Width / 2f, r.Y + r.Height / 2f, r.Width / 2f,r.Height /2f),
				};
				for (j = 1; j - 1 < rs.Length; j++) {
                    MapModel(rs[j - 1], i);
                    lastRect++; // ezt a return false-hoz is tehetjük? vagy mi?
				}
				return true;
			} else {
                //lastRect++;
				return false;
			}
		}

        protected override bool HandleTransform(object sender, EventArgs e)
        {
            lock (this)
            {
				bool result = false;
                obj = sender as StaticModel;
                circle = obj.VirtualShape.BoundingCircle;
                StaticModel sm = null;
                int objIndex = -1, i = 0;
                for (i = 0; i < inAreas[obj.AreaId].Count; i++) // TODO: ha kimegy a mappbol... A map minden részét tárod el az inAreas-ba!!!!!
                {
                    sm = GameObjects[inAreas[obj.AreaId][i]];
					if (!object.ReferenceEquals(sm, obj))
						result = SingleCollision(sm);
					if (result)
						break; // return true;
                    if (obj.Equals(sm))
                        objIndex = inAreas[obj.AreaId][i];
                }//The process or thread has changed since last step.
				Console.WriteLine(result);
                for (i = 0; i < areaIDs[objIndex].Count; i++)
                {
                    inAreas[areaIDs[objIndex][i]].Remove(objIndex);
                }
                areaIDs[objIndex].Clear();
                lastRect = j = 0;
                MapModel(OuterRect, objIndex);
                for (i = 0; i < areaIDs[objIndex].Count; i++)
                {
                    InGroup(areaIDs[objIndex][i]);
                }
                obj.AreaId = areaIDs[objIndex].Last();

                return result;
            }
        }


	}
}

