using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Anvil.Property
{
    [GenerateAuthoringComponent]
    public struct PropertyComponent : IComponentData
    {
        public float MaxHp;
        public float Hp;

        public float Attack;
        public float Defense;
    }
}

