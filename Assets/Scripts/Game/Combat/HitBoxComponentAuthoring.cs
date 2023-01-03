using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Anvil.Combat
{
    public class HitBoxComponentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            HitBoxComponent component = default(HitBoxComponent);
            dstManager.AddComponentData(entity, component);
            dstManager.AddBuffer<HitBoxInfo>(entity);
        }
    }
}
