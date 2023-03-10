using System.Collections.Generic;

namespace Galaxy
{
    /// <summary>
    /// 任务池。
    /// </summary>
    /// <typeparam name="T">任务类型。</typeparam>
    internal sealed class TaskPool<T> where T : ITask
    {
        private readonly Stack<ITaskAgent<T>> m_FreeAgents;
        private readonly LinkedList<ITaskAgent<T>> m_WorkingAgents;
        private readonly LinkedList<T> m_WaitingTasks;

        /// <summary>
        /// 初始化任务池的新实例。
        /// </summary>
        public TaskPool()
        {
            m_FreeAgents = new Stack<ITaskAgent<T>>();
            m_WorkingAgents = new LinkedList<ITaskAgent<T>>();
            m_WaitingTasks = new LinkedList<T>();
        }

        /// <summary>
        /// 获取任务代理总数量。
        /// </summary>
        public int TotalAgentCount
        {
            get
            {
                return FreeAgentCount + WorkingAgentCount;
            }
        }

        /// <summary>
        /// 获取可用任务代理数量。
        /// </summary>
        public int FreeAgentCount
        {
            get
            {
                return m_FreeAgents.Count;
            }
        }

        /// <summary>
        /// 获取工作中任务代理数量。
        /// </summary>
        public int WorkingAgentCount
        {
            get
            {
                return m_WorkingAgents.Count;
            }
        }

        /// <summary>
        /// 获取等待任务数量。
        /// </summary>
        public int WaitingTaskCount
        {
            get
            {
                return m_WaitingTasks.Count;
            }
        }

        /// <summary>
        /// 任务池轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds)
        {
            ProcessRunningTasks(elapseSeconds);
            ProcessWaitingTasks(elapseSeconds);
        }

        /// <summary>
        /// 关闭并清理任务池。
        /// </summary>
        public void Shutdown()
        {
            while (FreeAgentCount > 0)
            {
                m_FreeAgents.Pop().Shutdown();
            }

            foreach (ITaskAgent<T> workingAgent in m_WorkingAgents)
            {
                workingAgent.Shutdown();
            }
            m_WorkingAgents.Clear();

            m_WaitingTasks.Clear();
        }

        /// <summary>
        /// 增加任务代理。
        /// </summary>
        /// <param name="agent">要增加的任务代理。</param>
        public void AddAgent(ITaskAgent<T> agent)
        {
            if (agent == null)
            {
                throw new GalaxyException("Task agent is invalid.");
            }

            agent.Initialize();
            m_FreeAgents.Push(agent);
        }

        /// <summary>
        /// 增加任务。
        /// </summary>
        /// <param name="task">要增加的任务。</param>
        public void AddTask(T task)
        {
            LinkedListNode<T> current = m_WaitingTasks.Last;
            while (current != null)
            {
                if (task.Priority <= current.Value.Priority)
                {
                    break;
                }

                current = current.Previous;
            }

            if (current != null)
            {
                m_WaitingTasks.AddAfter(current, task);
            }
            else
            {
                m_WaitingTasks.AddFirst(task);
            }
        }

        /// <summary>
        /// 移除任务。
        /// </summary>
        /// <param name="serialId">要移除任务的序列编号。</param>
        /// <returns>被移除的任务。</returns>
        public T RemoveTask(int serialId)
        {
            foreach (T waitingTask in m_WaitingTasks)
            {
                if (waitingTask.SerialId == serialId)
                {
                    m_WaitingTasks.Remove(waitingTask);
                    return waitingTask;
                }
            }

            foreach (ITaskAgent<T> workingAgent in m_WorkingAgents)
            {
                if (workingAgent.Task.SerialId == serialId)
                {
                    workingAgent.Reset();
                    m_FreeAgents.Push(workingAgent);
                    m_WorkingAgents.Remove(workingAgent);
                    return workingAgent.Task;
                }
            }

            return default(T);
        }

        /// <summary>
        /// 移除所有任务。
        /// </summary>
        public void RemoveAllTasks()
        {
            m_WaitingTasks.Clear();
            foreach (ITaskAgent<T> workingAgent in m_WorkingAgents)
            {
                workingAgent.Reset();
                m_FreeAgents.Push(workingAgent);
            }
            m_WorkingAgents.Clear();
        }

        private void ProcessRunningTasks(float elapseSeconds)
        {
            LinkedListNode<ITaskAgent<T>> current = m_WorkingAgents.First;
            while (current != null)
            {
                if (!current.Value.Task.Done)
                {
                    current.Value.Update(elapseSeconds);
                    current = current.Next;
                    continue;
                }

                LinkedListNode<ITaskAgent<T>> next = current.Next;
                current.Value.Reset();
                m_FreeAgents.Push(current.Value);
                m_WorkingAgents.Remove(current);
                current = next;
            }
        }

        private void ProcessWaitingTasks(float elapseSeconds)
        {
            LinkedListNode<T> current = m_WaitingTasks.First;
            while (current != null && FreeAgentCount > 0)
            {
                ITaskAgent<T> agent = m_FreeAgents.Pop();
                LinkedListNode<ITaskAgent<T>> agentNode = m_WorkingAgents.AddLast(agent);
                LinkedListNode<T> next = current.Next;
                StartTaskStatus status = agent.Start(current.Value);
                if (status == StartTaskStatus.Done || status == StartTaskStatus.HasToWait || status == StartTaskStatus.UnknownError)
                {
                    agent.Reset();
                    m_FreeAgents.Push(agent);
                    m_WorkingAgents.Remove(agentNode);
                }

                if (status == StartTaskStatus.Done || status == StartTaskStatus.CanResume || status == StartTaskStatus.UnknownError)
                {
                    m_WaitingTasks.Remove(current);
                }

                current = next;
            }
        }
    }
}
