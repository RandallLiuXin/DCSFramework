using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using BovineLabs.Event.Systems;

namespace Anvil.LogicGraph
{
    public abstract partial class LogicGraphSystemBase : SystemBase
    {
        protected EventSystem m_EventSystem;
        protected override void OnCreate()
        {
            m_EventSystem = World.GetOrCreateSystem<LogicGraphEventSystem>();
            base.OnCreate();
        }
    }
}
