using Galaxy.Fsm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityFsmOwner = Galaxy.Fsm.IFsm<Galaxy.Entities.EntityFsm>;
using Galaxy.Visual;
using Galaxy.Visual.Command;

namespace Galaxy.Entities
{
    #region Enemy
    public abstract class EnemyBaseState : EntityBaseState
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
            fsm.ChangeState<EntityStateDead>();
        }
    }

    public class EntityStateIdle : EnemyBaseState
    {
        protected override EntityStateType GetStateType()
        {
            return EntityStateType.Idle;
        }
    }

    public class EntityStateMoveEnterParams : FsmEnterParams
    {
        public EntityStateMoveEnterParams()
        {
        }

        public override void Clear()
        {
        }
    }

    public class EntityStateMoving : EnemyBaseState
    {
        protected const string MoveVelocityStr = "MoveVelocity";

        protected override EntityStateType GetStateType()
        {
            return EntityStateType.Moving;
        }

        protected internal override void OnEnter(EntityFsmOwner fsmOwner, FsmEnterParams enterParams = null)
        {
            base.OnEnter(fsmOwner, enterParams);
        }

        protected internal override void OnUpdate(EntityFsmOwner fsmOwner, float elapseSeconds)
        {
            base.OnUpdate(fsmOwner, elapseSeconds);
        }

        protected internal override void OnLeave(EntityFsmOwner fsmOwner, bool isShutdown)
        {
            base.OnLeave(fsmOwner, isShutdown);
        }
    }

    public class EntityStateDead : EnemyBaseState
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
    #endregion
}
