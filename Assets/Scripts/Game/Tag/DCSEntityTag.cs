using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Anvil.DCS
{
    [GenerateAuthoringComponent]
    public struct DCSEntityTag : IComponentData
    {
        public uint EntityUid;
        public uint EntityVid;
    }
}
