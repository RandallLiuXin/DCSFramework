using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Anvil.LogicGraph;
using Unity.Mathematics;
using Unity.Collections;
using BovineLabs.Event.Systems;
using BovineLabs.Event.Containers;
using BovineLabs.Event.Jobs;

namespace Anvil.Property
{
    [UpdateInGroup(typeof(Common.CombatSystemGroup)), UpdateAfter(typeof(Combat.HurtBoxSystem))]
    public partial class DamageFormulaSystem : LogicGraphSystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public struct BeHitInfo
        {
            public Entity m_SourceEntity;
            public float m_Damage;
        }

        protected override void OnUpdate()
        {
            var dependency = Dependency;
            var damageEvents = new NativeParallelMultiHashMap<Entity, BeHitInfo>(0, Allocator.TempJob);

            dependency = new EffectDamageJob
            {
                damageEvents = damageEvents
            }.Schedule<EffectDamageJob, EffectDamageEvent>(m_EventSystem, dependency);

            var writer = m_EventSystem.CreateEventWriter<State.DeadStateEvent>();
            //TODO Randall 能否通过判断语句动态调用这里的代码 //if (damageEvents.Count() > 0)
            dependency = Entities.WithAll<PropertyComponent, PropertyChangeInfoComponent>().WithNone<DCS.DCSInitializeTag>().WithoutBurst().WithReadOnly(damageEvents).ForEach((Entity entity, ref PropertyComponent propertyData) =>
            {
                if (propertyData.Hp <= 0)
                    return;

                if (!damageEvents.ContainsKey(entity))
                    return;

                foreach (var beHitInfo in damageEvents.GetValuesForKey(entity))
                {
                    if (propertyData.Hp > beHitInfo.m_Damage)
                    {
                        propertyData.Hp -= beHitInfo.m_Damage;
                    }
                    else
                    {
                        propertyData.Hp = 0;
                        Debug.Log("Target dead");
                        writer.Write(new State.DeadStateEvent { entity = entity });
                    }
                }
            }).Schedule(dependency);
            m_EventSystem.AddJobHandleForProducer<State.DeadStateEvent>(dependency);

            Dependency = damageEvents.Dispose(dependency);
        }
    }

    //[BurstCompile]
    public struct EffectDamageJob : IJobEvent<EffectDamageEvent>
    {
        public NativeParallelMultiHashMap<Entity, DamageFormulaSystem.BeHitInfo> damageEvents;

        public void Execute(EffectDamageEvent e)
        {
            damageEvents.Add(e.m_TargetEntity, new DamageFormulaSystem.BeHitInfo { m_Damage = e.m_Damage, m_SourceEntity = e.m_Owner });
            //Debug.Log("EffectDamageJob: " + damageEvents.Count() + " " + e.m_Damage);
        }
    }

    public struct EffectDamageEvent : ILogicGraphInputEvent
    {
        public Entity m_Owner;

        public Entity m_TargetEntity;
        public float m_Damage;
    }

    //public struct DamageInfo : ILogicGraphInputEvent
    //{
    //    public Entity m_Owner;

    //    public float TotalDamage;
    //    public float3 HitPos;
    //    public quaternion HitRot;
    //    public float HitForce;
    //}
}
