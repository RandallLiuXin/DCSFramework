using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Anvil.Combat
{
    public struct HurtBoxComponent : IComponentData
    {

    }

    public struct HurtBoxInfo : IBufferElementData
    {
        public ColliderType Type;
        public float3 Center;
        public quaternion CollisionOrientation;
        public float ColliderArg1;
        public float ColliderArg2;
        public float ColliderArg3;

        public float3 StartPos;
        public float3 EndPos;
        public quaternion Orientation;
    }

    public struct HurtBoxResult : IBufferElementData
    {
        public Entity TargetEntity;
        public ColliderCastHit ColliderResult;
    }
}
