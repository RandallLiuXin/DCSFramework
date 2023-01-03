using Galaxy.Fsm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityFsmOwner = Galaxy.Fsm.IFsm<Galaxy.Entities.EntityFsm>;
using Anvil.Input;
using Galaxy.Visual;
using Galaxy.Visual.Command;

namespace Galaxy.Entities
{
    public class DeadStateEvent : FsmEventParams
    {
        public static readonly int EventId = typeof(DeadStateEvent).GetHashCode();

        public override void Clear()
        {
        }
    }

    public abstract class LocalPlayerBaseState : EntityBaseState
    {
        protected internal override void OnEnter(EntityFsmOwner fsmOwner, FsmEnterParams enterParams = null)
        {
            base.OnEnter(fsmOwner, enterParams);
            SubscribeEvent(DeadStateEvent.EventId, OnChangeToDeadState);
        }

        protected internal override void OnLeave(EntityFsmOwner fsmOwner, bool isShutdown)
        {
            UnsubscribeEvent(DeadStateEvent.EventId, OnChangeToDeadState);
            base.OnLeave(fsmOwner, isShutdown);
        }

        protected void OnChangeToDeadState(EntityFsmOwner fsm, object sender, object userData)
        {
            DeadStateEvent fsmEvent = userData as DeadStateEvent;
            Debug.Assert(fsmEvent != null);
            fsm.ChangeState<LocalPlayerStateDead>();
        }
    }

    public class LocalPlayerStateIdle : LocalPlayerBaseState
    {
        protected override EntityStateType GetStateType()
        {
            return EntityStateType.Idle;
        }

        protected internal override void OnEnter(EntityFsmOwner fsmOwner, FsmEnterParams enterParams = null)
        {
            base.OnEnter(fsmOwner, enterParams);
        }

        protected internal override void OnUpdate(EntityFsmOwner fsmOwner, float elapseSeconds)
        {
            base.OnUpdate(fsmOwner, elapseSeconds);
            if (InputProxyInstance.Instance.InputVector != Vector2.zero)
            {
                fsmOwner.ChangeState<LocalPlayerStateMoving>();
                return;
            }
            if (InputProxyInstance.Instance.Attack)
            {
                fsmOwner.ChangeState<LocalPlayerStateSkill>();
                return;
            }
        }

        protected internal override void OnLeave(EntityFsmOwner fsmOwner, bool isShutdown)
        {
            base.OnLeave(fsmOwner, isShutdown);
        }
    }

    public class LocalPlayerStateMoving : LocalPlayerBaseState
    {
        protected override EntityStateType GetStateType()
        {
            return EntityStateType.Moving;
        }

        protected internal override void OnEnter(EntityFsmOwner fsmOwner, FsmEnterParams enterParams = null)
        {
            base.OnEnter(fsmOwner, enterParams);

            {
                ModelCommandSetAnimatorParameter cmd = new ModelCommandSetAnimatorParameter();
                cmd.Datas.Add(new SetAnimatorParameterData
                {
                    ParameterName = AnimationConst.AC_Moving,
                    ParameterValueType = AnimatorParameterValueType.AP_Bool,
                    bValue = true
                });
                cmd.Datas.Add(new SetAnimatorParameterData
                {
                    ParameterName = AnimationConst.AC_VelocityZ,
                    ParameterValueType = AnimatorParameterValueType.AP_Float,
                    fValue = 1.0f
                });
                GalaxyEntry.GetModule<VisualProxyManager>().VisualCommand(GetVisualPid(fsmOwner), cmd);
            }
            {
                GalaxyEntry.GetModule<VisualProxyManager>().VisualCommand(GetVisualPid(fsmOwner), new ModelCommandToggleAnimatorRootMotion { });
            }
        }

        protected internal override void OnUpdate(EntityFsmOwner fsmOwner, float elapseSeconds)
        {
            base.OnUpdate(fsmOwner, elapseSeconds);
            if (InputProxyInstance.Instance.Attack)
            {
                fsmOwner.ChangeState<LocalPlayerStateSkill>();
                return;
            }

            var inputVector = InputProxyInstance.Instance.InputVector;
            // dispatch movement
            {
                var cmd = new Dots.EcsComponentCommand<Anvil.Player.PlayerInput>();
                cmd.InitOperation(new Anvil.Player.PlayerInput { m_NeedRefreshInput = true, m_InputVector = new Unity.Mathematics.float3(inputVector.x, 0, inputVector.y) });
                GalaxyEntry.GetModule<Dots.EcsAdapterModule>().AddEcsCommmand(fsmOwner.Owner.GetOwner().UID, cmd);
            }

            if (inputVector == Vector2.zero)
            {
                fsmOwner.ChangeState<LocalPlayerStateIdle>();
            }
        }

