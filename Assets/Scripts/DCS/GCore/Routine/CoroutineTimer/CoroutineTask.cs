using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Timer
{
    public class CoroutineTask
    {
        CoroutineTaskManager.Task m_Task;
        public bool IsRunning
        {
            get
            {
                return m_Task.IsRunning;
            }
        }

        public bool IsPaused
        {
            get
            {
                return m_Task.IsPaused;
            }
        }

        public bool IsStopped
        {
            get
            {
                return m_Task.IsStopped;
            }
        }

        public CoroutineTask(object reciever, IEnumerator c, GalaxyAction<bool> finished, bool autoStart = false)
        {
            m_Task = CoroutineTaskManager.CreateTask(reciever, c);
            m_Task.Finished = finished;
            if (autoStart)
            {
                m_Task.Start();
            }
        }

        public void Start()
        {
            m_Task.Start();
        }

        public void Stop()
        {
            m_Task.Stop();
        }

        public void Pause()
        {
            m_Task.Pause();
        }

        public void Unpause()
        {
            m_Task.Unpause();
        }
    }
}
