using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Galaxy.Dots
{
    public enum EcsQueryType
    {
        EQ_Default,
        EQ_PlayerTranslation,
        EQ_EntityState,
        //EQ_Entities,
    }

    public interface IEcsQueryJob
    {
        public abstract void InitJob(EcsQueryArgs args);
    }

    public abstract class EcsQueryArgs : IReference
    {
        public EcsQueryType m_QueryType;

        public EcsQueryArgs(EcsQueryType queryType)
        {
            m_QueryType = queryType;
        }

        public virtual void Clear()
        {
            m_QueryType = EcsQueryType.EQ_Default;
        }
    }

    public sealed class EcsQueryPlayerDefault : EcsQueryArgs
    {
        public EcsQueryPlayerDefault(EcsQueryType queryType) : base(queryType)
        {
        }

        public override void Clear()
        {
        }
    }

    public sealed class EcsQueryEntities : EcsQueryArgs
    {
        public HashSet<uint> m_Entities;

        public EcsQueryEntities(EcsQueryType queryType) : base(queryType)
        {
            m_Entities = new HashSet<uint> { };
        }

        public override void Clear()
        {
            m_Entities.Clear();
            base.Clear();
        }
    }
}

