using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Visual
{
    internal sealed partial class VisualProxyManager : GalaxyModule
    {
        private Dictionary<uint, VisualBase> m_VisualPool;

        private Dictionary<uint, uint> m_ProxyToVisualMap;
        private Dictionary<uint, List<uint>> m_VisualToProxyMap;

        private Dictionary<uint, uint> m_ProxyToOwnerMap;
        private Dictionary<uint, List<uint>> m_OwnerToProxyMap;

        // engine part
        private EngineProxy m_EngineProxy;

        // attach part
        private List<AttachCmd> m_AttachCmdQueue;
        private Dictionary<uint, AttachCmd> m_AttachMap;  // 记录vid被挂接在哪
        private Dictionary<uint, List<AttachCmd>> m_AttachedMap;  // 记录每个visual被其他visual挂接的cmd记录

        // TODO Randall 预加载 preload visual
        // self._preload_map = {} # 保存预加载的pid， {(pre_type, pre_id): pre_pid}

        internal override bool NeedUpdateLogic => false;
        internal override bool NeedUpdateMono => true;
        internal override bool NeedFixedUpdate => true;

        public VisualProxyManager()
        {
            m_VisualPool = new Dictionary<uint, VisualBase>();

            m_ProxyToVisualMap = new Dictionary<uint, uint>();
            m_VisualToProxyMap = new Dictionary<uint, List<uint>>();

            m_ProxyToOwnerMap = new Dictionary<uint, uint>();
            m_OwnerToProxyMap = new Dictionary<uint, List<uint>>();

            InitEnginePart();
            InitAttachPart();
            InitCommandAndQueryPart();
        }

        private void InitEnginePart()
        {
            m_EngineProxy = new EngineProxy();
            m_EngineProxy.Init();
        }

        internal override void Update(float elapseSeconds)
        {
        }

        internal override void FixedUpdate(float elapseSeconds)
        {
            VisualUpdate(elapseSeconds);
            VisualPostUpdate(elapseSeconds);
        }

        private void VisualUpdate(float elapseSeconds)
        {
            UpdateAttachPart();
            m_EngineProxy.Update(elapseSeconds);
            foreach (var visual in m_VisualPool)
            {
                visual.Value.Update(elapseSeconds);
            }
            m_EngineProxy.CameraUpdate(elapseSeconds);
        }

        private void VisualPostUpdate(float elapseSeconds)
        {
            PostUpdateAttachPart();
            m_EngineProxy.PostUpdate(elapseSeconds);
            foreach (var visual in m_VisualPool)
            {
                visual.Value.PostUpdate(elapseSeconds);
            }
            m_EngineProxy.CameraPostUpdate(elapseSeconds);
        }

        internal override void Shutdown()
        {
            // nothing need to do
        }

        public bool CheckPidValid(uint pid)
        {
            return m_ProxyToOwnerMap.ContainsKey(pid);
        }

        private VisualBase GetVisual(uint pid)
        {
            if (!m_ProxyToVisualMap.ContainsKey(pid))
                return null;
            uint vid = m_ProxyToVisualMap[pid];
            Debug.Assert(m_VisualPool.ContainsKey(vid));
            return m_VisualPool[vid];
        }

        #region Command&Query
        public void VisualCommand(uint pid, Event.VisualCommandArgs eventArgs)
        {
            VisualBase visual = GetVisual(pid);
            Debug.Assert(visual != null);
            ProcessCommand(visual, eventArgs);
        }

        public object[] VisualQuery(uint pid, Event.VisualQueryArgs eventArgs)
        {
            VisualBase visual = GetVisual(pid);
            Debug.Assert(visual != null);
            return ProcessQuery(visual, eventArgs);
        }

        public void EngineCommand(Event.EngineCommandArgs eventArgs)
        {
            Debug.Assert(m_EngineProxy != null);
            ProcessCommand(m_EngineProxy, eventArgs);
        }

        public object[] EngineQuery(Event.EngineQueryArgs eventArgs)
        {
            Debug.Assert(m_EngineProxy != null);
            return ProcessQuery(m_EngineProxy, eventArgs);
        }
        #endregion

        #region Visual资源相关
        public uint AcquireVisualProxy(uint uid, VisualType visualType, string modelPath, params object[] initArgs)
        {
            uint pid = GeneratorUid(Common.UidRequireType.VisualProxy);
            uint vid = CreateNewVisual(pid, visualType, modelPath, initArgs);
            Debug.Assert(!m_ProxyToVisualMap.ContainsKey(pid));
            m_ProxyToVisualMap[pid] = vid;
            m_VisualToProxyMap[vid] = new List<uint> { pid };
            m_ProxyToOwnerMap[pid] = uid;
            if (!m_OwnerToProxyMap.ContainsKey(uid))
                m_OwnerToProxyMap[uid] = new List<uint>();

            m_OwnerToProxyMap[uid].Add(pid);
            return pid;
        }

        //public uint CopyVisualProxy(uint uid, uint targetUid, uint pid)
        //{
        //}

        //public uint MoveVisualProxy(uint uid, uint targetUid, uint pid)
        //{
        //}

        public void ReleaseProxyForUid(uint uid)
        {
            Debug.Assert(m_OwnerToProxyMap.ContainsKey(uid));
            List<uint> removeList = new List<uint>(m_OwnerToProxyMap[uid]);
            foreach (uint pid in removeList)
            {
                ReleaseProxyId(pid, uid);
            }
        }

        public void ReleaseProxyId(uint pid, uint ownerUid)
        {
            Debug.Assert(m_ProxyToOwnerMap.ContainsKey(pid));
            uint uid = m_ProxyToOwnerMap[pid];
            m_ProxyToOwnerMap.Remove(pid);

            Debug.Assert(uid == ownerUid);
            Debug.Assert(m_OwnerToProxyMap.ContainsKey(uid));
            Debug.Assert(m_OwnerToProxyMap[uid].Contains(pid));
            m_OwnerToProxyMap[uid].Remove(pid);
            if (m_OwnerToProxyMap[uid].Count == 0)
                m_OwnerToProxyMap.Remove(uid);

            Debug.Assert(m_ProxyToVisualMap.ContainsKey(pid));
            uint vid = m_ProxyToVisualMap[pid];
            m_ProxyToVisualMap.Remove(pid);

            Debug.Assert(m_VisualToProxyMap.ContainsKey(vid));
            Debug.Assert(m_VisualToProxyMap[vid].Contains(pid));
            m_VisualToProxyMap[vid].Remove(pid);
            if (m_VisualToProxyMap[vid].Count == 0)
            {
                m_VisualToProxyMap.Remove(vid);
                DestroyVisual(vid);
            }
        }

        private uint CreateNewVisual(uint pid, VisualType visualType, string modelPath, params object[] initArgs)
        {
            uint vid = GeneratorUid(Common.UidRequireType.Visual);
            Debug.Assert(!m_VisualPool.ContainsKey(vid));
            VisualBase visual = Activator.CreateInstance(VisualDefine.VisualTypes[(int)visualType], pid, vid, modelPath) as VisualBase;
            visual.Init(initArgs);
            m_VisualPool.Add(vid, visual);
            return vid;
        }

        private void DestroyVisual(uint vid)
        {
            Debug.Assert(m_VisualPool.ContainsKey(vid));
            VisualBase visual = m_VisualPool[vid];
            m_VisualPool.Remove(vid);

            if (m_AttachedMap.ContainsKey(vid))
            {
                List<AttachCmd> cmdList = m_AttachedMap[vid];
                foreach (var cmd in cmdList)
                {
                    Unattach(cmd.Pid);
                }
            }

            UnattachVisual(visual);
            visual.Destroy();
        }

        private uint GeneratorUid(Common.UidRequireType uidType)
        {
            return GalaxyEntry.GetModule<Common.UidGenerator>().GenerateUid(uidType);
        }

        private uint GetOwnerUidByPid(uint pid)
        {
            Debug.Assert(m_ProxyToOwnerMap.ContainsKey(pid));
            return m_ProxyToOwnerMap[pid];
        }
        #endregion
    }
}
