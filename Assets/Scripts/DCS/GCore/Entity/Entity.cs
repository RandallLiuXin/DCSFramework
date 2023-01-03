using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Galaxy.Data;
using Galaxy.Fsm;

namespace Galaxy.Entities
{
    /// <summary>
    /// ECS代理对象，用于OOP对象与ECS对象交接
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
        Queue,                  // 初始化排队中
        Init,                   // 初始化中
        Run,                    // 运行中
        Destroy,                // 销毁
    }

    /// <summary>
    /// 游戏逻辑基础对象
    /// 1. 不支持动态添加组件
    /// 2. 新建Entity时需要在EntityDefine文件中完善相关定义
    /// 3. Entity中所有的component都需要是OOP类型的
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
            //创建单位并根据传入值赋值给对应的component
            Debug.Assert(entityDefine != null);
            HolderManager manager = GalaxyEntry.GetModule<HolderManager>();
            Holder holder = manager.CreateHolder(m_Uid, entityDefine.m_SystemTypes, entityDefine.m_CompTypes, serializedData);
            manager.AddHolder(m_Uid, holder);

            m_EntityState = EntityState.Run;
        }

        public void InitSystems()
        {
            //调用所有system初始化自身数据
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
            //初始化对象完成
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
            //轮询当前对象
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
            //轮询当前对象
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
            //轮询当前对象
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
            //移除对象之前
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
            //移除对象
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
            //热更新，替换掉所有本地的资源文件(nodegraph数据、lua、py等)
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
            //TODO Randall 这里需要提供断线重连进入方法
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
            //延迟创建
            if (m_LogicObject == null)
            {
                TryInitLogicGameObject();
            }

            return m_LogicObject;
        }
        #endregion
    }
}
