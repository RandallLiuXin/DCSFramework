using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Anvil.Animation
{
    public struct AnimationComponent : IComponentData
    {
    }

    public struct AnimationEventInfo : IBufferElementData
    {
        public Galaxy.Visual.AnimationEventCallback CallbackType;
        public int CallbackIntValue;
        public float CallbackFloatValue;
    }

    public struct AnimationRootMotion : IComponentData
    {
        public Vector3 DeltaPosition;
        public Quaternion DeltaRotation;
    }
}
