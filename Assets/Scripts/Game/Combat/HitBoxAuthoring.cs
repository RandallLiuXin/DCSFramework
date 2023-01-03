using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using Anvil.Animation;
using Unity.Jobs;
using Anvil.DCS;
using Unity.Transforms;

namespace Anvil.Combat
{
    public struct HitBox : IComponentData
    {
        public int BoneIndex;
        public float3 ColliderShape;
        public float3 ColliderOffset;
        public PhysicsCategoryTags ColliderBelongsTo;
        public PhysicsCategoryTags ColliderCollidesWith;
    }

    public struct HitBoxTag : IComponentData
    {
        public Entity logicEntity;
        public Entity parentEntity;
    }

    [RequireComponent(typeof(SkeletonBoneAuthoring))]
    public class HitBoxAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float3 ColliderShape;
        public float3 ColliderOffset;
        public PhysicsCategoryTags ColliderBelongsTo;
        public PhysicsCategoryTags ColliderCollidesWith;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var skeletionBoneData = GetComponent<SkeletonBoneAuthoring>();
            Debug.Assert(skeletionBoneData != null && skeletionBoneData.BoneIndex >= 0);

            HitBox component = default(HitBox);
            component.BoneIndex = skeletionBoneData.BoneIndex;

            component.ColliderShape = ColliderShape;
            component.ColliderOffset = ColliderOffset;
            component.ColliderBelongsTo = ColliderBelongsTo;
            component.ColliderCollidesWith = ColliderCollidesWith;
            dstManager.AddComponentData(entity, component);
        }
    }

    [UpdateInGroup(typeof(Common.CombatSystemGroup), OrderFirst = true)]
    public unsafe partial class HitBoxSystem : SystemBase
    {
        //EndSimulationEntityCommandBufferSystem m_EndSimulationECBSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            //m_EndSimulationECBSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<DCSInitializeTag, HitBoxComponent, DCSEntityTag>().ForEach((Entity entity, ref DCSInitializeTag initializeTag, ref DynamicBuffer<HitBoxInfo> hitBoxInfos) =>
            {
                if (initializeTag.m_HasInitHitBox)
                {
                    return;
                }

                initializeTag.m_HasInitHitBox = true;
                var childFromEntity = GetBufferFromEntity<Child>(true);
                List<Entity> searchList = new List<Entity> { entity };
                while (searchList.Count != 0)
                {
                    var currentEntity = searchList[0];
                    searchList.RemoveAt(0);

                    if (childFromEntity.HasComponent(currentEntity))
                    {
                        var children = childFromEntity[currentEntity];
                        foreach (var child in children)
                        {
                            searchList.Add(child.Value);
                        }
                    }

                    if (HasComponent<Animation.SkeletonBone>(currentEntity))
                    {
                        var SkeletonBoneData = GetComponentDataFromEntity<Animation.SkeletonBone>(true);
                        var data = SkeletonBoneData[currentEntity];
                        if (data.HasHitBox)
                        {
                            hitBoxInfos.Add(new HitBoxInfo { Socket = currentEntity });
                        }
                    }
                }
            }).Run();

            //deprecated code
//            var commandBuffer = m_EndSimulationECBSystem.CreateCommandBuffer().AsParallelWriter();

//            Entities.WithAll<HitBoxTag>().WithoutBurst().ForEach((Entity entity, int nativeThreadIndex, ref Translation translation, ref Rotation rotation, in HitBoxTag entityTag) =>
//            {
//                if (!EntityManager.Exists(entityTag.logicEntity) || !EntityManager.Exists(entityTag.parentEntity))
//                {
//                    commandBuffer.DestroyEntity(nativeThreadIndex, entity);
//                    return;
//                }
//                var parentTranslation = EntityManager.GetComponentData<Translation>(entityTag.parentEntity);
//                var parentRotation = EntityManager.GetComponentData<Rotation>(entityTag.parentEntity);
//                translation.Value = parentTranslation.Value;
//                rotation.Value = parentRotation.Value;
//            }).Run();

//            Entities.WithAll<HitBoxComponent, DCSEntityTag>().WithoutBurst().ForEach((Entity entity, int nativeThreadIndex, in DCSEntityTag entityTag) =>
//            {
//                if (entityTag.EntityVid == Galaxy.CoreConst.INVAILD_VID)
//                    return;

//                var childFromEntity = GetBufferFromEntity<Child>(true);
//                List<Entity> searchList = new List<Entity> { entity };
//                while (searchList.Count != 0)
//                {
//                    var currentEntity = searchList[0];
//                    searchList.RemoveAt(0);

//                    if (EntityManager.HasComponent<Child>(currentEntity))
//                    {
//                        var children = childFromEntity[currentEntity];
//                        foreach (var child in children)
//                        {
//                            searchList.Add(child.Value);
//                        }
//                    }

//                    if (EntityManager.HasComponent<HitBox>(currentEntity))
//                    {
//                        var hitbox = EntityManager.GetComponentData<HitBox>(currentEntity);

//                        var hitBoxEntity = commandBuffer.CreateEntity(nativeThreadIndex);
//                        commandBuffer.AddComponent(nativeThreadIndex, hitBoxEntity, new Translation { Value = float3.zero });
//                        commandBuffer.AddComponent(nativeThreadIndex, hitBoxEntity, new Rotation { Value = quaternion.identity });
//                        commandBuffer.AddComponent(nativeThreadIndex, hitBoxEntity, new LocalToWorld());
//#if UNITY_EDITOR
//                        commandBuffer.SetName(nativeThreadIndex, hitBoxEntity, entity.Index.ToString() + " Hitbox");
//#endif
//                        var colliderFilter = CollisionFilter.Default;
//                        colliderFilter.BelongsTo = hitbox.ColliderBelongsTo.Value;
//                        colliderFilter.CollidesWith = hitbox.ColliderCollidesWith.Value;
//                        var colliderMaterial = Unity.Physics.Material.Default;
//                        var colliderBlob = Unity.Physics.BoxCollider.Create(new BoxGeometry
//                        {
//                            BevelRadius = 0.001f,
//                            Center = hitbox.ColliderOffset,
//                            Orientation = quaternion.identity,
//                            Size = hitbox.ColliderShape
//                        }, colliderFilter, colliderMaterial);
//                        var colliderMass = new MassProperties();
//                        commandBuffer.AddComponent(nativeThreadIndex, hitBoxEntity, new PhysicsCollider { Value = colliderBlob });
//                        commandBuffer.AddComponent(nativeThreadIndex, hitBoxEntity, PhysicsMass.CreateKinematic(colliderMass));
//                        commandBuffer.AddComponent(nativeThreadIndex, hitBoxEntity, new HitBoxTag { logicEntity = entity, parentEntity = currentEntity });

//                        commandBuffer.RemoveComponent<HitBox>(nativeThreadIndex, currentEntity);
//                    }
//                }
//            }).Run();

//            m_EndSimulationECBSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
