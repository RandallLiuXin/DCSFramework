using System.Collections.Generic;
using Galaxy.Fsm;
using Galaxy.Task;
using Galaxy.Event;
using ProcedureOwner = Galaxy.Fsm.IFsm<Galaxy.Procedure.IProcedureManager>;

namespace Galaxy.Procedure
{
    /// <summary>
    /// 流程加载任务
    /// </summary>
    public class ProcedureTask : TaskBase
    {
        /// <summary>
        /// 任务完成时调用。
        /// </summary>
        /// <param name="reason">任务完成的原因。</param>
        protected internal override void OnComplete(string reason)
        {
            base.OnComplete(reason);

            SendLoadingProcessEvent();

        }

        /// <summary>
        /// 任务失败时调用。
        /// </summary>
        /// <param name="reason">任务失败的原因。</param>
        protected internal override void OnFailure(string reason)
        {
            base.OnFailure(reason);

            SendLoadingProcessEvent();
        }

        /// <summary>
        /// 任务取消时调用。
        /// </summary>
        /// <param name="reason">任务取消的原因。</param>
        protected internal override void OnCancel(string reason)
        {
            base.OnCancel(reason);

            SendLoadingProcessEvent();
        }

        private void SendLoadingProcessEvent()
        {
            UIUpdateLoadingProgress uiEvent = new UIUpdateLoadingProgress(1);
            GalaxyEntry.GetModule<EventManager>().Fire(this, uiEvent);
        }
    }

    public class ProcedureLoadingCache<TState> : ProcedureTask where TState : FsmState<IProcedureManager>
    {
        public ProcedureLoadingCache(ProcedureOwner procedureOwner)
        {
            m_procedureOwner = procedureOwner;
        }

        /// <summary>
        /// 任务完成时调用。
        /// </summary>
        /// <param name="reason">任务完成的原因。</param>
        protected internal override void OnComplete(string reason)
        {
            base.OnComplete(reason);
            m_procedureOwner.ChangeState<TState>();
        }

        /// <summary>
        /// 任务失败时调用。
        /// </summary>
        /// <param name="reason">任务失败的原因。</param>
        protected internal override void OnFailure(string reason)
        {
            base.OnFailure(reason);
        }

        /// <summary>
        /// 任务取消时调用。
        /// </summary>
        /// <param name="reason">任务取消的原因。</param>
        protected internal override void OnCancel(string reason)
        {
            base.OnCancel(reason);
        }

        private ProcedureOwner m_procedureOwner;
    }

    /// <summary>
    /// 流程基类。
    /// </summary>
    public abstract class ProcedureBase : FsmState<IProcedureManager>
    {
        /// <summary>
        /// 状态初始化时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        protected internal override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        /// <summary>
        /// 进入状态时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        /// <param name="enterParams">初始化参数。</param>
        protected internal override void OnEnter(ProcedureOwner procedureOwner, FsmEnterParams enterParams = null)
        {
            base.OnEnter(procedureOwner);
        }

        /// <summary>
        /// 状态轮询时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        protected internal override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds);
        }

        /// <summary>
        /// 离开状态时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        /// <param name="isShutdown">是否是关闭状态机时触发。</param>
        protected internal override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }

        /// <summary>
        /// 状态销毁时调用。
        /// </summary>
        /// <param name="procedureOwner">流程持有者。</param>
        protected internal override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }

        protected void RequireLoadingTask(ref List<ProcedureTask> tasks)
        {
        }

        protected void ChangeProcedure<TState>(ProcedureOwner procedureOwner) where TState : FsmState<IProcedureManager>
        {
            List<ProcedureTask> tasks = new List<ProcedureTask>();
            RequireLoadingTask(ref tasks);

            tasks.Add(new ProcedureLoadingCache<TState>(procedureOwner));
            ChangeState<ProcedureLoading>(procedureOwner, new ProcedureLoadingEnterParams(tasks));
        }
    }
}
