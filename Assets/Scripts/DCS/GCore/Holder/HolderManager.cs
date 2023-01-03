using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Galaxy.Entities;

namespace Galaxy.Data
{
    internal sealed partial class HolderManager : GalaxyModule
    {
        private Dictionary<uint, Holder> m_HolderCache;
        private HolderProxyPool m_HolderProxyPool;

        public HolderManager()
        {
            m_HolderCache = new Dictionary<uint, Holder>();
            m_HolderProxyPool = new HolderProxyPool(this);
        }

        internal override void Update(float elapseSeconds)
        {

        }

        internal override void Shutdown()
        {
            foreach (var item in m_HolderCache)
            {
                item.Value.Clear();
            }
            m_HolderCache.Clear();
            m_HolderProxyPool.ClearPool();
        }

        public Holder CreateHolder(uint id, SystemType[] systemTypes, CompType[] componentTypes, Dictionary<CompType, ComponentBase> serializedData)
        {
            return new Holder(id, systemTypes, componentTypes, serializedData);
        }

        public void AddHolder(uint id, Holder holder)
        {
            if (m_HolderCache.ContainsKey(id) || holder == null)
                throw new GalaxyException();

            m_HolderCache.Add(id, holder);
        }

        public void RemoveHolder(uint id)
        {
            if (!m_HolderCache.ContainsKey(id))
                throw new GalaxyException();

            m_HolderCache.Remove(id);
            m_HolderProxyPool.RemoveHolder(id);
        }

        public bool HasHolder(uint id)
        {
            return m_HolderCache.ContainsKey(id);
        }

        public Holder GetHolder(uint id)
        {
            return m_HolderCache.ContainsKey(id) ? m_HolderCache[id] : null;
        }

        public HolderProxy GetHolder(uint id, SystemTypeDefine systemDefine)
        {
            Debug.Assert(systemDefine != null);
            return GetHolder(id, systemDefine.m_SystemType, systemDefine.m_DependSystemTypes, systemDefine.m_RWCompTypes, systemDefine.m_ROCompTypes);
        }

        public HolderProxy GetHolder(uint id, HolderTypeDefine holderDefine)
        {
            Debug.Assert(holderDefine != null);
            return m_HolderProxyPool.GetHolder(id, holderDefine.m_DependSystemTypes, holderDefine.m_RWCompTypes, holderDefine.m_ROCompTypes);
        }

        public HolderProxy GetHolder(uint id, SystemType systemType, SystemType[] dependSystemTypes, CompType[] rwComponents, CompType[] roComponents)
        {
            Debug.Assert(m_HolderProxyPool != null);
            return m_HolderProxyPool.GetHolder(id, systemType, dependSystemTypes, rwComponents, roComponents);
        }

        public HolderProxy GetRoHolder(uint id, CompType[] accessAttr)
        {
            Debug.Assert(m_HolderProxyPool != null);
            return m_HolderProxyPool.GetHolder(id, new SystemType[] { }, new CompType[] { }, accessAttr);
        }

#if UNITY_EDITOR
        public Dictionary<uint, Holder> GetAllData()
        {
            return m_HolderCache;
        }
#endif
    }
}
