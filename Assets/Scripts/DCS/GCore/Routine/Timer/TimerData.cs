using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Galaxy.Timer
{
    public class TimerData : AbsTimerData
    {
        private Action<float> m_Action;

        public override Delegate Action
        {
            get { return m_Action; }
            set { m_Action = value as Action<float>; }
        }

        public override void DoAction(float realDeltaTime)
        {
            base.DoAction(realDeltaTime);
            if (m_Action != null)
                m_Action(realDeltaTime);
        }
    }

    public class TimerData<T> : AbsTimerData
    {
        private Action<float, T> m_action;

        public override Delegate Action
        {
            get { return m_action; }
            set { m_action = value as Action<float, T>; }
        }

        private T m_Arg1;

        public T Arg1
        {
            get { return m_Arg1; }
            set { m_Arg1 = value; }
        }

        public override void DoAction(float realDeltaTime)
        {
            base.DoAction(realDeltaTime);
            m_action(realDeltaTime, m_Arg1);
        }
    }

    public class TimerData<T, U> : AbsTimerData
    {
        private Action<float, T, U> m_Action;

        public override Delegate Action
        {
            get { return m_Action; }
            set { m_Action = value as Action<float, T, U>; }
        }

        private T m_Arg1;

        public T Arg1
        {
            get { return m_Arg1; }
            set { m_Arg1 = value; }
        }

        private U m_Arg2;

        public U Arg2
        {
            get { return m_Arg2; }
            set { m_Arg2 = value; }
        }

        public override void DoAction(float realDeltaTime)
        {
            base.DoAction(realDeltaTime);
            m_Action(realDeltaTime, m_Arg1, m_Arg2);
        }
    }

    public class TimerData<T, U, V> : AbsTimerData
    {
        private Action<float, T, U, V> m_Action;

        public override Delegate Action
        {
            get { return m_Action; }
            set { m_Action = value as Action<float, T, U, V>; }
        }

        private T m_Arg1;

        public T Arg1
        {
            get { return m_Arg1; }
            set { m_Arg1 = value; }
        }

        private U m_Arg2;

        public U Arg2
        {
            get { return m_Arg2; }
            set { m_Arg2 = value; }
        }

        private V m_Arg3;

        public V Arg3
        {
            get { return m_Arg3; }
            set { m_Arg3 = value; }
        }

        public override void DoAction(float realDeltaTime)
        {
            base.DoAction(realDeltaTime);
            m_Action(realDeltaTime, m_Arg1, m_Arg2, m_Arg3);
        }
    }
}
