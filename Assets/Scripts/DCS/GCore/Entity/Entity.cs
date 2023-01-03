using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Galaxy.Data;
using Galaxy.Fsm;

namespace Galaxy.Entities
{
    /// <summary>
    /// ECS�����������OOP������ECS���󽻽�
    /// </summary>
    public class ECSProxy : IReference
    {
        private uint m_Uid = CoreConst.INVAILD_UID;

        public ECSProxy(uint id)
        {
            m_Uid = id;
        }

        public void Clear()
        {
            m_Uid = CoreConst.INVAILD_UID;
        }

        public uint UID => m_Uid;
    }

    public enum EntityState
    {
        Default,
        Queue,                  // ��ʼ���Ŷ���
        Init,                   // ��ʼ����
        Run,                    // ������
        Destroy,                // ����
    }

    /// <summary>
    /// ��Ϸ�߼���������
    /// 1. ��֧�ֶ�̬������
    /// 2. �½�Entityʱ��Ҫ��EntityDefine�ļ���������ض���
    /// 3. Entity�����е�component����Ҫ��OOP���͵�
    /// </summary>
    public abstract class Entity : IReference
    {
        private uint m_Uid = CoreConst.INVAILD_UID;
        private ECSProxy m_EcsProxy;
        protected EntityState m_EntityState;
        protected List<SystemType> m_Systems;
        protected EntityFsm m_EntityFsm;

        public Entity(uint id, SystemType[] systems)
        {
            m_Uid = id;
            m_EcsProxy = new ECSProxy(id);
            m_EntityState = EntityState.Default;
            m_Systems = new List<SystemType>(systems);
        }

        public void Clear()
        {
            m_Systems.Clear();
            m_EntityState = EntityState.Default;
            m_EcsProxy = null;
            m_Uid = CoreConst.INVAILD_UID;
        }

        public void Init(EntityTypeDefine entityDefine, Dictionary<CompType, ComponentBase> serializedData)
        {
            //������λ�����ݴ���ֵ��ֵ����Ӧ��component
            Debug.Assert(entityDefine != null);
            HolderManager manager = GalaxyEntry.GetModule<HolderManager>();
            Holder holder = manager.CreateHolder(m_Uid, entityDefine.m_SystemTypes, entityDefine.m_CompTypes, serializedData);
            manager.AddHolder(m_Uid, holder);

            m_EntityState = EntityState.Run;
        }

        public void InitSystems()
        {
            //��������system��ʼ����������
            if (IsDestroy())
                return;

            GalaxySystemManager manager = GalaxyEntry.GetModule<GalaxySystemManager>();
            foreach (SystemType systemType in m_Systems)
            {
                GalaxySystem system = manager.GetSystem(systemType);
                system.Init(GetHolder(system));
            }
        }

        public void Initialize()
        {
            //��ʼ���������
            if (IsDestroy())
                return;

            GalaxySystemManager manager = GalaxyEntry.GetModule<GalaxySystemManager>();
            foreach (SystemType systemType in m_Systems)
            {
                GalaxySystem system = manager.GetSystem(systemType);
                manager.GetSystem(systemType).Initialize(GetHolder(system));
            }

            InitializeFsm();
        }

        public void Update(float elapseSeconds)
        {
            //��ѯ��ǰ����
            if (IsDestroy())
                return;

            UpdateFsm();

            GalaxySystemManager manager = GalaxyEntry.GetModule<GalaxySystemManager>();
            foreach (SystemType systemType in m_Systems)
            {
                GalaxySystem system = manager.GetSystem(systemType);
                manager.GetSystem(systemType).Update(GetHolder(system), elapseSeconds);
            }
        }

        public void PostUpdate(float elapseSeconds)
        {
            //��ѯ��ǰ����
            if (IsDestroy())
                return;

            GalaxySystemManager manager = GalaxyEntry.GetModule<GalaxySystemManager>();
            foreach (SystemType systemType in m_Systems)
            {
                GalaxySystem system = manager.GetSystem(systemType);
                manager.GetSystem(systemType).PostUpdate(GetHolder(system), elapseSeconds);
            }
        }

