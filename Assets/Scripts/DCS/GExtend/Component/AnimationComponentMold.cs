using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Galaxy.Entities;
using Galaxy.Mold;

namespace Galaxy.Visual
{
    [GalaxyComponent("AnimationComponent", CompType.Animation)]
    public class AnimationComponentMold : MoldBase
    {
        [GalaxyProperty()]
        public int AnimatorType;
    }
}
