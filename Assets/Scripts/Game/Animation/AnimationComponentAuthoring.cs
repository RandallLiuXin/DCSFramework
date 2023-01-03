using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Anvil.Animation
{
    public class AnimationComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new AnimationComponent());
            dstManager.AddComponentData(entity, new AnimationRootMotion());
            dstManager.AddBuffer<AnimationEventInfo>(entity);
            dstManager.AddBuffer<SkeletonCacheInfo>(entity);
        }
    }
}