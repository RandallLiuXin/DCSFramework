using System;
using System.Collections.Generic;
using UnityEngine;
using Galaxy.Entities;

namespace Galaxy.Data
{
    using HolderKey = UInt32;
    using SystemKey = UInt32;
    using CompKey = UInt32;

    internal sealed class HolderProxyPool
    {
        // holder uid, <key: <systems, rwComps, roComps>, value: holder proxy>
        private Dictionary<HolderKey, Dictionary<Tuple<SystemKey, CompKey, CompKey>, HolderProxy>> m_Pool;
        private HolderManager m_Owner;

        public HolderProxyPool(HolderManager owner)
        {
            m_Owner = owner;
            m_Pool = new Dictionary<HolderKey, Dictionary<Tuple<SystemKey, CompKey, CompKey>, HolderProxy>>();

            Debug.Assert((uint)SystemType.Count < 32);
            Debug.Assert((uint)CompType.Count < 32);
        }

        public bool HasHolder(HolderKey id)
        {
            return m_Pool.ContainsKey(id);
        }

        public void RemoveHolder(HolderKey id)
        {
            if (!m_Pool.ContainsKey(id))
                return;
            var dict = m_Pool[id];
            dict.Clear();
            m_Pool.Remove(id);
        }

        public void ClearPool()
        {
            foreach (var item in m_Pool)
            {
                item.Value.Clear();
            }
            m_Pool.Clear();
        }

        private SystemKey GetUniqueKey(SystemType[] systemTypes)
        {
            if (GalaxyEntry.ms_GameIsRelease)
            {
                return 0;
            }

            SystemKey result = 0;
            foreach (var item in systemTypes)
            {
                result += (SystemKey)(1 << (int)item);
            }
            return result;
        }
        private CompKey GetUniqueKey(CompType[] componentTypes)
        {
            if (GalaxyEntry.ms_GameIsRelease)
            {
                return 0;
            }

            CompKey result = 0;
            foreach (var item in componentTypes)
            {
                result += (CompKey)(1 << (int)item);
            }
            return result;
        }

        public HolderProxy GetHolder(HolderKey id, SystemType[] dependSystemTypes, CompType[] rwComponents, CompType[] roComponents)
        {
            Tuple<SystemKey, CompKey, CompKey> key = new Tuple<HolderKey, HolderKey, HolderKey>(GetUniqueKey(dependSystemTypes), GetUniqueKey(rwComponents), GetUniqueKey(roComponents));
            if (m_Pool.ContainsKey(id) && m_Pool[id].ContainsKey(key))
                return m_Pool[id][key];

            Holder holder = m_Owner.GetHolder(id);
            if (holder == null)
                return null;

            HolderProxy proxy = new HolderProxy(holder, dependSystemTypes, rwComponents, roComponents);
            if (!m_Pool.ContainsKey(id))
                m_Pool.Add(id, new Dictionary<Tuple<SystemKey, CompKey, CompKey>, HolderProxy>());
            m_Pool[id].Add(key, proxy);
            return proxy;
        }

        public HolderProxy GetHolder(HolderKey id, SystemType systemType, SystemType[] dependSystemTypes, CompType[] rwComponents, CompType[] roComponents)
        {
            Tuple<SystemKey, CompKey, CompKey> key = new Tuple<HolderKey, HolderKey, HolderKey>(GetUniqueKey(dependSystemTypes), GetUniqueKey(rwComponents), GetUniqueKey(roComponents));
            if (m_Pool.ContainsKey(id) && m_Pool[id].ContainsKey(key))
                return m_Pool[id][key];

            Holder holder = m_Owner.GetHolder(id);
            if (holder == null)
                return null;

            //add self to depend list
            SystemType[] tempDependSystemTypes = new SystemType[dependSystemTypes.Length + 1];
            tempDependSystemTypes[0] = systemType;
            for (int i = 0; i < dependSystemTypes.Length; i++)
            {
                tempDependSystemTypes[i + 1] = dependSystemTypes[i];
            }

            HolderProxy proxy = new HolderProxy(holder, tempDependSystemTypes, rwComponents, roComponents);
            if (!m_Pool.ContainsKey(id))
                m_Pool.Add(id, new Dictionary<Tuple<SystemKey, CompKey, CompKey>, HolderProxy>());
            m_Pool[id].Add(key, proxy);
            return proxy;
        }
    }

    public class HolderTypeDefine
    {
        public HolderTypeDefine(SystemType[] systems, CompType[] rwComps, CompType[] roComps)
        {
            m_DependSystemTypes = systems;
            m_RWCompTypes = rwComps;
            m_ROCompTypes = roComps;
        }

        public SystemType[] m_DependSystemTypes;
        public CompType[] m_RWCompTypes;
        public CompType[] m_ROCompTypes;
    }

    /// <summary>
    /// entity所拥有的数据获取方式，通过适配器把整个数据修改分为读写和只读
    /// (c#没有c++的const声明)
    /// </summary>
    public class HolderProxy
    {
        private Holder m_Holder;
        private List<SystemType> m_DenpendSystemTypes;
        private Dictionary<CompType, ComponentProxy> m_RWComponents;
        private Dictionary<CompType, ComponentProxy> m_ROComponents;

        public HolderProxy(Holder holder, SystemType[] dependSystemTypes, CompType[] rwComponents, CompType[] roComponents)
        {
            m_Holder = holder;
            m_DenpendSystemTypes = new List<SystemType>(dependSystemTypes);
            m_RWComponents = new Dictionary<CompType, ComponentProxy>();
            foreach (CompType compType in rwComponents)
            {
                ComponentBase component = m_Holder.GetComponent(compType);
                Debug.Assert(component != null);
                ComponentProxy proxy = ComponentHelper.CreateProxyInstance(compType, component, AccessType.ReadWrite);
                Debug.Assert(proxy != null);
                m_RWComponents.Add(compType, proxy);
            }
            m_ROComponents = new Dictionary<CompType, ComponentProxy>();
            foreach (CompType compType in roComponents)
            {
                ComponentBase component = m_Holder.GetComponent(compType);
                Debug.Assert(component != null);
                ComponentProxy proxy = ComponentHelper.CreateProxyInstance(compType, component, AccessType.ReadOnly);
                Debug.Assert(proxy != null);
                m_ROComponents.Add(compType, proxy);
            }
        }

        public uint Uid => m_Holder.UID;

        public ComponentProxy GetComponent(CompType compType)
        {
            if (m_RWComponents.ContainsKey(compType))
            {
                return m_RWComponents[compType];
            }
            else if (m_ROComponents.ContainsKey(compType))
            {
                return m_ROComponents[compType];
            }
            else
            {
                Debug.Assert(false);
                throw new GalaxyException(Utility.Text.Format("This holder don't have {0}", compType));
            }
        }
        public T GetComponent<T>() where T : ComponentProxy
        {
            CompType compType = ComponentHelper.GetCompEnumByType(typeof(T));
            ComponentProxy proxy = GetComponent(compType);
            Debug.Assert(proxy != null);
            T ret = proxy as T;
            Debug.Assert(ret != null);
            return ret;
        }

        public GalaxySystemProxy GetSystemProxy(SystemType systemType)
        {
            Debug.Assert(m_DenpendSystemTypes.Contains(systemType));
            return m_Holder.GetSystemProxy(systemType);
        }
        public T GetSystemProxy<T>() where T : GalaxySystemProxy
        {
            SystemType systemType = SystemHelper.GetSystemEnumByType(typeof(T));
            T system = GetSystemProxy(systemType) as T;
            Debug.Assert(system != null);
            return system;
        }
    }
}
