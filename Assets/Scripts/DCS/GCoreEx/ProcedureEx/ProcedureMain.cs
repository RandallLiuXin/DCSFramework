using Galaxy.Fsm;
using ProcedureOwner = Galaxy.Fsm.IFsm<Galaxy.Procedure.IProcedureManager>;

namespace Galaxy.Procedure
{
    /// <summary>
    /// 游戏运行时逻辑
    /// </summary>
    public class ProcedureMain : ProcedureBase
    {
        protected internal override void OnInit(ProcedureOwner procedureOwner)
        {
            base.OnInit(procedureOwner);
            // 初始化GameMode
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
