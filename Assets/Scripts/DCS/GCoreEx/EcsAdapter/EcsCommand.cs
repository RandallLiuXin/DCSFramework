using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Galaxy.Dots
{
    public abstract class EcsCommandBase : IReference
    {
        public abstract void InitOperation(params object[] objects);
        public abstract void RunOperation(EntityCommandBuffer.ParallelWriter ecb, int nativeThreadIndex, Entity entity);
        public abstract void Clear();
    }

    public interface IEcsComponentCommand<T> where T : struct, IComponentData
    {
        void InitOperation(params object[] objects);
        void Execute(EntityCommandBuffer.ParallelWriter ecb, int nativeThreadIndex, Entity entity, T component);
    }

    public class EcsComponentCommand<T> : EcsCommandBase, IEcsComponentCommand<T> where T : struct, IComponentData
    {
        private bool m_HasInitialized;
        private T m_Data;

        public override void InitOperation(params object[] objects)
        {
            Debug.Assert(objects != null && objects.Length == 1);
            m_Data = (T)objects[0];
            m_HasInitialized = true;
        }

        public override void RunOperation(EntityCommandBuffer.ParallelWriter ecb, int nativeThreadIndex, Entity entity)
        {
            Debug.Assert(m_HasInitialized);
            Execute(ecb, nativeThreadIndex, entity, m_Data);
        }
        public void Execute(EntityCommandBuffer.ParallelWriter ecb, int nativeThreadIndex, Entity entity, T component)
        {
            ecb.SetComponent(nativeThreadIndex, entity, component);
        }

        public override void Clear()
        {
            m_HasInitialized = false;
            m_Data = default;
        }
    }

    public interface IEcsBufferCommand<T> where T : struct, IBufferElementData
    {
        void InitOperation(params object[] objects);
        void Execute(EntityCommandBuffer.ParallelWriter ecb, int nativeThreadIndex, Entity entity, T component);
    }

    public class EcsBufferCommand<T> : EcsCommandBase, IEcsBufferCommand<T> where T : struct, IBufferElementData
    {
        private bool m_HasInitialized;
        private T m_Data;

        public override void InitOperation(params object[] objects)
        {
            Debug.Assert(objects != null && objects.Length == 1);
            m_Data = (T)objects[0];
            m_HasInitialized = true;
        }

        public override void RunOperation(EntityCommandBuffer.ParallelWriter ecb, int nativeThreadIndex, Entity entity)
        {
            Debug.Assert(m_HasInitialized);
            Execute(ecb, nativeThreadIndex, entity, m_Data);
        }
        public void Execute(EntityCommandBuffer.ParallelWriter ecb, int nativeThreadIndex, Entity entity, T component)
        {
            ecb.AppendToBuffer<T>(nativeThreadIndex, entity, component);
        }

        public override void Clear()
        {
            m_HasInitialized = false;
            m_Data = default;
        }
    }
}

