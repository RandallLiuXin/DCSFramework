using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;

namespace Anvil.Combat
{
    public struct HitBoxComponent : IComponentData
    {
    }

    public struct HitBoxInfo : IBufferElementData
    {
        public Entity Socket;
    }
}
