using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Galaxy.Entities
{
    public sealed class SystemDefine
    {
        public static Type[] SystemTypes = new Type[(int)SystemType.Count]
        {
            typeof(Visual.VisualSystem),
            typeof(Visual.AnimationSystem),
        };

        public static Type[] SystemProxyTypes = new Type[(int)SystemType.Count]
        {
            typeof(Visual.VisualSystemProxy),
            typeof(Visual.AnimationSystemProxy),
        };
    }

    public enum SystemType
    {
        Visual,
        Animation,
        Count, //¼ÆÊý
    }

    public enum SystemPriority
    {

    }
}
