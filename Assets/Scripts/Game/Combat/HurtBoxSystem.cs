using System;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Physics.Authoring;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Entities;
using Unity.Jobs;
using Anvil.DCS;
using Anvil.LogicGraph;

namespace Anvil.Combat
{
    [UpdateInGroup(typeof(Common.CombatSystemGroup)), UpdateAfter(typeof(HitBoxSystem))]
    public unsafe partial class HurtBoxSystem : LogicGraphSystemBase
    {
#if UNITY_EDITOR
        DebugStream m_DebugStreamSystem;
#endif

        protected override void OnCreate()
        {
            base.OnCreate();
#if UNITY_EDITOR
            m_DebugStreamSystem = World.GetOrCreateSystem<DebugStream>();
#endif
        }

        protected override void OnUpdate()
        {
            var hitBoxes = new NativeParallelHashMap<Entity, Entity>(0, Allocator.TempJob);
            var factionMap = new NativeParallelHashMap<Entity, uint>(0, Allocator.TempJob);

            Entities.WithAll<HitBoxComponent, DCSEntityTag>().WithoutBurst().ForEach((Entity entity, in DynamicBuffer<HitBoxInfo> hitBoxInfos) =>
            {
                //UnityEngine.Debug.Log("hitbox update: " + entity.Index);
                foreach (var item in hitBoxInfos)
                {
                    bool ret = hitBoxes.TryAdd(item.Socket, entity);
                    UnityEngine.Debug.Assert(ret);
                }
            }).Run();

            Entities.WithAll<HurtBoxComponent>().WithoutBurst().WithReadOnly(hitBoxes).ForEach((Entity entity, ref DynamicBuffer<HurtBoxInfo> HurtBoxes, ref DynamicBuffer<HurtBoxResult> hurtBoxResults, in HurtBoxComponent hurtComponent) =>
            {
                if (HurtBoxes.IsEmpty)
                    return;

                //UnityEngine.Debug.Log("hurtbox update: " + Time.ElapsedTime + " " + entity.Index);
                var world = World.GetExistingSystem<BuildPhysicsWorld>().PhysicsWorld;

                var filter = new CollisionFilter { BelongsTo = Common.PhysicsConst.PhysicsLayer.HurtBox, CollidesWith = Common.PhysicsConst.PhysicsLayer.HitBox };

                foreach (var item in HurtBoxes)
                {
                    NativeList<ColliderCastHit> ColliderCastHits = new NativeList<ColliderCastHit>(Allocator.Temp);
                    BlobAssetReference<Collider> collider = default;
                    switch (item.Type)
                    {
                        case ColliderType.Sphere:
                            collider = SphereCollider.Create(new SphereGeometry
                            {
                                Center = item.Center,
                                Radius = item.ColliderArg1
                            }, filter);
                            break;
                        case ColliderType.Box:
                            collider = BoxCollider.Create(new BoxGeometry
                            {
                                Center = item.Center,
                                Orientation = item.CollisionOrientation,
                                Size = new float3(item.ColliderArg1, item.ColliderArg2, item.ColliderArg3),
                                BevelRadius = 0.0f
                            }, filter);
                            break;
                        default:
                            throw new NotImplementedException();
                    }

                    ColliderCastInput colliderCastInput = new ColliderCastInput(collider, item.StartPos, item.EndPos, item.Orientation);
                    world.CastCollider(colliderCastInput, ref ColliderCastHits);

                    //UnityEngine.Debug.Log("hurtbox foreach: " + Time.ElapsedTime + " " + entity.Index + " " + item.StartPos + " " + item.EndPos + " " + ColliderCastHits.IsEmpty);
                    foreach (var colliderResult in ColliderCastHits)
                    {
                        //UnityEngine.Debug.Log(colliderResult.ColliderKey.ToString() + " " + colliderResult.Entity.ToString());
                        if (hitBoxes.ContainsKey(colliderResult.Entity))
                        {
                            //UnityEngine.Debug.Log("Attack target: " + colliderResult.ColliderKey.ToString() + " " + hitBoxes[colliderResult.Entity]);
                            hurtBoxResults.Add(new HurtBoxResult { TargetEntity = hitBoxes[colliderResult.Entity], ColliderResult = colliderResult });
                        }
                    }
                    ColliderCastHits.Dispose();

#if UNITY_EDITOR
                    var context = m_DebugStreamSystem.GetContext(1);
                    context.Begin(0);
                    var colliderPtr = (BoxCollider*)colliderCastInput.Collider;
                    context.Box(colliderPtr->Size, colliderPtr->Center + colliderCastInput.Start, colliderCastInput.Orientation, UnityEngine.Color.green);
                    context.End();
#endif
                }

                HurtBoxes.Clear();
            }).Run();

            {
                var factionEntity = GetSingletonEntity<FactionTag>();
                var factionBuffer = GetBufferFromEntity<FactionData>(true)[factionEntity];
                foreach (var item in factionBuffer)
                {
                    factionMap.Add(item.FactionEntity, item.FactionId);
                }
            }

            Entities.WithAll<HurtBoxComponent>().WithoutBurst().WithReadOnly(factionMap).ForEach((Entity entity, ref DynamicBuffer<HurtBoxResult> hurtBoxResults, in HurtBoxComponent hurtComponent) =>
            {
                if (hurtBoxResults.IsEmpty)
                    return;

                if (!HasSingleton<FactionTag>())
                    return;

                if (!factionMap.ContainsKey(entity))
                {
                    UnityEngine.Debug.Log("factionData haven't created?");
                    return;
                }

                uint currentFaction = factionMap[entity];
                var hitResults = new NativeParallelHashMap<Entity, ColliderCastHit>(0, Allocator.TempJob);
                foreach (var result in hurtBoxResults)
                {
                    var targetEntity = result.TargetEntity;
                    if (!factionMap.ContainsKey(targetEntity))
                        continue;
                    if (!FactionHelper.IsEnemy(currentFaction, factionMap[targetEntity]))
                        continue;

                    var colliderCastHit = result.ColliderResult;
                    if (hitResults.ContainsKey(targetEntity))
                    {
                        if (hitResults[targetEntity].Fraction > colliderCastHit.Fraction)
                        {
                            continue;
                        }

                        hitResults[targetEntity] = colliderCastHit;
                    }
                    else
                    {
                        hitResults.Add(targetEntity, colliderCastHit);
                    }
                }
                hurtBoxResults.Clear();

                //UnityEngine.Debug.Log("hitResults is: " + hitResults.Count().ToString());
                var writer = m_EventSystem.CreateEventWriter<Property.EffectDamageEvent>();
                foreach (var result in hitResults)
                {
                    //TODO Randall ÉËº¦¸Ä³É¶Á±í
                    writer.Write(new Property.EffectDamageEvent { m_Owner = entity, m_Damage = 1, m_TargetEntity = result.Key });
                }
                hitResults.Dispose();
                m_EventSystem.AddJobHandleForProducer<Property.EffectDamageEvent>(Dependency);
            }).Run();

            Dependency = factionMap.Dispose(Dependency);
            Dependency = hitBoxes.Dispose(Dependency);
        }

        protected override void OnDestroy()
        {
#if UNITY_EDITOR
            m_DebugStreamSystem = null;
#endif
        }

    }
}
