using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace Anvil.Movement
{
    public partial class MovementSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            float deltaTime = Time.DeltaTime;
            Entities.WithAll<Translation, Rotation, MovementComponent>().ForEach((ref Translation translation, ref Rotation rotation, in MovementComponent movement) =>
            {
                translation.Value += movement.m_MoveVelocity * deltaTime;
                rotation.Value = math.slerp(rotation.Value, movement.m_TargetRotation, movement.m_RotationSpeed * deltaTime);
            }).ScheduleParallel();
        }
    }
}
