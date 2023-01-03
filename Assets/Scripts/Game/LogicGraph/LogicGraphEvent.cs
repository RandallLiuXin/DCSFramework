using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Anvil.LogicGraph
{
    public struct LogicGraphEvent : Common.IEventData
    {
        public bool m_NeedHandleEvent;
        public uint m_LogicGraphId;
        public uint m_LogicGraphNodeId;
        public uint m_NodeEvent;
    }

    public interface ILogicGraphInputEvent : Common.IEventData
    {
    }
}
