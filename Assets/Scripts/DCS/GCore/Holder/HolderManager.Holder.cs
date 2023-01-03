using System.Collections.Generic;
using System;
using Galaxy.Entities;
using UnityEngine;

namespace Galaxy.Data
{
    /// <summary>
    /// entity所拥有的数据
    /// 
    /// TODO Randall
    /// 1 Holder构造函数中可以通过serializedData实现存档恢复/网络连接恢复
    /// 2 这里可以增加一个属性变动的广播监听，并广播所有监听者
    /// </summary>
    public class Holder : IReference
    {
        private uint m_Uid = CoreConst.INVAILD_UID;
        private List<SystemType> m_SystemTypes = null;
        private Dictionary<CompType, ComponentBase> m_DataComponents = null;

        public Holder(uint id, SystemType[] systemTypes, CompType[] componentTypes, Dictionary<CompType, ComponentBase> serializedData)
        {
            m_Uid = id;
            m_SystemTypes = new List<SystemType>(systemTypes);

            m_DataComponents = new Dictionary<CompType, ComponentBase>();
            foreach (CompType compType in componentTypes)
            {
                ComponentBase component;
                if (serializedData.ContainsKey(compType))
                {
                    component = ComponentHelper.CreateInstance(compType, serializedData[compType]);
                }
                else
                {
                    component = ComponentHelper.CreateInstance(compType);
                }
                Debug.Assert(component != null && !m_DataComponents.ContainsKey(component.GetCompType()));
                m_DataComponents[component.GetCompType()] = component;
            }
        }

        public void Clear()
        {
            m_DataComponents.Clear();
            m_DataComponents = null;
            m_SystemTypes.Clear();
            m_SystemTypes = null;
            m_Uid = CoreConst.INVAILD_UID;
        }

        public uint UID => m_Uid;

        public ComponentBase GetComponent(CompType compType)
        {
            Debug.Assert(compType < CompType.Count && compType >= 0);
            Debug.Assert(m_DataComponents.ContainsKey(compType));
            return m_DataComponents[compType];
        }
        public T GetComponent<T>() where T : ComponentBase
        {
            CompType compType = ComponentHelper.GetCompEnumByType(typeof(T));
            T component = GetComponent(compType) as T;
            Debug.Assert(component != null);
            return component;
        }

        public GalaxySystem GetSystem(SystemType systemType)
        {
            Debug.Assert(m_SystemTypes.Contains(systemType));
            return GalaxyEntry.GetModule<GalaxySystemManager>().GetSystem(systemType);
        }
        public T GetSystem<T>() where T : GalaxySystem
        {
            SystemType systemType = SystemHelper.GetSystemEnumByType(typeof(T));
            T system = GetSystem(systemType) as T;
            Debug.Assert(system != null);
            return system;
        }

        public GalaxySystemProxy GetSystemProxy(SystemType systemType)
        {
            Debug.Assert(m_SystemTypes.Contains(systemType));
            return GalaxyEntry.GetModule<GalaxySystemManager>().GetSystemProxy(systemType);
        }
        public T GetSystemProxy<T>() where T : GalaxySystemProxy
        {
            SystemType systemType = SystemHelper.GetSystemEnumByType(typeof(T));
            T system = GetSystemProxy(systemType) as T;
            Debug.Assert(system != null);
            return system;
        }

#if UNITY_EDITOR
        public Dictionary<CompType, ComponentBase> GetAllData()
        {
            return m_DataComponents;
        }
#endif
    }
}
