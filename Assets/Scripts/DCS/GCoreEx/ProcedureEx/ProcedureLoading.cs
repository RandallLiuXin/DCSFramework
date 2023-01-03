using Galaxy.Fsm;
using System.Collections.Generic;
using ProcedureOwner = Galaxy.Fsm.IFsm<Galaxy.Procedure.IProcedureManager>;

namespace Galaxy.Procedure
{
    public class ProcedureLoadingEnterParams : FsmEnterParams
    {
        public ProcedureLoadingEnterParams(List<ProcedureTask> tasks)
        {
            m_procedureTasks = tasks;
        }

        public override void Clear()
        {
            m_procedureTasks.Clear();
        }

        private List<ProcedureTask> m_procedureTasks;
    }

    /// <summary>
    /// 通用加载流程
    /// </summary>
    public class ProcedureLoading : ProcedureBase
    {
        protected internal override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected internal override void OnEnter(ProcedureOwner procedureOwner, FsmEnterParams enterParams = null)
        {
            base.OnEnter(procedureOwner);
        }

        protected internal override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds);
        }

        protected internal override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            base.OnLeave(procedureOwner, isShutdown);
        }

        protected internal override void OnDestroy(ProcedureOwner procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }
    }
}
