using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Anvil.Animation;

namespace Anvil.Player
{
    public partial class PlayerControllerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var dependency = Dependency;
            dependency = Entities.WithAll<PlayerInput, CharacterControllerInternalData, PlayerControllerComponent>().ForEach((ref PlayerInput playerInput, ref CharacterControllerInternalData ccData, in PlayerControllerComponent playerController) =>
            {
                if (playerInput.m_NeedRefreshInput)
                {
                    playerInput.m_NeedRefreshInput = false;
                    ccData.Input.Movement = new float2(playerInput.m_InputVector.x, playerInput.m_InputVector.z);
                    ccData.Input.Looking = Quaternion.LookRotation(playerInput.m_InputVector).eulerAngles.y * Mathf.Deg2Rad;
                }
            }).ScheduleParallel(dependency);

            dependency = Entities.WithAll<AnimationRootMotion, CharacterControllerInternalData, PlayerControllerComponent>().ForEach((ref AnimationRootMotion rootMotion, ref CharacterControllerInternalData ccData, in PlayerControllerComponent playerController) =>
            {
                if (math.abs(rootMotion.DeltaPosition.x) < float.Epsilon 
                    && math.abs(rootMotion.DeltaPosition.y) < float.Epsilon
                    && math.abs(rootMotion.DeltaPosition.z) < float.Epsilon)
                {
                    return;
                }

                ccData.Input.RootMotionDeltaPos = rootMotion.DeltaPosition;

                rootMotion.DeltaPosition.x = 0;
                rootMotion.DeltaPosition.y = 0;
                rootMotion.DeltaPosition.z = 0;
            }).ScheduleParallel(dependency);

            Dependency = dependency;
        }
    }
}
