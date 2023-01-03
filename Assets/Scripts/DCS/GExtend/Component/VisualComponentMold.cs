using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Galaxy.Entities;
using Galaxy.Mold;

namespace Galaxy.Visual
{
    [GalaxyComponent("VisualComponent", CompType.Visual)]
    public class VisualComponentMold : MoldBase
    {
        [GalaxyProperty()]
        public string ModelPath;
        [GalaxyProperty()]
        public uint VisualPid;
        [GalaxyProperty()]
        public Vector3 VisualPos;
        [GalaxyProperty()]
        public Quaternion VisualRot;
        [GalaxyProperty()]
        public uint VisualType;
    }
}
