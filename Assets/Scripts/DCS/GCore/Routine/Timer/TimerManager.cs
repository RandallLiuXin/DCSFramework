using System;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Timer
{
    public class TimerManager : MonoBehaviour
	{
        #region static param
        private static ulong s_NextTimerId;
        private static float s_RealTimeSinceStartup;
        private static float s_EngineTimeSinceStartup;
        private static float s_RealDeltaTime;
        private static float s_EngineDeltaTime;
        private static readonly object m_QueueLock = new object();
        private static TimerManager m_Instance;
        private static TimerManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    throw new GalaxyException("We must have a TimerManager instance");
				}
                return m_Instance;
            }
        }
        
        private static ulong GetNextTimerId()
        {
            if (s_NextTimerId == ulong.MaxValue)
            {
                s_NextTimerId = ulong.MinValue;
            }
            return (++s_NextTimerId);
        }

        public static void RemoveTimer(ulong timerId)
        {
            lock (m_QueueLock)
            {
                if (!Instance.m_RemoveTimeList.Contains(timerId))
                {
                    Instance.m_RemoveTimeList.Add(timerId);
                }

                Instance.m_TimerDict.ForceRemove(timerId);
            }
        }

        public static int GetTimerCounter(ulong timerId)
        {
            if (Instance.m_TimerDict.ContainsKey(timerId))
            {
                AbsTimerData data = Instance.m_TimerDict[timerId];
                return data.Counter;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reciver">触发者，为了能自动销毁，必须基于Mono</param>
        /// <param name="handler">出发事件</param>
        /// <returns></returns>
        public static ulong AddRealTimer(UnityEngine.Object reciver, Action<float> handler)
        {
            var p = Instance.GetTimerData(new TimerData(), 0, 0, true);
            p.IsRealTimer = true;
            p.IsFrameAction = true;
            p.BindReference(reciver);
            p.Action = handler;
            return Instance.AddRealTimer(p);
        }

        public static ulong AddRealTimer<T>(UnityEngine.Object reciver, Action<float, T> handler, T arg1)
        {
            var p = Instance.GetTimerData(new TimerData<T>(), 0, 0, true);
            p.IsRealTimer = true;
            p.IsFrameAction = true;
            p.BindReference(reciver);
            p.Action = handler;
            p.Arg1 = arg1;
            return Instance.AddRealTimer(p);
        }

        public static ulong AddRealTimer<T, U>(UnityEngine.Object reciver, Action<float, T, U> handler, T arg1, U arg2)
        {
            var p = Instance.GetTimerData(new TimerData<T, U>(), 0, 0, true);
            p.IsRealTimer = true;
            p.IsFrameAction = true;
            p.BindReference(reciver);
            p.Action = handler;
            p.Arg1 = arg1;
            p.Arg2 = arg2;
            return Instance.AddRealTimer(p);
        }

        public static ulong AddRealTimer<T, U, V>(UnityEngine.Object reciver, Action<float, T, U, V> handler, T arg1, U arg2, V arg3)
        {
            var p = Instance.GetTimerData(new TimerData<T, U, V>(), 0, 0, true);
            p.IsRealTimer = true;
            p.IsFrameAction = true;
            p.BindReference(reciver);
            p.Action = handler;
            p.Arg1 = arg1;
            p.Arg2 = arg2;
            p.Arg3 = arg3;
            return Instance.AddRealTimer(p);
        }

        public static ulong AddRealTimer(UnityEngine.Object reciver, float start, Action<float> handler)
        {
            return AddRealTimer(reciver, start, 0, handler);
        }

        public static ulong AddRealTimer<T>(UnityEngine.Object reciver, float start, Action<float, T> handler, T arg1)
        {
            return AddRealTimer(reciver, start, 0, handler, arg1);
        }

        public static ulong AddRealTimer<T, U>(UnityEngine.Object reciver, float start, Action<float, T, U> handler, T arg1, U arg2)
        {
            return AddRealTimer(reciver, start, 0, handler, arg1, arg2);
        }

        public static ulong AddRealTimer<T, U, V>(UnityEngine.Object reciver, float start, Action<float, T, U, V> handler, T arg1, U arg2, V arg3)
        {
            return AddRealTimer(reciver, start, 0, handler, arg1, arg2, arg3);
        }
        
        public static ulong AddRealTimer(UnityEngine.Object reciver, float start, float interval, Action<float> handler)
        {
            if (Instance == null)
            {
                Debug.LogError("TimerManager is Not Initialized");
                return 0;
            }

            var p = Instance.GetTimerData(new TimerData(), start, interval, true);
            p.IsRealTimer = true;
            p.IsFrameAction = false;
            p.BindReference(reciver);
            p.Action = handler;
            return Instance.AddRealTimer(p);
        }

        public static ulong AddRealTimer<T>(UnityEngine.Object reciver, float start, float interval, Action<float, T> handler, T arg1)
        {
            if (Instance == null)
            {
                Debug.LogError("TimerManager is Not Initialized");
                return 0;
            }
            var p = Instance.GetTimerData(new TimerData<T>(), start, interval, true);
            p.IsRealTimer = true;
            p.IsFrameAction = false;
            p.BindReference(reciver);
            p.Action = handler;
            p.Arg1 = arg1;
            return Instance.AddRealTimer(p);
        }

        public static ulong AddRealTimer<T, U>(UnityEngine.Object reciver, float start, float interval, Action<float, T, U> handler, T arg1, U arg2)
        {
            if (Instance == null)
            {
                Debug.LogError("TimerManager is Not Initialized");
                return 0;
            }
            var p = Instance.GetTimerData(new TimerData<T, U>(), start, interval, true);
            p.IsRealTimer = true;
            p.IsFrameAction = false;
            p.BindReference(reciver);
            p.Action = handler;
            p.Arg1 = arg1;
            p.Arg2 = arg2;
            return Instance.AddRealTimer(p);
        }

        public static ulong AddRealTimer<T, U, V>(UnityEngine.Object reciver, float start, float interval, Action<float, T, U, V> handler, T arg1, U arg2, V arg3)
        {
            var p = Instance.GetTimerData(new TimerData<T, U, V>(), start, interval, true);
            p.IsRealTimer = true;
            p.IsFrameAction = false;
            p.BindReference(reciver);
            p.Action = handler;
            p.Arg1 = arg1;
            p.Arg2 = arg2;
            p.Arg3 = arg3;
            return Instance.AddRealTimer(p);
        }
        
        public static ulong AddEngineTimer(UnityEngine.Object reciver, Action<float> handler)
        {
            var p = Instance.GetTimerData(new TimerData(), 0, 0, false);
            p.IsRealTimer = false;
            p.IsFrameAction = true;
            p.BindReference(reciver);
            p.Action = handler;
            return Instance.AddEngineTimer(p);
        }

        public static ulong AddEngineTimer<T>(UnityEngine.Object reciver, Action<float, T> handler, T arg1)
        {
            var p = Instance.GetTimerData(new TimerData<T>(), 0, 0, false);
            p.IsRealTimer = false;
            p.IsFrameAction = true;
            p.BindReference(reciver);
            p.Action = handler;
            p.Arg1 = arg1;
            return Instance.AddEngineTimer(p);
        }

        public static ulong AddEngineTimer<T, U>(UnityEngine.Object reciver, Action<float, T, U> handler, T arg1, U arg2)
        {
            var p = Instance.GetTimerData(new TimerData<T, U>(), 0, 0, false);
            p.IsRealTimer = false;
            p.IsFrameAction = true;
            p.BindReference(reciver);
            p.Action = handler;
            p.Arg1 = arg1;
            p.Arg2 = arg2;
            return Instance.AddEngineTimer(p);
        }

        public static ulong AddEngineTimer<T, U, V>(UnityEngine.Object reciver, Action<float, T, U, V> handler, T arg1, U arg2, V arg3)
        {
            var p = Instance.GetTimerData(new TimerData<T, U, V>(), 0, 0, false);
            p.IsRealTimer = false;
            p.IsFrameAction = true;
            p.BindReference(reciver);
            p.Action = handler;
            p.Arg1 = arg1;
            p.Arg2 = arg2;
            p.Arg3 = arg3;
            return Instance.AddEngineTimer(p);
        }

        public static ulong AddEngineTimer(UnityEngine.Object reciver, float start, Action<float> handler)
        {
            return AddEngineTimer(reciver, start, 0, handler);
        }

        public static ulong AddEngineTimer<T>(UnityEngine.Object reciver, float start, Action<float, T> handler, T arg1)
        {
            return AddEngineTimer(reciver, start, 0, handler, arg1);
        }

        public static ulong AddEngineTimer<T, U>(UnityEngine.Object reciver, float start, Action<float, T, U> handler, T arg1, U arg2)
        {
            return AddEngineTimer(reciver, start, 0, handler, arg1, arg2);
        }

        public static ulong AddEngineTimer<T, U, V>(UnityEngine.Object reciver, float start, Action<float, T, U, V> handler, T arg1, U arg2, V arg3)
        {
            return AddEngineTimer(reciver, start, 0, handler, arg1, arg2, arg3);
        }
        
        public static ulong AddEngineTimer(UnityEngine.Object reciver, float start, float interval, Action<float> handler)
        {
            if (Instance == null)
            {
                Debug.LogError("TimerManager is Not Initialized");
                return 0;
            }

            var p = Instance.GetTimerData(new TimerData(), start, interval, false);
            p.IsRealTimer = false;
            p.IsFrameAction = false;
            p.BindReference(reciver);
            p.Action = handler;
            return Instance.AddEngineTimer(p);
        }

        public static ulong AddEngineTimer<T>(UnityEngine.Object reciver, float start, float interval, Action<float, T> handler, T arg1)
        {
            if (Instance == null)
            {
                Debug.LogError("TimerManager is Not Initialized");
                return 0;
            }
            var p = Instance.GetTimerData(new TimerData<T>(), start, interval, false);
            p.IsRealTimer = false;
            p.IsFrameAction = false;
            p.BindReference(reciver);
            p.Action = handler;
            p.Arg1 = arg1;
            return Instance.AddEngineTimer(p);
        }

        public static ulong AddEngineTimer<T, U>(UnityEngine.Object reciver, float start, float interval, Action<float, T, U> handler, T arg1, U arg2)
        {
            if (Instance == null)
            {
                Debug.LogError("TimerManager is Not Initialized");
                return 0;
            }
            var p = Instance.GetTimerData(new TimerData<T, U>(), start, interval, false);
            p.IsRealTimer = false;
            p.IsFrameAction = false;
            p.BindReference(reciver);
            p.Action = handler;
            p.Arg1 = arg1;
            p.Arg2 = arg2;
            return Instance.AddEngineTimer(p);
        }

        public static ulong AddEngineTimer<T, U, V>(UnityEngine.Object reciver, float start, float interval, Action<float, T, U, V> handler, T arg1, U arg2, V arg3)
        {
            var p = Instance.GetTimerData(new TimerData<T, U, V>(), start, interval, false);
            p.IsRealTimer = false;
            p.IsFrameAction = false;
            p.BindReference(reciver);
            p.Action = handler;
            p.Arg1 = arg1;
            p.Arg2 = arg2;
            p.Arg3 = arg3;
            return Instance.AddEngineTimer(p);
        }

        #endregion

        // 补充 方便查找效率 id - entity
        private Dictionary<ulong, AbsTimerData> m_TimerDict = new Dictionary<ulong, AbsTimerData>();

        //排序 id - entity - priority
        private KeyedPriorityQueue<ulong, AbsTimerData, float> m_RealTimeQueue = new KeyedPriorityQueue<ulong, AbsTimerData, float>();
        private KeyedPriorityQueue<ulong, AbsTimerData, float> m_EngineTimeQueue = new KeyedPriorityQueue<ulong, AbsTimerData, float>();

        private Queue<AbsTimerData> m_RealTimeAddQueue = new Queue<AbsTimerData>();
        private Queue<AbsTimerData> m_EngineTimeAddQueue = new Queue<AbsTimerData>();

        private List<ulong> m_RemoveTimeList = new List<ulong>();

        private ulong AddRealTimer(AbsTimerData p)
        {
            lock (m_QueueLock)
            {
                m_RealTimeQueue.Enqueue(p.TimerId, p, p.NextTick);
                m_TimerDict.ForceAdd(p.TimerId, p);
            }
            return p.TimerId;
        }

        private ulong AddEngineTimer(AbsTimerData p)
        {
            lock (m_QueueLock)
            {
                m_EngineTimeQueue.Enqueue(p.TimerId, p, p.NextTick);
                m_TimerDict.ForceAdd(p.TimerId, p);
            }
            return p.TimerId;
        }

        private T GetTimerData<T>(T p, float start, float interval, bool isRealTime) where T : AbsTimerData
        {
            p.Interval = interval;
            p.TimerId = GetNextTimerId();
            if (isRealTime)
            {
                p.NextTick = s_RealTimeSinceStartup + 1 + start;
            }
            else
            {
                p.NextTick = s_EngineTimeSinceStartup + 1 + start;
            }
            return p;
        }

        private void Awake()
        {
            Debug.Assert(m_Instance == null);
            m_Instance = this;
        }

        private void Start()
		{
            s_RealTimeSinceStartup = Time.realtimeSinceStartup;
			s_EngineTimeSinceStartup = Time.time;
		}

		private void Update()
		{
			UpdateRealTime();
			UpdateEngineTime(Time.deltaTime);
		}
		
        private void UpdateEngineTime(float fElapseTimes)
        {
            s_EngineDeltaTime = fElapseTimes;
            s_EngineTimeSinceStartup = Time.time;

            while (m_EngineTimeQueue.Count != 0)
            {
                AbsTimerData p;
                lock (m_QueueLock)
                    p = m_EngineTimeQueue.Peek();

                if (m_RemoveTimeList.Contains(p.TimerId))
                {
                    m_RemoveTimeList.Remove(p.TimerId);
                    lock (m_QueueLock)
                    {
                        m_EngineTimeQueue.Dequeue();
                        m_TimerDict.ForceRemove(p.TimerId);
                    }
                    continue;
                }
                if (!p.IsFrameAction)
                {
                    if (s_EngineTimeSinceStartup < p.NextTick)
                    {
                        break;
                    }

                    lock (m_QueueLock)
                    {
                        m_EngineTimeQueue.Dequeue();
                        m_TimerDict.ForceRemove(p.TimerId);
                    }
                    if (p.Valid)
                    {
                        if (p.Interval > 0)
                        {
                            p.NextTick += (ulong)p.Interval;
                            lock (m_QueueLock)
                                m_EngineTimeAddQueue.Enqueue(p);
                            p.DoAction(s_EngineDeltaTime);
                        }
                        else
                        {
                            p.DoAction(s_EngineDeltaTime);
                        }
                    }
                }
                else
                {
                    lock (m_QueueLock)
                    {
                        m_EngineTimeQueue.Dequeue();
                        m_TimerDict.ForceRemove(p.TimerId);

                        if (p.Valid)
                        {
                            m_EngineTimeAddQueue.Enqueue(p);
                            p.DoAction(s_EngineDeltaTime);
                        }
                    }
                }
            }

            while (m_EngineTimeAddQueue.Count != 0)
            {
                lock (m_QueueLock)
                {
                    AbsTimerData p = m_EngineTimeAddQueue.Dequeue();
                    m_EngineTimeQueue.Enqueue(p.TimerId, p, p.NextTick);
                    m_TimerDict.ForceAdd(p.TimerId, p);
                }
            }
        }

        private void UpdateRealTime()
        {
            float gameDuration = Time.realtimeSinceStartup;
            s_RealDeltaTime = gameDuration - s_RealTimeSinceStartup;
            s_RealTimeSinceStartup = gameDuration;
            while (m_RealTimeQueue.Count != 0)
            {
                AbsTimerData p;
                lock (m_QueueLock)
                    p = m_RealTimeQueue.Peek();

                if (m_RemoveTimeList.Contains(p.TimerId))
                {
                    m_RemoveTimeList.Remove(p.TimerId);
                    lock (m_QueueLock)
                    {
                        m_RealTimeQueue.Dequeue();
                        m_TimerDict.ForceRemove(p.TimerId);
                    }
                    continue;
                }

                if (!p.IsFrameAction)
                {
                    if (s_RealTimeSinceStartup < p.NextTick)
                    {
                        break;
                    }

                    lock (m_QueueLock)
                    {
                        m_RealTimeQueue.Dequeue();
                        m_TimerDict.ForceRemove(p.TimerId);
                    }
                    if (p.Valid)
                    {
                        if (p.Interval > 0)
                        {
                            p.NextTick += (ulong)p.Interval;
                            lock (m_QueueLock)
                                m_RealTimeAddQueue.Enqueue(p);
                            p.DoAction(s_RealDeltaTime);
                        }
                        else
                        {
                            p.DoAction(s_RealDeltaTime);
                        }
                    }
                }
                else
                {
                    lock (m_QueueLock)
                    {
                        m_RealTimeQueue.Dequeue();
                        m_TimerDict.ForceRemove(p.TimerId);
                        if (p.Valid)
                        {
                            m_RealTimeAddQueue.Enqueue(p);
                            p.DoAction(s_RealDeltaTime);
                        }
                    }
                }
            }

            while (m_RealTimeAddQueue.Count != 0)
            {
                lock (m_QueueLock)
                {
                    AbsTimerData p = m_RealTimeAddQueue.Dequeue();
                    m_RealTimeQueue.Enqueue(p.TimerId, p, p.NextTick);
                    m_TimerDict.ForceAdd(p.TimerId, p);
                }
            }
        }

		private void OnDestroy()
		{
			m_RealTimeQueue.Clear();
			m_RealTimeAddQueue.Clear();
			m_EngineTimeAddQueue.Clear();
			m_EngineTimeQueue.Clear();
			m_RemoveTimeList.Clear();
			m_TimerDict.Clear();
		}
    }
}