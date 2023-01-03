using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Visual
{
    internal sealed class VisualProxyRegister : GalaxyModule
    {
        private Dictionary<VisualCommandType, List<VisualFlushType>> m_VisualRelatedMap;
        private Dictionary<VisualCommandType, GalaxyAction<VisualBase, Event.VisualCommandArgs>> m_VisualCmdToActionMap;
        private Dictionary<VisualFlushType, GalaxyAction<VisualBase>> m_VisualFlushToActionMap;

        private Dictionary<EngineCommandType, List<EngineFlushType>> m_EngineRelatedMap;
        private Dictionary<EngineCommandType, GalaxyAction<EngineProxy, Event.EngineCommandArgs>> m_EngineCmdToActionMap;
        private Dictionary<EngineFlushType, GalaxyAction<EngineProxy>> m_EngineFlushToActionMap;

        public delegate object[] GalaxyQuery<in T>(T obj);
        private Dictionary<VisualQueryType, QueryBase> m_VisualQueryMap;
        private Dictionary<EngineQueryType, QueryBase> m_EngineQueryMap;

        internal override bool NeedUpdateLogic => false;
        internal override bool NeedUpdateMono => true;

        public VisualProxyRegister()
        {
            m_VisualRelatedMap = new Dictionary<VisualCommandType, List<VisualFlushType>>();
            m_VisualCmdToActionMap = new Dictionary<VisualCommandType, GalaxyAction<VisualBase, Event.VisualCommandArgs>>();
            m_VisualFlushToActionMap = new Dictionary<VisualFlushType, GalaxyAction<VisualBase>>();

            m_EngineRelatedMap = new Dictionary<EngineCommandType, List<EngineFlushType>>();
            m_EngineCmdToActionMap = new Dictionary<EngineCommandType, GalaxyAction<EngineProxy, Event.EngineCommandArgs>>();
            m_EngineFlushToActionMap = new Dictionary<EngineFlushType, GalaxyAction<EngineProxy>>();

            // Query
            m_VisualQueryMap = new Dictionary<VisualQueryType, QueryBase>();
            m_EngineQueryMap = new Dictionary<EngineQueryType, QueryBase>();
        }

        internal override void Update(float elapseSeconds)
        {
        }

        internal override void Shutdown()
        {
            m_VisualRelatedMap.Clear();
            m_VisualCmdToActionMap.Clear();
            m_VisualFlushToActionMap.Clear();

            m_EngineRelatedMap.Clear();
            m_EngineCmdToActionMap.Clear();
            m_EngineFlushToActionMap.Clear();
        }

        #region Command
        public void RegisterCommandToFlush(VisualCommandType commandType, List<VisualFlushType> flushTypes)
        {
            Debug.Assert(!m_VisualRelatedMap.ContainsKey(commandType));
            m_VisualRelatedMap[commandType] = flushTypes;
        }

        public void RegisterCommandToFlush(EngineCommandType commandType, List<EngineFlushType> flushTypes)
        {
            Debug.Assert(!m_EngineRelatedMap.ContainsKey(commandType));
            m_EngineRelatedMap[commandType] = flushTypes;
        }

        public void RegisterCommand(VisualCommandType commandType, GalaxyAction<VisualBase, Event.VisualCommandArgs> action)
        {
            Debug.Assert(!m_VisualCmdToActionMap.ContainsKey(commandType));
            m_VisualCmdToActionMap[commandType] = action;
        }

        public void RegisterCommand(EngineCommandType commandType, GalaxyAction<EngineProxy, Event.EngineCommandArgs> action)
        {
            Debug.Assert(!m_EngineCmdToActionMap.ContainsKey(commandType));
            m_EngineCmdToActionMap[commandType] = action;
        }

        public void RegisterFlush(VisualFlushType flushType, GalaxyAction<VisualBase> action)
        {
            Debug.Assert(!m_VisualFlushToActionMap.ContainsKey(flushType));
            m_VisualFlushToActionMap[flushType] = action;
        }

        public void RegisterFlush(EngineFlushType flushType, GalaxyAction<EngineProxy> action)
        {
            Debug.Assert(!m_EngineFlushToActionMap.ContainsKey(flushType));
            m_EngineFlushToActionMap[flushType] = action;
        }

        public List<VisualFlushType> GetRelatedFlushs(VisualCommandType visualCommand)
        {
            Debug.Assert(m_VisualRelatedMap.ContainsKey(visualCommand));
            return m_VisualRelatedMap[visualCommand];
        }

        public List<EngineFlushType> GetRelatedFlushs(EngineCommandType engineCommand)
        {
            Debug.Assert(m_EngineRelatedMap.ContainsKey(engineCommand));
            return m_EngineRelatedMap[engineCommand];
        }

        public GalaxyAction<VisualBase, Event.VisualCommandArgs> GetCommandAction(VisualCommandType visualCommand)
        {
            Debug.Assert(m_VisualCmdToActionMap.ContainsKey(visualCommand));
            return m_VisualCmdToActionMap[visualCommand];
        }

        public GalaxyAction<EngineProxy, Event.EngineCommandArgs> GetCommandAction(EngineCommandType engineCommand)
        {
            Debug.Assert(m_EngineCmdToActionMap.ContainsKey(engineCommand));
            return m_EngineCmdToActionMap[engineCommand];
        }

        public GalaxyAction<VisualBase> GetFlushAction(VisualFlushType flushType)
        {
            Debug.Assert(m_VisualFlushToActionMap.ContainsKey(flushType));
            return m_VisualFlushToActionMap[flushType];
        }

        public GalaxyAction<EngineProxy> GetFlushAction(EngineFlushType flushType)
        {
            Debug.Assert(m_EngineFlushToActionMap.ContainsKey(flushType));
            return m_EngineFlushToActionMap[flushType];
        }
        #endregion

        #region Query
        public void RegisterQuery(Query.VisualQuery query)
        {
            VisualQueryType queryType = query.GetVisualQueryType();
            Debug.Assert(!m_VisualQueryMap.ContainsKey(queryType));
            m_VisualQueryMap.Add(queryType, query);
        }

        public void RegisterQuery(Query.EngineQuery query)
        {
            EngineQueryType queryType = query.GetEngineQueryType();
            Debug.Assert(!m_EngineQueryMap.ContainsKey(queryType));
            m_EngineQueryMap.Add(queryType, query);
        }

        public QueryBase GetQuery(VisualQueryType queryType)
        {
            Debug.Assert(m_VisualQueryMap.ContainsKey(queryType));
            return m_VisualQueryMap[queryType];
        }

        public QueryBase GetQuery(EngineQueryType queryType)
        {
            Debug.Assert(m_EngineQueryMap.ContainsKey(queryType));
            return m_EngineQueryMap[queryType];
        }
        #endregion
    }

    public abstract class QueryBase
    {
        public object[] Query(VisualBase visual, Event.GameEventArgs eventArgs)
        {
            if (Perpared(visual))
            {
                return QueryPrepared(visual, eventArgs as Event.VisualQueryArgs);
            }
            else
            {
                return QueryNotPrepared(visual, eventArgs as Event.VisualQueryArgs);
            }
        }

        public object[] Query(EngineProxy engine, Event.GameEventArgs eventArgs)
        {
            if (Perpared(engine))
            {
                return QueryPrepared(engine, eventArgs as Event.EngineQueryArgs);
            }
            else
            {
                return QueryNotPrepared(engine, eventArgs as Event.EngineQueryArgs);
            }
        }

        protected bool Perpared(VisualBase visual) { return visual.IsAttached(); }
        protected bool Perpared(EngineProxy engine) { return engine != null; }

        protected abstract object[] QueryPrepared(VisualBase visual, Event.VisualQueryArgs eventArgs);
        protected abstract object[] QueryPrepared(EngineProxy engine, Event.EngineQueryArgs eventArgs);
        protected abstract object[] QueryNotPrepared(VisualBase visual, Event.VisualQueryArgs eventArgs);
        protected abstract object[] QueryNotPrepared(EngineProxy engine, Event.EngineQueryArgs eventArgs);
    }
}
