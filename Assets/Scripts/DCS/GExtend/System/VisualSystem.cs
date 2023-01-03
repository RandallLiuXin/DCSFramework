using UnityEngine;
using Galaxy.Entities;
using Galaxy.Data;
using Galaxy.Mold;

namespace Galaxy.Visual
{
    [GalaxySystem(SystemType.Visual)]
    public class VisualSystem : GameSystem
    {
        private static SystemTypeDefine ms_SystemDefine = new SystemTypeDefine(
            SystemType.Visual,
            new SystemType[] { },
            new CompType[] { CompType.Visual },
            new CompType[] { }
            );
        public override SystemTypeDefine GetSystemDefine() => ms_SystemDefine;

        private VisualProxyManager m_visualProxyManager;

        internal override void Init(HolderProxy holder)
        {
            m_visualProxyManager = GalaxyEntry.GetModule<VisualProxyManager>();

            var visualData = holder.GetComponent<VisualComponentProxy>();
            Debug.Assert(!visualData.ModelPath.IsNE() && visualData.VisualType < (uint)VisualType.Count);
            visualData.VisualPid = m_visualProxyManager.AcquireVisualProxy(holder.Uid, (VisualType)visualData.VisualType, visualData.ModelPath);

            Command.VisualCommandPosition positionCommand = new Command.VisualCommandPosition
            {
                Position = visualData.VisualPos
            };
            m_visualProxyManager.VisualCommand(visualData.VisualPid, positionCommand);

            Command.VisualCommandRotation rotationCommand = new Command.VisualCommandRotation
            {
                Rotation = visualData.VisualRot
            };
            m_visualProxyManager.VisualCommand(visualData.VisualPid, rotationCommand);

            m_visualProxyManager.AttachToScene(visualData.VisualPid);
        }

        internal override void Update(HolderProxy holder, float elapseSeconds)
        {
        }

        internal override void FixedUpdate(HolderProxy holder, float elapseSeconds)
        {
            //Visual model move into dots, so don't need those logic
            //var visualData = holder.GetComponent<VisualComponentProxy>();
            //if (visualData.VisualPid == CoreConst.INVAILD_PID)
            //    return;

            //Command.VisualCommandPosition positionCommand = new Command.VisualCommandPosition
            //{
            //    Position = visualData.VisualPos
            //};
            //m_visualProxyManager.VisualCommand(visualData.VisualPid, positionCommand);

            //Command.VisualCommandRotation rotationCommand = new Command.VisualCommandRotation
            //{
            //    Rotation = visualData.VisualRot
            //};
            //m_visualProxyManager.VisualCommand(visualData.VisualPid, rotationCommand);
        }

        internal override void Destroy(HolderProxy holder)
        {
            m_visualProxyManager.ReleaseProxyForUid(holder.Uid);
        }

        [GalaxyMethod()]
        public void AttachToBone(HolderProxy holder, uint targetPid, string bindTargetName, Vector3 localPosition, Quaternion localRotaion, Vector3 localScale)
        {
            var visualData = holder.GetComponent<VisualComponentProxy>();
            if (visualData.VisualPid == CoreConst.INVAILD_PID)
                return;

            m_visualProxyManager.AttachToBone(visualData.VisualPid, targetPid, bindTargetName, localPosition, localRotaion, localScale);
        }

        [GalaxyMethod()]
        public void UnAttach(HolderProxy holder)
        {
            var visualData = holder.GetComponent<VisualComponentProxy>();
            if (visualData.VisualPid == CoreConst.INVAILD_PID)
                return;

            m_visualProxyManager.Unattach(visualData.VisualPid);
        }
    }
}
