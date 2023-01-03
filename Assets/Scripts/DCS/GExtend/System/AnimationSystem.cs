using UnityEngine;
using Galaxy.Entities;
using Galaxy.Data;
using Galaxy.Mold;
using Galaxy.Visual.Command;

namespace Galaxy.Visual
{
    [GalaxySystem(SystemType.Animation)]
    public class AnimationSystem : GameSystem
    {
        private static SystemTypeDefine ms_SystemDefine = new SystemTypeDefine(
            SystemType.Animation,
            new SystemType[] { },
            new CompType[] { CompType.Animation },
            new CompType[] { CompType.Visual }
            );
        public override SystemTypeDefine GetSystemDefine() => ms_SystemDefine;

        private VisualProxyManager m_visualProxyManager;

        internal override void Init(HolderProxy holder)
        {
            m_visualProxyManager = GalaxyEntry.GetModule<VisualProxyManager>();
        }

        internal override void Initialize(HolderProxy holder)
        {
            var visualData = holder.GetComponent<VisualComponentProxy>();
            var animationData = holder.GetComponent<AnimationComponentProxy>();

            Debug.Assert(visualData.VisualPid != CoreConst.INVAILD_VID);

            switch ((AnimatorType)animationData.AnimatorType)
            {
                case AnimatorType.Player:
                    {
                        m_visualProxyManager.VisualCommand(visualData.VisualPid, new ModelCommandSetAnimatorEventCallback
                        {
                            EventType = AnimationEventCallback.AECB_Hit,
                            CallBackAction = () =>
                            {
                                var cmd = new Dots.EcsBufferCommand<Anvil.Animation.AnimationEventInfo>();
                                cmd.InitOperation(new Anvil.Animation.AnimationEventInfo { CallbackType = AnimationEventCallback.AECB_Hit });
                                GalaxyEntry.GetModule<Dots.EcsAdapterModule>().AddEcsCommmand(holder.Uid, cmd);
                            }
                        });

                        m_visualProxyManager.VisualCommand(visualData.VisualPid, new ModelCommandSetAnimatorEventCallback
                        {
                            EventType = AnimationEventCallback.AECB_FootL,
                            CallBackAction = () =>
                            {
                                var cmd = new Dots.EcsBufferCommand<Anvil.Animation.AnimationEventInfo>();
                                cmd.InitOperation(new Anvil.Animation.AnimationEventInfo { CallbackType = AnimationEventCallback.AECB_FootL });
                                GalaxyEntry.GetModule<Dots.EcsAdapterModule>().AddEcsCommmand(holder.Uid, cmd);
                            }
                        });

                        m_visualProxyManager.VisualCommand(visualData.VisualPid, new ModelCommandSetAnimatorEventCallback
                        {
                            EventType = AnimationEventCallback.AECB_FootR,
                            CallBackAction = () =>
                            {
                                var cmd = new Dots.EcsBufferCommand<Anvil.Animation.AnimationEventInfo>();
                                cmd.InitOperation(new Anvil.Animation.AnimationEventInfo { CallbackType = AnimationEventCallback.AECB_FootR });
                                GalaxyEntry.GetModule<Dots.EcsAdapterModule>().AddEcsCommmand(holder.Uid, cmd);
                            }
                        });

                        m_visualProxyManager.VisualCommand(visualData.VisualPid, new ModelCommandSetAnimatorEventCallback
                        {
                            EventType = AnimationEventCallback.AECB_Land,
                            CallBackAction = () =>
                            {
                                var cmd = new Dots.EcsBufferCommand<Anvil.Animation.AnimationEventInfo>();
                                cmd.InitOperation(new Anvil.Animation.AnimationEventInfo { CallbackType = AnimationEventCallback.AECB_Land });
                                GalaxyEntry.GetModule<Dots.EcsAdapterModule>().AddEcsCommmand(holder.Uid, cmd);
                            }
                        });
                    }

                    {
                        m_visualProxyManager.VisualCommand(visualData.VisualPid, new ModelCommandSetAnimatorRootMotionCallback
                        {
                            CallBackAction = (Vector3 deltaPosition, Quaternion deltaRotation) =>
                            {
                                var cmd = new Dots.EcsComponentCommand<Anvil.Animation.AnimationRootMotion>();
                                cmd.InitOperation(new Anvil.Animation.AnimationRootMotion { DeltaPosition = deltaPosition, DeltaRotation = deltaRotation });
                                GalaxyEntry.GetModule<Dots.EcsAdapterModule>().AddEcsCommmand(holder.Uid, cmd);
                            }
                        });
                    }
                    break;
                default:
                    break;
            }
        }

        internal override void Update(HolderProxy holder, float elapseSeconds)
        {

        }

        internal override void Destroy(HolderProxy holder)
        {

        }
    }
}
