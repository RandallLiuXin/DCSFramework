using UnityEngine;
using System;

namespace Galaxy.Entities
{
    internal sealed class GalaxySystemManager : GalaxyModule
    {
        private GalaxySystem[] m_Systems;
        private GalaxySystemProxy[] m_SystemProxys;

        public GalaxySystemManager()
        {
            InitAllSystems();
        }

        private void InitAllSystems()
        {
            m_Systems = new GalaxySystem[(int)SystemType.Count];
            m_SystemProxys = new GalaxySystemProxy[(int)SystemType.Count];

            for (SystemType type = 0; type < SystemType.Count; type++)
            {
                int index = (int)type;
                GalaxySystem system = SystemHelper.CreateInstance(type);
                Debug.Assert(system != null);
                m_Systems[index] = system;

                GalaxySystemProxy systemProxy = SystemHelper.CreateProxyInstance(type, system);
                Debug.Assert(systemProxy != null);
                m_SystemProxys[index] = systemProxy;
            }
        }

        internal override void Update(float elapseSeconds)
        {

        }

        internal override void Shutdown()
        {

        }

        public GalaxySystem GetSystem(SystemType systemType)
        {
            Debug.Assert(systemType < SystemType.Count && systemType >= 0);
            return m_Systems[(uint)systemType];
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
            Debug.Assert(systemType < SystemType.Count && systemType >= 0);
            return m_SystemProxys[(uint)systemType];
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
