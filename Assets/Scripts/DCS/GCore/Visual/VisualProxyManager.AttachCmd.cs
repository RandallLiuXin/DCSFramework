using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Visual
{
    internal sealed partial class VisualProxyManager
    {
        public enum AttachType
        {
            AttachScene,
            AttachBone,
            //AttachSocket,
            //AttachMesh,
        }

        public class AttachCmd
        {
            private AttachType m_AttachType;
            private uint m_OwnerUid;
            private uint m_Pid;
            private uint m_TargetPid;
            private object[] m_Args;

            public AttachCmd(AttachType attachType, uint ownerUid, uint pid, uint targetPid, object[] args)
            {
                m_AttachType = attachType;
                m_OwnerUid = ownerUid;
                m_Pid = pid;
                m_TargetPid = targetPid;
                m_Args = args;
            }

            public uint GetTargetPid()
            {
                return m_AttachType == AttachType.AttachScene ? CoreConst.INVAILD_PID : m_TargetPid;
            }

            public AttachType AttachType => m_AttachType;
            public uint OwnerUid => m_OwnerUid;
            public uint Pid => m_Pid;
            public uint TargetPid => m_TargetPid;
            public object[] Args => m_Args;
        }

        private void InitAttachPart()
        {
            m_AttachCmdQueue = new List<AttachCmd>();
            m_AttachMap = new Dictionary<uint, AttachCmd>();
            m_AttachedMap = new Dictionary<uint, List<AttachCmd>>();
        }

        private void UpdateAttachPart()
        {
            ProcessAttachCmdQueue();
        }

        private void PostUpdateAttachPart()
        {
            //empty
        }

        private void ProcessAttachCmdQueue()
        {
            //两级缓存
            List<AttachCmd> attachCmds = new List<AttachCmd>(m_AttachCmdQueue);
            m_AttachCmdQueue.Clear();
            foreach (var attachCmd in attachCmds)
            {
                TryProcessAttachCmd(attachCmd);
            }
        }

        private bool TryProcessAttachCmd(AttachCmd attachCmd)
        {
            Debug.Assert(CheckAttachCmdValid(attachCmd));
            if (!CheckAttachCmdPrepared(attachCmd))
            {
                m_AttachCmdQueue.Add(attachCmd);
                return false;
            }
            if (!DispatchAttach(attachCmd))
            {
                //RemoveAttachRecord(attachCmd);
                Debug.Assert(false);
                return false;
            }
            return true;
        }

        private bool CheckAttachCmdValid(AttachCmd cmd)
        {
            if (!m_ProxyToVisualMap.ContainsKey(cmd.Pid))
                return false;
            uint targetPid = cmd.GetTargetPid();
            if (targetPid != CoreConst.INVAILD_PID && !m_ProxyToVisualMap.ContainsKey(targetPid))
                return false;
            VisualBase visual = GetVisual(cmd.Pid);
            Debug.Assert(visual != null && !visual.IsAttached());
            return true;
        }

        private bool CheckAttachCmdPrepared(AttachCmd cmd)
        {
            VisualBase visual = GetVisual(cmd.Pid);
            Debug.Assert(visual != null);
            if (!visual.IsLoaded())
                return false;
            uint targetPid = cmd.GetTargetPid();
            if (targetPid == CoreConst.INVAILD_PID)
                return true;
            VisualBase targetVisual = GetVisual(targetPid);
            Debug.Assert(targetVisual != null);
            return targetVisual.IsLoaded();
        }

        private bool DispatchAttach(AttachCmd cmd)
        {
            switch (cmd.AttachType)
            {
                case AttachType.AttachScene:
                    return AttachToScene(cmd);
                case AttachType.AttachBone:
                    return AttachToBone(cmd);
                //case AttachType.AttachSocket:
                //    return AttachToSocket(cmd);
                default:
                    return false;
            }
        }

        private bool AttachToScene(AttachCmd cmd)
        {
            VisualBase visual = GetVisual(cmd.Pid);
            if (visual == null)
                return false;
            Entities.Entity owner = GalaxyEntry.GetModule<Entities.EntityManager>().GetEntity(cmd.OwnerUid);
            if (owner == null || owner.GetLogicObject() == null)
            {
                Debug.Assert(false);
                return false;
            }
            visual.AttachToScene(owner.GetLogicObject());
            visual.AttachFinish();
            return true;
        }

        private bool AttachToBone(AttachCmd cmd)
        {
            VisualBase visual = GetVisual(cmd.Pid);
            VisualBase targetVisual = GetVisual(cmd.TargetPid);
            if (visual == null || targetVisual == null)
                return false;
            targetVisual.BindToBone(visual.ResObject, cmd.Args);
            visual.AttachFinish();
            return true;
        }

        //private bool AttachToSocket(AttachCmd cmd)
        //{
        //}

        public void AttachToScene(uint pid)
        {
            AttachCmd attachCmd = new AttachCmd(AttachType.AttachScene, GetOwnerUidByPid(pid), pid, CoreConst.INVAILD_PID, null);
            AddAttachRecord(attachCmd);
            TryProcessAttachCmd(attachCmd);
        }

        public void AttachToBone(uint pid, uint targetPid, string bindTargetName, Vector3 localPosition, Quaternion localRotaion, Vector3 localScale)
        {
            AttachCmd attachCmd = new AttachCmd(AttachType.AttachBone, GetOwnerUidByPid(pid), pid, targetPid, new object[4] { bindTargetName, localPosition, localRotaion, localScale });
            AddAttachRecord(attachCmd);
            TryProcessAttachCmd(attachCmd);
        }

        public void Unattach(uint pid)
        {
            //仅移除挂接，但没有移除资源，可以复用
            VisualBase visual = GetVisual(pid);
            if (visual == null)
                return;
            UnattachVisual(visual);
        }

        private void UnattachVisual(VisualBase visual)
        {
            if (visual == null || !m_AttachMap.ContainsKey(visual.VID))
                return;
            visual.RemoveFromParent();
            RemoveAttachRecord(visual.VID, m_AttachMap[visual.VID]);
        }

        private void AddAttachRecord(AttachCmd cmd)
        {
            Debug.Assert(cmd != null && m_ProxyToVisualMap.ContainsKey(cmd.Pid));
            uint vid = m_ProxyToVisualMap[cmd.Pid];
            m_AttachMap.Add(vid, cmd);

            if (cmd.TargetPid == CoreConst.INVAILD_PID)
                return;
            if (!m_ProxyToVisualMap.ContainsKey(cmd.TargetPid))
                return;

            uint targetVid = m_ProxyToVisualMap[cmd.TargetPid];
            if (!m_AttachedMap.ContainsKey(targetVid))
                m_AttachedMap[targetVid] = new List<AttachCmd>();

            m_AttachedMap[targetVid].Add(cmd);
        }

        private void RemoveAttachRecord(uint vid, AttachCmd cmd)
        {
            Debug.Assert(cmd != null && vid != CoreConst.INVAILD_VID);
            m_AttachMap.Remove(vid);

            if (cmd.TargetPid == CoreConst.INVAILD_PID)
                return;
            if (!m_ProxyToVisualMap.ContainsKey(cmd.TargetPid))
                return;

            uint targetVid = m_ProxyToVisualMap[cmd.TargetPid];
            if (!m_AttachedMap.ContainsKey(targetVid))
                return;
            m_AttachedMap[targetVid].Remove(cmd);
            if (m_AttachedMap[targetVid].Count == 0)
                m_AttachedMap.Remove(targetVid);
        }

        public bool CheckAttached(uint pid)
        {
            return m_ProxyToVisualMap.ContainsKey(pid) 
                && m_AttachMap.ContainsKey(m_ProxyToVisualMap[pid]);
        }
    }
}
