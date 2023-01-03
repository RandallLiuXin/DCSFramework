using UnityEngine;
using Unity.Entities;

namespace Anvil.Animation
{
    public class RagdollBoneAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        [RegisterBinding(typeof(RagdollBone), "BoneIndex")]
        public int BoneIndex = -1;
        [RegisterBinding(typeof(RagdollBone), "SkeletonVid")]
        public uint SkeletonVid;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            RagdollBone component = default(RagdollBone);
            component.HasInitialized = false;
            component.BoneIndex = BoneIndex;
            component.SkeletonVid = SkeletonVid;
            dstManager.AddComponentData(entity, component);
        }
    }
}
