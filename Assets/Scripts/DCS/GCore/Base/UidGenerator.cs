using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Common
{
    public enum UidRequireType
    {
        Default,
        Entity,
        Visual,
        VisualProxy,
        PropertyChangeSource,
    }

    internal sealed partial class UidGenerator : GalaxyModule
    {
        private Dictionary<UidRequireType, uint> m_UidMap;

        public UidGenerator()
        {
            m_UidMap = new Dictionary<UidRequireType, uint>();
        }

        internal override void Update(float elapseSeconds)
        {
        }

        internal override void Shutdown()
        {
        }

        public uint GenerateUid(UidRequireType requireType)
        {
            if (!m_UidMap.ContainsKey(requireType))
            {
                m_UidMap.Add(requireType, CoreConst.INVAILD_UID);
            }
            return ++m_UidMap[requireType];
        }

        public uint GenerateChangeSouceUid()
        {
            return GenerateUid(UidRequireType.PropertyChangeSource);
        }
    }
}
