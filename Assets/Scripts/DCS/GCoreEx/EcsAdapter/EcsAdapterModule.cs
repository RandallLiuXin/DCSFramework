using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Galaxy.Dots
{
    internal sealed partial class EcsAdapterModule : GalaxyModule
    {
        private Dictionary<uint, List<EcsCommandBase>> m_EcsCommands;
        private Dictionary<EcsQueryType, EcsQueryArgs> m_EcsQueries;

        public EcsAdapterModule()
        {
            m_EcsCommands = new Dictionary<uint, List<EcsCommandBase>>();
            m_EcsQueries = new Dictionary<EcsQueryType, EcsQueryArgs>();
        }

        internal override void Update(float elapseSeconds)
        {

        }

        internal override void Shutdown()
        {
            foreach (var item in m_EcsCommands)
            {
                foreach (var cmd in item.Value)
                {
                    cmd.Clear();
                }
                item.Value.Clear();
            }
            m_EcsCommands.Clear();

            foreach (var item in m_EcsQueries)
            {
                item.Value.Clear();
            }
            m_EcsQueries.Clear();
        }

        #region Command

        public void AddEcsCommmand(uint entityUid, EcsCommandBase operation)
        {
            if (!m_EcsCommands.ContainsKey(entityUid))
            {
                m_EcsCommands.Add(entityUid, new List<EcsCommandBase>());
            }

            m_EcsCommands[entityUid].Add(operation);
        }

        public List<EcsCommandBase> GetEcsCommands(uint entityUid)
        {
            return m_EcsCommands.ContainsKey(entityUid) ? m_EcsCommands[entityUid] : null;
        }

        public void RemoveEcsCommands(uint entityUid)
        {
            m_EcsCommands.Remove(entityUid);
        }

        #endregion

        #region Query

        public void AddEcsQuery(EcsQueryType queryType)
        {
            if (!m_EcsQueries.ContainsKey(queryType))
            {
                m_EcsQueries.Add(queryType, EcsQueryHelper.GetEcsQuery(queryType));
            }
        }

        public void RemoveEcsQuery(EcsQueryType queryType)
        {
            m_EcsQueries.Remove(queryType);
        }

        public Dictionary<EcsQueryType, EcsQueryArgs> GetCurrentEcsQueries()
        {
            return m_EcsQueries;
        }

        public void AddEcsEntityQuery(EcsQueryType queryType, uint entityUid)
        {
            AddEcsQuery(queryType);
            Debug.Assert(m_EcsQueries.ContainsKey(queryType));
            EcsQueryEntities query = m_EcsQueries[queryType] as EcsQueryEntities;
            Debug.Assert(query != null);
            query.m_Entities.Add(entityUid);
        }

        public void RemoveEcsEntityQuery(EcsQueryType queryType, uint entityUid)
        {
            Debug.Assert(m_EcsQueries.ContainsKey(queryType));
            EcsQueryEntities query = m_EcsQueries[queryType] as EcsQueryEntities;
            Debug.Assert(query != null);
            query.m_Entities.Remove(entityUid);
        }

        #endregion

    }
}

