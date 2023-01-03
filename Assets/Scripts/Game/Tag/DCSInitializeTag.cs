using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Galaxy.Entities;

namespace Anvil.DCS
{
    public struct DCSInitializeTag : IComponentData
    {
        public EntityType m_EntityType;
        public uint m_EntityConfigId;

        public bool m_HasDCSEntity;
        public bool m_HasInitHitBox;
        public bool m_HasInitProperty;
        public bool m_HasInitAnimation;

        public bool HasInitAllComponent()
        {
            return m_HasDCSEntity 
                && m_HasInitHitBox 
                && m_HasInitProperty 
                && m_HasInitAnimation;
        }
    }
}