        protected internal override void OnLeave(EntityFsmOwner fsmOwner, bool isShutdown)
        {
            {
                GalaxyEntry.GetModule<VisualProxyManager>().VisualCommand(GetVisualPid(fsmOwner), new ModelCommandToggleAnimatorRootMotion { });
            }

            {
                var cmd = new Dots.EcsComponentCommand<Anvil.Player.PlayerInput>();
                cmd.InitOperation(new Anvil.Player.PlayerInput { m_NeedRefreshInput = true, m_InputVector = new Unity.Mathematics.float3(0, 0, 0) });
                GalaxyEntry.GetModule<Dots.EcsAdapterModule>().AddEcsCommmand(fsmOwner.Owner.GetOwner().UID, cmd);
            }

            {
                ModelCommandSetAnimatorParameter cmd = new ModelCommandSetAnimatorParameter();
                cmd.Datas.Add(new SetAnimatorParameterData
                {
                    ParameterName = AnimationConst.AC_VelocityZ,
                    ParameterValueType = AnimatorParameterValueType.AP_Float,
                    fValue = 0.0f
                });
                cmd.Datas.Add(new SetAnimatorParameterData
                {
                    ParameterName = AnimationConst.AC_Moving,
                    ParameterValueType = AnimatorParameterValueType.AP_Bool,
                    bValue = false
                });
                GalaxyEntry.GetModule<VisualProxyManager>().VisualCommand(GetVisualPid(fsmOwner), cmd);
            }

            base.OnLeave(fsmOwner, isShutdown);
        }
    }

    public class LocalPlayerStateSkill : LocalPlayerBaseState
    {
        protected override EntityStateType GetStateType()
        {
            return EntityStateType.Skill;
        }

        protected internal override void OnEnter(EntityFsmOwner fsmOwner, FsmEnterParams enterParams = null)
        {
            base.OnEnter(fsmOwner, enterParams);
            {
                ModelCommandSetAnimatorParameter cmd = new ModelCommandSetAnimatorParameter();
                cmd.Datas.Add(new SetAnimatorParameterData
                {
                    ParameterName = AnimationConst.AC_Moving,
                    ParameterValueType = AnimatorParameterValueType.AP_Bool,
                    bValue = false
                });
                cmd.Datas.Add(new SetAnimatorParameterData
                {
                    ParameterName = AnimationConst.AC_Trigger,
                    ParameterValueType = AnimatorParameterValueType.AP_Trigger,
                    bValue = true
                });
                cmd.Datas.Add(new SetAnimatorParameterData
                {
                    ParameterName = AnimationConst.AC_TriggerNumber,
                    ParameterValueType = AnimatorParameterValueType.AP_Int,
                    iValue = (int)AnimationTriggerType.Attack
                });
                cmd.Datas.Add(new SetAnimatorParameterData
                {
                    ParameterName = AnimationConst.AC_Action,
                    ParameterValueType = AnimatorParameterValueType.AP_Int,
                    iValue = (int)AnimationActionAttack.ActionAttack_L1
                });
                GalaxyEntry.GetModule<VisualProxyManager>().VisualCommand(GetVisualPid(fsmOwner), cmd);
            }

            InputProxyInstance.Instance.Attack = false;

            //TODO Randall test
            Timer.RoutineRunner.WaitForSeconds(0.8f, () =>
            {
                fsmOwner.ChangeState<LocalPlayerStateIdle>();
            });
        }

        protected internal override void OnUpdate(EntityFsmOwner fsmOwner, float elapseSeconds)
        {
            base.OnUpdate(fsmOwner, elapseSeconds);
        }

        protected internal override void OnLeave(EntityFsmOwner fsmOwner, bool isShutdown)
        {
            {
                ModelCommandSetAnimatorParameter cmd = new ModelCommandSetAnimatorParameter();
                cmd.Datas.Add(new SetAnimatorParameterData
                {
                    ParameterName = AnimationConst.AC_Trigger,
                    ParameterValueType = AnimatorParameterValueType.AP_Trigger,
                    bValue = false
                });
                cmd.Datas.Add(new SetAnimatorParameterData
                {
                    ParameterName = AnimationConst.AC_TriggerNumber,
                    ParameterValueType = AnimatorParameterValueType.AP_Int,
                    iValue = 0
                });
                cmd.Datas.Add(new SetAnimatorParameterData
                {
                    ParameterName = AnimationConst.AC_Action,
                    ParameterValueType = AnimatorParameterValueType.AP_Int,
                    iValue = 0
                });
                GalaxyEntry.GetModule<VisualProxyManager>().VisualCommand(GetVisualPid(fsmOwner), cmd);
            }
            base.OnLeave(fsmOwner, isShutdown);
        }
    }

    public class LocalPlayerStateDead : LocalPlayerBaseState
    {
        protected override EntityStateType GetStateType()
        {
            return EntityStateType.Dead;
        }

        protected internal override void OnEnter(EntityFsmOwner fsmOwner, FsmEnterParams enterParams = null)
        {
            base.OnEnter(fsmOwner, enterParams);
            {
                GalaxyEntry.GetModule<VisualProxyManager>().VisualCommand(GetVisualPid(fsmOwner), new ModelCommandToggleAnimator { });
            }
        }

        protected internal override void OnLeave(EntityFsmOwner fsmOwner, bool isShutdown)
        {
            {
                GalaxyEntry.GetModule<VisualProxyManager>().VisualCommand(GetVisualPid(fsmOwner), new ModelCommandToggleAnimator { });
            }
            base.OnLeave(fsmOwner, isShutdown);
        }
    }
}
