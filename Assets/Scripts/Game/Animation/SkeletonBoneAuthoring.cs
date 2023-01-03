using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

namespace Anvil.Animation
{
    public class SkeletonBoneAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [RegisterBinding(typeof(SkeletonBone), "BoneIndex")]
        public int BoneIndex = -1;
        [RegisterBinding(typeof(SkeletonBone), "SkeletonVid")]
        public uint SkeletonVid;
        [RegisterBinding(typeof(SkeletonBone), "NeedCache")]
        public bool NeedCache = false;
        [RegisterBinding(typeof(SkeletonBone), "CacheName")]
        public string CacheName;
        [RegisterBinding(typeof(SkeletonBone), "HasHitBox")]
        public bool HasHitBox = false;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            SkeletonBone component = default(SkeletonBone);
            component.BoneIndex = BoneIndex;
            component.SkeletonVid = SkeletonVid;
            component.NeedCache = NeedCache;
            component.CacheName = CacheName;
            component.HasHitBox = HasHitBox;
            dstManager.AddComponentData(entity, component);
        }
    }
}