using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Anvil.Movement
{
    [GenerateAuthoringComponent]
    public struct MovementComponent : IComponentData
    {
        public float3 m_MoveVelocity;

        public quaternion m_TargetRotation;
        public float m_RotationSpeed;
    }
}
