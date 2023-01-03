using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Galaxy.Timer
{
    public abstract class AbsTimerData
    {
        public ulong TimerId;
        public float Interval;
        public float NextTick;
        public WeakReference Reference;
        public bool IsFrameAction = false;
        public bool IsRealTimer = false;
        public int Counter = 0;


        public void BindReference(object origin)
        {
            if (origin != null)
            {
                Reference = new WeakReference(origin, true);
            }
        }

        public bool Valid
        {
            get
            {
                if (Reference != null && Reference.IsAlive && Reference.Target != null)
                {
                    if (Reference.Target is UnityEngine.Object)
                    {
                        UnityEngine.Object obj = Reference.Target as UnityEngine.Object;
                        if (obj != null)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        public abstract Delegate Action
        {
            get; set;
        }

        public virtual void DoAction(float realDeltaTime)
        {
            Counter++;
        }
    }
}
