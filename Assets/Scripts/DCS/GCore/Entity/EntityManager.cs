using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Galaxy.Entities
{
    public sealed class EntityTypeDefine
    {
        public EntityType m_EntityType;
        public SystemType[] m_SystemTypes;
        public CompType[] m_CompTypes;
    }

    internal sealed partial class EntityManager : GalaxyModule
    {
        private static Dictionary<EntityType, EntityTypeDefine> ms_EntityType2AllComponentMap;

        private Dictionary<uint, Entity> m_EntityPool;
        private Dictionary<EntityType, List<Entity>> m_TypeInstanceMap;

        internal override bool NeedFixedUpdate => true;

        /// <summary>
        /// 初始化流程管理器的新实例。
        /// </summary>
        public EntityManager()
        {
            m_EntityPool = new Dictionary<uint, Entity>();
            m_TypeInstanceMap = new Dictionary<EntityType, List<Entity>>();
            CollectEntityTypeDefine();
        }

        public void CollectEntityTypeDefine()
        {
            if (ms_EntityType2AllComponentMap == null)
                ms_EntityType2AllComponentMap = new Dictionary<EntityType, EntityTypeDefine>();

            //这个方法中具体收集了相关参数(定义在EntityDefine中)
            InternalCollectEntityTypeDefine();
        }

        /// <summary>
        /// 实体管理器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        internal override void Update(float elapseSeconds)
        {
            List<uint> removeList = new List<uint>();
            foreach (var item in m_EntityPool)
            {
                item.Value.Update(elapseSeconds);
            }

            foreach (var item in m_EntityPool)
            {
                item.Value.PostUpdate(elapseSeconds);
                if (item.Value.IsDestroy())
                    removeList.Add(item.Key);
            }

            foreach (var item in removeList)
            {
                DestroyEntityInternal(item);
                RemoveEntity(item);
            }
            removeList.Clear();
        }

        internal override void FixedUpdate(float elapseSeconds)
        {
            foreach (var item in m_EntityPool)
            {
                item.Value.FixedUpdate(elapseSeconds);
            }
        }

        /// <summary>
        /// 关闭并清理实体管理器。
        /// </summary>
        internal override void Shutdown()
        {
            List<uint> removeList = new List<uint>(m_EntityPool.Keys);
            foreach (var item in removeList)
            {
                DestroyEntityInternal(item);
                RemoveEntity(item);
            }
        }

        public Entity CreateEntity(EntityType entityType, uint configId)
        {
            return CreateEntity(GalaxyEntry.GetModule<Common.UidGenerator>().GenerateUid(Common.UidRequireType.Entity), entityType, EntityHelper.GetEntityConfigData(configId));
        }

        public Entity CreateEntity(EntityType entityType, Dictionary<CompType, ComponentBase> serializedData)
        {
            return CreateEntity(GalaxyEntry.GetModule<Common.UidGenerator>().GenerateUid(Common.UidRequireType.Entity), entityType, serializedData);
        }

        public Entity CreateEntity(uint id, EntityType entityType, Dictionary<CompType, ComponentBase> serializedData)
        {
            if (HasEntity(id))
            {
                Entity entity = GetEntity(id);
                if (entity.IsDestroy())
                {
                    RemoveEntity(id);
                }
                else
                {
                    Debug.Assert(false);
                }
            }

            Debug.Assert(ms_EntityType2AllComponentMap.ContainsKey(entityType));
            EntityTypeDefine entityDefine = ms_EntityType2AllComponentMap[entityType];
            Entity newEntity = EntityHelper.CreateInstance(entityType, id, entityDefine.m_SystemTypes);
            Debug.Assert(newEntity != null);
            newEntity.Init(entityDefine, serializedData);

            newEntity.InitSystems();

            //数据初始化完成后即可加入管理器
            AddEntity(newEntity);

            newEntity.Initialize();

            return newEntity;
        }

        public void DestroyEntity(uint id)
        {
            DestroyEntity(GetEntity(id));
        }

        public void DestroyEntity(Entity entity)
        {
            DestroyEntityInternal(entity);
            RemoveEntity(entity);
        }

        private void DestroyEntityInternal(uint id)
        {
            DestroyEntityInternal(GetEntity(id));
        }

        private void DestroyEntityInternal(Entity entity)
        {
            if (entity == null || entity.IsDestroy())
                return;
            entity.PreDestroy();
            entity.Destroy();
        }

        private void AddEntity(Entity entity)
        {
            Debug.Assert(entity != null);
            Debug.Assert(entity.UID != CoreConst.INVAILD_UID);
            Debug.Assert(entity.IsRun());
            m_EntityPool.Add(entity.UID, entity);

            EntityType entityType = entity.GetEntityType();
            if (!m_TypeInstanceMap.ContainsKey(entityType))
                m_TypeInstanceMap.Add(entityType, new List<Entity>());
            m_TypeInstanceMap[entityType].Add(entity);
        }

        private void RemoveEntity(uint id)
        {
            RemoveEntity(GetEntity(id));
        }

        private void RemoveEntity(Entity entity)
        {
            if (entity == null)
                return;
            EntityType entityType = entity.GetEntityType();
            Debug.Assert(m_TypeInstanceMap.ContainsKey(entityType));
            Debug.Assert(m_TypeInstanceMap[entityType].Contains(entity));
            m_TypeInstanceMap[entityType].Remove(entity);
            m_EntityPool.Remove(entity.UID);
        }

        public bool HasEntity(uint id)
        {
            return m_EntityPool.ContainsKey(id);
        }

        public Entity GetEntity(uint id)
        {
            return m_EntityPool.ContainsKey(id) ? m_EntityPool[id] : null;
        }

        public uint GetSingletonUid(EntityType entityType)
        {
            Debug.Assert(m_TypeInstanceMap != null);
            if (!m_TypeInstanceMap.ContainsKey(entityType))
                return CoreConst.INVAILD_UID;

            return m_TypeInstanceMap[entityType].Count == 1 ? m_TypeInstanceMap[entityType][0].UID : CoreConst.INVAILD_UID;
        }
    }
}
