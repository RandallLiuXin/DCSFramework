using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BovineLabs.Event.Systems;

namespace Anvil.LogicGraph
{
    public class LogicGraphEventSystem : EventSystem
    {
        public override bool UsePersistentAllocator => true;

        protected override WorldMode Mode => WorldMode.DefaultWorldName;
    }
}
