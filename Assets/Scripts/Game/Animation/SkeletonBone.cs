using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;

namespace Anvil.Animation
{
    public struct SkeletonBone : IComponentData
    {
        public bool HasInitialized;
        public int BoneIndex;
        public uint SkeletonVid;

        public bool NeedCache;
        public FixedString32Bytes CacheName;

        public bool HasHitBox;
    }

    public struct SkeletonCacheInfo : IBufferElementData
    {
        public FixedString32Bytes SocketName;
        public Entity CacheEntity;
    }
    
    public struct RagdollBone : IComponentData
    {
        public bool HasInitialized;
        public int BoneIndex;
        public uint SkeletonVid;
    }
}
