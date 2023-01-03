using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Galaxy;
using Galaxy.Visual;
using Galaxy.Visual.Command;
using Unity.Physics;

namespace Anvil.Animation
{
    //TODO Randall 事件系统需要分类处理，目前先用logicgraph的
    [UpdateInGroup(typeof(DCS.DCSSystemGroup)), UpdateAfter(typeof(DCS.DCSCommandHandlerSystem))]
    public partial class AnimationSystem : LogicGraph.LogicGraphSystemBase
    {
        public static Dictionary<uint, List<Transform>> m_QueryResult = new Dictionary<uint, List<Transform>>();

        private BeginSimulationEntityCommandBufferSystem m_BeginSimulationECBSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            m_BeginSimulationECBSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!GalaxyEntry.HasInitialized())
            {
                return;
            }

            #region Init

            Entities.WithAll<AnimationComponent, DCS.DCSInitializeTag, DCS.DCSEntityTag>().WithoutBurst().ForEach((Entity entity, ref AnimationComponent animationData, ref DCS.DCSInitializeTag initializeTag, in DCS.DCSEntityTag entityTag) =>
            {
                if (initializeTag.m_HasInitAnimation)
                {
                    return;
                }

                if (entityTag.EntityUid == CoreConst.INVAILD_UID && entityTag.EntityVid == CoreConst.INVAILD_VID)
                {
                    return;
                }

                initializeTag.m_HasInitAnimation = true;

                var childFromEntity = GetBufferFromEntity<Child>(true);
                List<Entity> searchList = new List<Entity> { entity };
                while (searchList.Count != 0)
                {
                    var currentEntity = searchList[0];
                    searchList.RemoveAt(0);

                    if (EntityManager.HasComponent<Child>(currentEntity))
                    {
                        var children = childFromEntity[currentEntity];
                        foreach (var child in children)
                        {
                            searchList.Add(child.Value);
                        }
                    }

                    if (EntityManager.HasComponent<SkeletonBone>(currentEntity))
                    {
                        var data = EntityManager.GetComponentData<SkeletonBone>(currentEntity);
                        data.HasInitialized = true;
                        data.SkeletonVid = entityTag.EntityVid;
                        EntityManager.SetComponentData(currentEntity, data);

                        if (data.NeedCache && EntityManager.HasComponent<SkeletonCacheInfo>(entity))
                        {
                            var cacheBuffer = EntityManager.GetBuffer<SkeletonCacheInfo>(entity);
                            cacheBuffer.Add(new SkeletonCacheInfo { CacheEntity = currentEntity, SocketName = data.CacheName });
                        }
                    }
                }
            }).Run();

            #endregion

            #region Animation

            //Animation bone
            m_QueryResult.Clear();

            Entities.WithAll<AnimationComponent, DCS.DCSEntityTag>().WithNone<DCS.DCSInitializeTag>().WithoutBurst().ForEach((Entity entity, ref AnimationComponent animationData, in DCS.DCSEntityTag entityTag) =>
            {
                VisualQueryGetSkeleton query = new VisualQueryGetSkeleton();
                var results = GalaxyEntry.GetModule<VisualProxyManager>().VisualQuery(entityTag.EntityVid, query);
                // visual not ready
                if (results == null)
                {
                    return;
                }
                Debug.Assert(results != null && results.Length == 1);
                List<Transform> skeleton = results[0] as List<Transform>;
                Debug.Assert(skeleton != null && skeleton.Count != 0);
                m_QueryResult[entityTag.EntityVid] = skeleton;
            }).Run();

            Entities.WithAll<SkeletonBone, LocalToParent>().WithoutBurst().ForEach((Entity entity, ref LocalToParent localToParent, in SkeletonBone skeletonBoneData) =>
            {
                uint vid = skeletonBoneData.SkeletonVid;
                if (!skeletonBoneData.HasInitialized)
                    return;

                if (!m_QueryResult.ContainsKey(vid))
                    return;

                var resultList = m_QueryResult[vid];
                var boneTf = resultList[skeletonBoneData.BoneIndex];

                localToParent.Value = float4x4.TRS(boneTf.localPosition, boneTf.localRotation, boneTf.localScale);
            }).Run();

            #endregion
        }
    }
}
