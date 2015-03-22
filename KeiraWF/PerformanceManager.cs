using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace KeiraWF {
    struct IteratorIndex {
        public int FromIndex, Length, Step;
        public Delegate CallBack;
    }

    class PerformanceManager { // vagy ErőforrásKezelő
        public event EventHandler FreeTime;
        public event EventHandler IterationEvent;
        public float RefreshRate { get; set; }
        public float CurrentFps { get; set; }
        public float OptimalFps { get; set; }
        public Dictionary<IteratorIndex, IEnumerable<object>> Iterators { get; set; }


        public void IterateCollection(IteratorIndex ii, params object[] args)
        {
            List<object> list = Iterators[ii].ToList();
            while (ii.FromIndex <= ii.Step && ii.FromIndex < list.Count)
            { // a lényeg hogy a speed képernyőn is akkora legyen amekkorára állítjuk 1 obj meg n obj  esetén is. Így fps-hez igazítva nem lehet pontosan beállítani,de több a semminél
                ii.CallBack.DynamicInvoke(args);
//                EventHandler.
                //Enemies[ii.From++].StepTo(Gamer, 2f * 50f / fps); // TODO: (PerfMan) mert 60 fps az már jo annál jobb nem kell, 
                if (ii.FromIndex == list.Count - 1) // de  ha akarod lehet több is és akkor kicsi fpsnél gyorsabb lesz nagynál meg lasabb
                {// a speed-et az Enemies.Count-al arányosan váltani kell, ha nagyobb akkor több speed, ha kevesebb akkor kisebb speed. (PerformanceManager!)
                    ii.FromIndex = 0;
                    ii.Step = ii.Length;
                }
                if (ii.FromIndex == ii.Step)
                {
                    ii.Step += ii.Length;
                    break;
                }
            }
            if (ii.FromIndex >= list.Count)
            {
                ii.FromIndex = 0;
                ii.Step = ii.Length;
            }
        }
    }
}
