using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anvil.Common
{
    public class PhysicsConst
    {
        public class PhysicsLayer
        {
            public static uint HurtBox = (uint)1 << 30;
            public static uint HitBox = (uint)1 << 31;
        }
    }
}
