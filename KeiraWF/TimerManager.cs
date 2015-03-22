using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

// TODO: Az erőforrásokat oszd el. Pl az a player kapjon többet amelyik meglátott. Amelyik meg nincs is a képbe kevesebbet.
// Aztán timereknek csinálj vmi sync-et, h egyszerre több ne egyszerre fusson, meg az interval dinamikusan állítódjon pl fps-hez igazítva

namespace InternalSection {
    public class TimerManager {
        List<Timer> timers = new List<Timer>();
        
        public TimerManager(params TimerCallback[] callback)
        {
            for (int i = 0; i < callback.Length; i++)
                timers.Add(new Timer(callback[i]));
        }

        public void StartTimer(int i, double interval)
        {
        }
        public void StartSyncTimers()
        {
            for (int i = 0; i < timers.Count; i++)
            {
                timers[i].Change(10, 70);
            }
        }
        public void SyncTimers() // tartsd szinkronba
        { // most frissítsd a szinkronizációs szakaszokat. most frissítsd az intervalokat stb...

        }

        public Timer this[int i]
        {
            get
            {
                return timers[i];
            }
        }
    }
}


