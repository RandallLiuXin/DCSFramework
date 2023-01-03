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
    /// ״̬���ࡣ
    /// </summary>
    public abstract class EntityBaseState : FsmState<EntityFsm>
    {
        /// <summary>
        /// ״̬��ʼ��ʱ���á�
        /// </summary>
        /// <param name="fsmOwner">���̳����ߡ�</param>
        protected internal override void OnInit(IFsm<EntityFsm> fsmOwner)
        {
            base.OnInit(fsmOwner);
        }

        /// <summary>
        /// ����״̬ʱ���á�
        /// </summary>
        /// <param name="fsmOwner">���̳����ߡ�</param>
        /// <param name="enterParams">��ʼ��������</param>
        protected internal override void OnEnter(IFsm<EntityFsm> fsmOwner, FsmEnterParams enterParams = null)
        {
            var holder = GetHolder(fsmOwner);
            Debug.Assert(holder != null);
            //holder.GetComponent<State.StateComponent>().CurrentState = (uint)GetStateType();
            base.OnEnter(fsmOwner);
        }

        /// <summary>
        /// ״̬��ѯʱ���á�
        /// </summary>
        /// <param name="fsmOwner">���̳����ߡ�</param>
        /// <param name="elapseSeconds">�߼�����ʱ�䣬����Ϊ��λ��</param>
        protected internal override void OnUpdate(IFsm<EntityFsm> fsmOwner, float elapseSeconds)
        {
            base.OnUpdate(fsmOwner, elapseSeconds);
        }

        /// <summary>
        /// �뿪״̬ʱ���á�
        /// </summary>
        /// <param name="fsmOwner">���̳����ߡ�</param>
        /// <param name="isShutdown">�Ƿ��ǹر�״̬��ʱ������</param>
        protected internal override void OnLeave(IFsm<EntityFsm> fsmOwner, bool isShutdown)
        {
            base.OnLeave(fsmOwner, isShutdown);
        }

        /// <summary>
        /// ״̬����ʱ���á�
        /// </summary>
        /// <param name="fsmOwner">���̳����ߡ�</param>
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
    /// Entity״̬��
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
