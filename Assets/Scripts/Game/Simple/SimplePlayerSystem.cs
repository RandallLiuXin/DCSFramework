using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Anvil.Animation;
using Anvil.Combat;
using BovineLabs.Event;
using BovineLabs.Event.Systems;

namespace Anvil.Prototype
{
    [UpdateInGroup(typeof(Common.CombatSystemGroup), OrderLast = true)]
    public partial class SimplePlayerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<SimplePlayerComponent, AnimationComponent>().ForEach((Entity entity, ref SimplePlayerComponent simplePlayer, ref DynamicBuffer<AnimationEventInfo> animationEvents) =>
            {
                foreach (var eventInfo in animationEvents)
                {
                    switch (eventInfo.CallbackType)
                    {
                        case Galaxy.Visual.AnimationEventCallback.AECB_Hit:
                            //Debug.Log("Create hurt box");
                            var cacheBuffer = GetBufferFromEntity<SkeletonCacheInfo>();
                            var localToWorldBuffer = GetComponentDataFromEntity<LocalToWorld>(true);
                            var hurtBoxesBuffer = GetBufferFromEntity<HurtBoxInfo>();

                            foreach (var item in cacheBuffer[entity])
                            {
                                if (item.SocketName != "Hand_L")
                                    continue;

                                //Debug.Log(item.SocketName);
                                var localToWorld = localToWorldBuffer[item.CacheEntity];

                                var hurtBoxes = hurtBoxesBuffer[entity];
                                hurtBoxes.Add(new HurtBoxInfo
                                {
                                    Type = Unity.Physics.ColliderType.Box,
                                    Center = float3.zero,
                                    CollisionOrientation = quaternion.identity,
                                    ColliderArg1 = 0.5f,
                                    ColliderArg2 = 0.5f,
                                    ColliderArg3 = 0.5f,

                                    StartPos = localToWorld.Position,
                                    EndPos = localToWorld.Position,
                                    Orientation = localToWorld.Rotation,
                                });
                            }
                            break;
                        case Galaxy.Visual.AnimationEventCallback.AECB_FootL:
                            //Debug.Log("Handle animation event: AECB_FootL");
                            break;
                        case Galaxy.Visual.AnimationEventCallback.AECB_FootR:
                            //Debug.Log("Handle animation event: AECB_FootR");
                            break;
                        case Galaxy.Visual.AnimationEventCallback.AECB_Land:
                            break;
                        default: 
                            throw new NotImplementedException();
                    }
                }
                animationEvents.Clear();
            }).Run();
        }
    }
}
