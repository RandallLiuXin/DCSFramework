using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using System.Collections;
using System.Collections.Generic;

namespace Anvil.State
{
    public struct StateComponent : IComponentData
    {
        public Common.StatefulProperty<bool> DeadFlag;
    }

    public struct DeadStateEvent : Common.IEventData
    {
        public Entity entity;
    }

    //TODO Randall 事件系统需要分类处理，目前先用logicgraph的
    [UpdateInGroup(typeof(Common.CombatSystemGroup)), UpdateAfter(typeof(Property.DamageFormulaSystem))]
    public partial class StateSystem : LogicGraph.LogicGraphSystemBase
    {
        private bool m_NeedRefresh;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_NeedRefresh = false;
        }

        protected override void OnUpdate()
        {
            var dependency = Dependency;

            if (m_NeedRefresh)
            {
                Entities.WithAll<StateComponent>().ForEach((Entity entity, ref StateComponent stateData) =>
                {
                    if (!stateData.DeadFlag.IsTouch())
                        return;

                    stateData.DeadFlag.UnTouch();
                }).Run();
                m_NeedRefresh = false;
            }

            if (m_EventSystem.HasEventReaders<DeadStateEvent>())
            {
                var handle = m_EventSystem.GetEventReaders<DeadStateEvent>(default, out var readers);
                m_EventSystem.AddJobHandleForConsumer<DeadStateEvent>(handle); // necessary evil
                handle.Complete();

                List<Entity> entities = new List<Entity>();

                foreach (var reader in readers)
                {
                    for (int i = 0; i < reader.ForEachCount; i++)
                    {
                        var count = reader.BeginForEachIndex(i);
                        for (int j = 0; j < count; j++)
                        {
                            DeadStateEvent stateEvent = reader.Read<DeadStateEvent>();
                            entities.Add(stateEvent.entity);
                        }
                        reader.EndForEachIndex();
                    }
                }

                if (entities.Count != 0)
                {
                    NativeArray<Entity> deadEntities = new NativeArray<Entity>(entities.ToArray(), Allocator.TempJob);
                    dependency = Entities.WithAll<StateComponent>().WithReadOnly(deadEntities).ForEach((Entity entity, ref StateComponent stateData) =>
                    {
                        if (!deadEntities.Contains(entity))
                            return;

                        stateData.DeadFlag.SetValue(true);
                    }).ScheduleParallel(dependency);

                    dependency = deadEntities.Dispose(dependency);
                }

                m_NeedRefresh = true;
                entities.Clear();
            }

            Dependency = dependency;
        }
    }
}
