using Galaxy.Fsm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Entities
{
    public enum EntityStateType
    {
        Idle,
        Moving,
        Skill,
        Dead,

        Count,
    }

    /// <summary>
    /// 状态基类。
    /// </summary>
    public abstract class EntityBaseState : FsmState<EntityFsm>
    {
        /// <summary>
        /// 状态初始化时调用。
        /// </summary>
        /// <param name="fsmOwner">流程持有者。</param>
        protected internal override void OnInit(IFsm<EntityFsm> fsmOwner)
        {
            base.OnInit(fsmOwner);
        }

        /// <summary>
        /// 进入状态时调用。
        /// </summary>
        /// <param name="fsmOwner">流程持有者。</param>
        /// <param name="enterParams">初始化参数。</param>
        protected internal override void OnEnter(IFsm<EntityFsm> fsmOwner, FsmEnterParams enterParams = null)
        {
            var holder = GetHolder(fsmOwner);
            Debug.Assert(holder != null);
            //holder.GetComponent<State.StateComponent>().CurrentState = (uint)GetStateType();
            base.OnEnter(fsmOwner);
        }

        /// <summary>
        /// 状态轮询时调用。
        /// </summary>
        /// <param name="fsmOwner">流程持有者。</param>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        protected internal override void OnUpdate(IFsm<EntityFsm> fsmOwner, float elapseSeconds)
        {
            base.OnUpdate(fsmOwner, elapseSeconds);
        }

        /// <summary>
        /// 离开状态时调用。
        /// </summary>
        /// <param name="fsmOwner">流程持有者。</param>
        /// <param name="isShutdown">是否是关闭状态机时触发。</param>
        protected internal override void OnLeave(IFsm<EntityFsm> fsmOwner, bool isShutdown)
        {
            base.OnLeave(fsmOwner, isShutdown);
        }

        /// <summary>
        /// 状态销毁时调用。
        /// </summary>
        /// <param name="fsmOwner">流程持有者。</param>
        protected internal override void OnDestroy(IFsm<EntityFsm> fsmOwner)
        {
            base.OnDestroy(fsmOwner);
        }

        protected abstract EntityStateType GetStateType();

        #region dcs extend

        protected Data.Holder GetHolder(IFsm<EntityFsm> fsmOwner)
        {
            return fsmOwner.Owner.GetOwner().GetHolderForFsm();
        }

        public uint GetVisualPid(IFsm<EntityFsm> fsmOwner)
        {
            var roHolder = GalaxyEntry.GetModule<Data.HolderManager>().GetRoHolder(fsmOwner.Owner.GetOwner().UID, new CompType[] { CompType.Visual });
            return roHolder.GetComponent<Visual.VisualComponentProxy>().VisualPid;
        }
        #endregion
    }

    /// <summary>
    /// Entity状态机
    /// </summary>
    public abstract class EntityFsm
    {
        private Entity m_Owner;
        private IFsmManager m_FsmManager;
        private IFsm<EntityFsm> m_EntityFsm;

        public EntityFsm(Entity owner)
        {
            Debug.Assert(owner != null);
            m_Owner = owner;
            m_FsmManager = GalaxyEntry.GetModule<FsmManager>();
            Debug.Assert(m_FsmManager != null);
        }

        public virtual void Init()
        {
            m_EntityFsm = m_FsmManager.CreateFsm(m_Owner.UID.ToString(), this, GetEntityAllStates());
            m_EntityFsm.Start(GetDefaultState());

            GalaxyEntry.GetModule<Dots.EcsAdapterModule>().AddEcsEntityQuery(Dots.EcsQueryType.EQ_EntityState, m_Owner.UID);
        }

        public virtual void Destroy()
        {
            GalaxyEntry.GetModule<Dots.EcsAdapterModule>().RemoveEcsEntityQuery(Dots.EcsQueryType.EQ_EntityState, m_Owner.UID);

            if (m_EntityFsm != null)
            {
                m_FsmManager.DestroyFsm(m_EntityFsm);
                m_EntityFsm = null;
            }
            m_EntityFsm = null;
        }

        public abstract EntityBaseState[] GetEntityAllStates();
        public abstract Type GetDefaultState();
        public IFsm<EntityFsm> GetFsm() => m_EntityFsm;

        public FsmState<EntityFsm> GetCurrentState() => m_EntityFsm.CurrentState;
        public Dictionary<string, Variable> GetAllFsmDatas => m_EntityFsm.GetAllData();

        public Entity GetOwner() => m_Owner;
    }
}
