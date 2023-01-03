using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Anvil.Player
{
    public struct PlayerInput : IComponentData
    {
        public bool m_NeedRefreshInput;
        public float3 m_InputVector;
    }
}
