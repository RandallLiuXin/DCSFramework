using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Galaxy.Entities
{
    public class ComponentDefine
    {
        public static Type[] CompTypes = new Type[(int)CompType.Count]
        {
            typeof(Visual.VisualComponent),
            typeof(Visual.AnimationComponent),
        };
        public static Type[] CompProxyTypes = new Type[(int)CompType.Count]
        {
            typeof(Visual.VisualComponentProxy),
            typeof(Visual.AnimationComponentProxy),
        };
    }

    public enum CompType
    {
        Visual,
        Animation,
        Count,
    }
}