        public void FixedUpdate(float elapseSeconds)
        {
            //��ѯ��ǰ����
            if (IsDestroy())
                return;

            GalaxySystemManager manager = GalaxyEntry.GetModule<GalaxySystemManager>();
            foreach (SystemType systemType in m_Systems)
            {
                GalaxySystem system = manager.GetSystem(systemType);
                manager.GetSystem(systemType).FixedUpdate(GetHolder(system), elapseSeconds);
            }
        }

        public void PreDestroy()
        {
            //�Ƴ�����֮ǰ
            if (IsDestroy())
                return;

            DestroyFsm();

            GalaxySystemManager manager = GalaxyEntry.GetModule<GalaxySystemManager>();
            foreach (SystemType systemType in m_Systems)
            {
                GalaxySystem system = manager.GetSystem(systemType);
                manager.GetSystem(systemType).PreDestroy(GetHolder(system));
            }
        }

        public void Destroy()
        {
            //�Ƴ�����
            if (IsDestroy())
                return;

            GalaxySystemManager manager = GalaxyEntry.GetModule<GalaxySystemManager>();
            foreach (SystemType systemType in m_Systems)
            {
                GalaxySystem system = manager.GetSystem(systemType);
                manager.GetSystem(systemType).Destroy(GetHolder(system));
            }

            GalaxyEntry.GetModule<HolderManager>().RemoveHolder(m_Uid);
            TryDestroyLogicGameObject();
        }

        public void ReloadScript()
        {
            //�ȸ��£��滻�����б��ص���Դ�ļ�(nodegraph���ݡ�lua��py��)
        }

        private HolderProxy GetHolder(GalaxySystem system)
        {
            return GalaxyEntry.GetModule<HolderManager>().GetHolder(UID, system.GetSystemDefine());
        }

        public uint UID => m_Uid;
        public ECSProxy GetEcsProxy => m_EcsProxy;

        public bool IsQueue()
        {
            return m_EntityState == EntityState.Queue;
        }
        public bool IsInit()
        {
            return m_EntityState == EntityState.Init;
        }
        public bool IsRun()
        {
            return m_EntityState == EntityState.Run;
        }
        public bool IsDestroy()
        {
            return m_EntityState == EntityState.Destroy;
        }

        public abstract EntityType GetEntityType();

        protected abstract EntityFsm CreateEntityFsm();
        protected void InitializeFsm()
        {
            m_EntityFsm = CreateEntityFsm();
            //TODO Randall ������Ҫ�ṩ�����������뷽��
            m_EntityFsm.Init();
        }

        protected void UpdateFsm()
        {
        }

        protected void DestroyFsm()
        {
            m_EntityFsm.Destroy();
            m_EntityFsm = null;
        }

        public Holder GetHolderForFsm()
        {
            return GalaxyEntry.GetModule<HolderManager>().GetHolder(UID);
        }

        public void FireFsmEvent(int eventId, FsmEventParams fsmEvent)
        {
            m_EntityFsm.GetFsm().FireEvent(this, eventId, fsmEvent);
        }

        public void ChangeFsmState<TState>(FsmEnterParams enterParams = null) where TState : FsmState<EntityFsm>
        {
            m_EntityFsm.GetFsm().ChangeState<TState>(enterParams);
        }

        #region ForUnity
        private GameObject m_LogicObject;

        protected void TryInitLogicGameObject()
        {
            m_LogicObject = new GameObject();
            m_LogicObject.SetActive(true);
#if UNITY_EDITOR
            m_LogicObject.name = Utility.Text.Format("Entity_{0}", UID);
#endif
        }

        protected void TryDestroyLogicGameObject()
        {
            if (m_LogicObject == null)
            {
                return;
            }

            Object.DestroyImmediate(m_LogicObject);
        }

        public GameObject GetLogicObject()
        {
            //�ӳٴ���
            if (m_LogicObject == null)
            {
                TryInitLogicGameObject();
            }

            return m_LogicObject;
        }
        #endregion
    }
}
